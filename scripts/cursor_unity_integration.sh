#!/bin/bash

# Cursor Unity Integration Script
# Provides comprehensive integration between Cursor and Unity for Adventure_God v1.1

set -euo pipefail

# Configuration
UNITY_PATH=${UNITY_PATH:-"/Applications/Unity/Hub/Editor/2022.3.62f1/Unity.app/Contents/MacOS/Unity"}
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_PROJECT_PATH="$PROJECT_PATH/unity"
GATEWAY_PATH="$PROJECT_PATH/gateway"
LOG_DIR="$PROJECT_PATH/logs"
REPORTS_DIR="$PROJECT_PATH/reports"

# Create directories
mkdir -p "$LOG_DIR" "$REPORTS_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Unity is available
check_unity() {
    if [[ ! -f "$UNITY_PATH" ]]; then
        log_error "Unity not found at: $UNITY_PATH"
        log_info "Please set UNITY_PATH environment variable or install Unity"
        return 1
    fi
    log_success "Unity found at: $UNITY_PATH"
}

# Check if gateway is running
check_gateway() {
    if lsof -ti:8787 > /dev/null 2>&1; then
        log_success "Gateway is running on port 8787"
        return 0
    else
        log_warning "Gateway is not running on port 8787"
        return 1
    fi
}

# Start gateway if not running
start_gateway() {
    if check_gateway; then
        log_info "Gateway already running"
        return 0
    fi
    
    log_info "Starting gateway..."
    cd "$GATEWAY_PATH"
    
    # Kill any existing processes
    lsof -ti:8787 | xargs -r kill -9 2>/dev/null || true
    
    # Start gateway in background
    nohup npx ts-node src/index.ts > "$LOG_DIR/gateway.log" 2>&1 &
    local gateway_pid=$!
    echo $gateway_pid > "$LOG_DIR/gateway.pid"
    
    # Wait for gateway to start
    sleep 3
    
    if check_gateway; then
        log_success "Gateway started successfully (PID: $gateway_pid)"
        return 0
    else
        log_error "Failed to start gateway"
        return 1
    fi
}

# Stop gateway
stop_gateway() {
    if [[ -f "$LOG_DIR/gateway.pid" ]]; then
        local pid=$(cat "$LOG_DIR/gateway.pid")
        if kill -0 "$pid" 2>/dev/null; then
            log_info "Stopping gateway (PID: $pid)..."
            kill "$pid"
            rm "$LOG_DIR/gateway.pid"
            log_success "Gateway stopped"
        fi
    fi
    
    # Kill any remaining processes on port 8787
    lsof -ti:8787 | xargs -r kill -9 2>/dev/null || true
}

# Run Unity headless test
run_unity_test() {
    local test_name=${1:-"comprehensive"}
    local log_file="$LOG_DIR/unity_test_${test_name}_$(date +%Y%m%d_%H%M%S).log"
    
    log_info "Running Unity headless test: $test_name"
    log_info "Log file: $log_file"
    
    cd "$UNITY_PROJECT_PATH"
    
    # Run Unity in headless mode
    "$UNITY_PATH" -batchmode -quit -nographics \
        -projectPath "$UNITY_PROJECT_PATH" \
        -executeMethod CursorHeadlessTest.RunComprehensiveTest \
        -logFile "$log_file"
    
    local exit_code=$?
    
    if [[ $exit_code -eq 0 ]]; then
        log_success "Unity test completed successfully"
    else
        log_error "Unity test failed with exit code: $exit_code"
    fi
    
    # Check for test results
    if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]]; then
        log_info "Test results available at: $PROJECT_PATH/cursor_test_results.md"
        cat "$PROJECT_PATH/cursor_test_results.md"
    fi
    
    return $exit_code
}

# Validate project setup
validate_project() {
    log_info "Validating project setup..."
    
    # Check Unity project
    if [[ ! -d "$UNITY_PROJECT_PATH" ]]; then
        log_error "Unity project not found at: $UNITY_PROJECT_PATH"
        return 1
    fi
    
    # Check gateway project
    if [[ ! -d "$GATEWAY_PATH" ]]; then
        log_error "Gateway project not found at: $GATEWAY_PATH"
        return 1
    fi
    
    # Check required files
    local required_files=(
        "$UNITY_PROJECT_PATH/Assets/Scripts/Core/Planner.cs"
        "$UNITY_PROJECT_PATH/Assets/Scripts/AI/BrainClient.cs"
        "$GATEWAY_PATH/src/index.ts"
        "$GATEWAY_PATH/src/schema/events.ts"
    )
    
    for file in "${required_files[@]}"; do
        if [[ ! -f "$file" ]]; then
            log_error "Required file not found: $file"
            return 1
        fi
    done
    
    log_success "Project validation passed"
    return 0
}

# Generate project report
generate_report() {
    local report_file="$REPORTS_DIR/project_report_$(date +%Y%m%d_%H%M%S).md"
    
    log_info "Generating project report: $report_file"
    
    cat > "$report_file" << EOF
# Adventure_God v1.1 Project Report
Generated: $(date)

## Project Structure
- Unity Project: $UNITY_PROJECT_PATH
- Gateway Project: $GATEWAY_PATH
- Unity Version: $(basename "$(dirname "$(dirname "$UNITY_PATH")")")

## Component Status
EOF
    
    # Check Unity components
    cd "$UNITY_PROJECT_PATH"
    if [[ -f "$PROJECT_PATH/cursor_validation_report.md" ]]; then
        cat "$PROJECT_PATH/cursor_validation_report.md" >> "$report_file"
    fi
    
    # Check gateway status
    echo "" >> "$report_file"
    echo "## Gateway Status" >> "$report_file"
    if check_gateway; then
        echo "- ✅ Gateway is running" >> "$report_file"
    else
        echo "- ❌ Gateway is not running" >> "$report_file"
    fi
    
    # Check recent logs
    echo "" >> "$report_file"
    echo "## Recent Logs" >> "$report_file"
    if [[ -f "$LOG_DIR/gateway.log" ]]; then
        echo "### Gateway Log (last 10 lines)" >> "$report_file"
        tail -10 "$LOG_DIR/gateway.log" >> "$report_file"
    fi
    
    log_success "Report generated: $report_file"
}

# Main function
main() {
    local command=${1:-"help"}
    
    case "$command" in
        "check")
            check_unity
            check_gateway
            validate_project
            ;;
        "start-gateway")
            start_gateway
            ;;
        "stop-gateway")
            stop_gateway
            ;;
        "test")
            start_gateway
            run_unity_test
            ;;
        "report")
            generate_report
            ;;
        "clean")
            stop_gateway
            rm -rf "$LOG_DIR"/* "$REPORTS_DIR"/*
            log_success "Cleaned logs and reports"
            ;;
        "help"|*)
            echo "Cursor Unity Integration Script"
            echo ""
            echo "Usage: $0 <command>"
            echo ""
            echo "Commands:"
            echo "  check         - Check Unity, gateway, and project setup"
            echo "  start-gateway - Start the gateway server"
            echo "  stop-gateway  - Stop the gateway server"
            echo "  test          - Run Unity headless test with gateway"
            echo "  report        - Generate project status report"
            echo "  clean         - Clean logs and reports"
            echo "  help          - Show this help message"
            echo ""
            echo "Environment Variables:"
            echo "  UNITY_PATH    - Path to Unity executable (default: $UNITY_PATH)"
            ;;
    esac
}

# Run main function with all arguments
main "$@"
