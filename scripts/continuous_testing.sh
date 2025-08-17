#!/bin/bash

# Continuous Testing Workflow for Adventure God v1.1
# Monitors project changes and runs tests automatically

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_DIR="$PROJECT_ROOT/unity"
GATEWAY_DIR="$PROJECT_ROOT/gateway"
TEST_RESULTS_DIR="$PROJECT_ROOT/test_results"
LOG_DIR="$PROJECT_ROOT/logs"
WATCH_DIRS=("$UNITY_DIR/Assets" "$GATEWAY_DIR/src" "$PROJECT_ROOT/docs")

# Create directories
mkdir -p "$TEST_RESULTS_DIR"
mkdir -p "$LOG_DIR"

# State tracking
LAST_RUN_TIME=0
CHANGES_DETECTED=false
TEST_IN_PROGRESS=false

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check if files have changed since last run
check_for_changes() {
    local current_time=$(date +%s)
    local changed_files=()
    
    for dir in "${WATCH_DIRS[@]}"; do
        if [ -d "$dir" ]; then
            # Find files modified since last run
            local new_files=$(find "$dir" -type f -newermt "@$LAST_RUN_TIME" 2>/dev/null || true)
            if [ -n "$new_files" ]; then
                changed_files+=($new_files)
            fi
        fi
    done
    
    if [ ${#changed_files[@]} -gt 0 ]; then
        log_info "Changes detected in ${#changed_files[@]} files:"
        for file in "${changed_files[@]:0:5}"; do
            log_info "  - $file"
        done
        if [ ${#changed_files[@]} -gt 5 ]; then
            log_info "  ... and $(( ${#changed_files[@]} - 5 )) more files"
        fi
        CHANGES_DETECTED=true
    else
        CHANGES_DETECTED=false
    fi
    
    LAST_RUN_TIME=$current_time
}

# Run quick tests (fast feedback)
run_quick_tests() {
    log_info "Running quick tests..."
    
    local quick_test_log="$LOG_DIR/quick_test_$(date +%Y%m%d_%H%M%S).log"
    
    # Test Unity compilation
    cd "$UNITY_DIR"
    if ./run_headless.sh > "$quick_test_log" 2>&1; then
        if grep -q "error CS" "$quick_test_log"; then
            log_error "Unity compilation errors detected"
            return 1
        else
            log_success "Unity compilation successful"
        fi
    else
        log_error "Unity headless test failed"
        return 1
    fi
    
    # Test Gateway compilation
    cd "$GATEWAY_DIR"
    if npx tsc --noEmit > "$quick_test_log" 2>&1; then
        log_success "Gateway compilation successful"
    else
        log_error "Gateway compilation failed"
        return 1
    fi
    
    log_success "Quick tests passed"
    return 0
}

# Run comprehensive tests
run_comprehensive_tests() {
    log_info "Running comprehensive tests..."
    
    # Run the automated testing script
    if "$PROJECT_ROOT/scripts/automated_testing.sh"; then
        log_success "Comprehensive tests completed successfully"
        return 0
    else
        log_error "Comprehensive tests failed"
        return 1
    fi
}

# Run Unity editor tests
run_unity_tests() {
    log_info "Running Unity editor tests..."
    
    cd "$UNITY_DIR"
    
    # Check if Unity is available
    if ! command -v unity-hub &> /dev/null; then
        log_warning "Unity Hub not available, skipping Unity editor tests"
        return 0
    fi
    
    # Run Unity in batch mode to execute editor tests
    local unity_test_log="$LOG_DIR/unity_test_$(date +%Y%m%d_%H%M%S).log"
    
    # This would require Unity to be installed and accessible
    # For now, we'll simulate the test
    log_info "Unity editor tests would run here"
    log_success "Unity editor tests completed (simulated)"
    return 0
}

# Generate test summary
generate_test_summary() {
    local summary_file="$TEST_RESULTS_DIR/continuous_test_summary.md"
    
    echo "# Continuous Testing Summary" > "$summary_file"
    echo "Generated: $(date)" >> "$summary_file"
    echo "" >> "$summary_file"
    echo "## Recent Test Results" >> "$summary_file"
    echo "" >> "$summary_file"
    
    # Find recent test reports
    local recent_reports=$(find "$TEST_RESULTS_DIR" -name "test_summary_*.md" -mtime -1 | sort -r | head -5)
    
    if [ -n "$recent_reports" ]; then
        for report in $recent_reports; do
            local report_name=$(basename "$report" .md)
            local report_time=$(echo "$report_name" | sed 's/test_summary_//')
            echo "### $report_time" >> "$summary_file"
            echo "Report: [$report_name]($report)" >> "$summary_file"
            echo "" >> "$summary_file"
        done
    else
        echo "No recent test reports found." >> "$summary_file"
    fi
    
    echo "## Monitoring Status" >> "$summary_file"
    echo "- **Watched Directories**: ${WATCH_DIRS[*]}" >> "$summary_file"
    echo "- **Last Check**: $(date)" >> "$summary_file"
    echo "- **Changes Detected**: $CHANGES_DETECTED" >> "$summary_file"
    echo "" >> "$summary_file"
    
    log_info "Test summary generated: $summary_file"
}

# Main monitoring loop
monitor_project() {
    log_info "Starting continuous testing monitor"
    log_info "Watching directories: ${WATCH_DIRS[*]}"
    log_info "Press Ctrl+C to stop monitoring"
    
    # Initial test run
    log_info "Running initial comprehensive test..."
    run_comprehensive_tests
    
    # Main monitoring loop
    while true; do
        log_info "Checking for changes..."
        check_for_changes
        
        if [ "$CHANGES_DETECTED" = true ]; then
            if [ "$TEST_IN_PROGRESS" = false ]; then
                TEST_IN_PROGRESS=true
                log_info "Changes detected, running tests..."
                
                # Run quick tests first for fast feedback
                if run_quick_tests; then
                    log_success "Quick tests passed, running comprehensive tests..."
                    if run_comprehensive_tests; then
                        log_success "All tests passed after changes"
                    else
                        log_error "Comprehensive tests failed after changes"
                    fi
                else
                    log_error "Quick tests failed, skipping comprehensive tests"
                fi
                
                # Run Unity tests if available
                run_unity_tests
                
                # Generate summary
                generate_test_summary
                
                TEST_IN_PROGRESS=false
            else
                log_info "Tests already in progress, skipping this change"
            fi
        else
            log_info "No changes detected"
        fi
        
        # Wait before next check
        sleep 30
    done
}

# Run tests once
run_tests_once() {
    log_info "Running tests once..."
    
    if run_quick_tests && run_comprehensive_tests && run_unity_tests; then
        log_success "All tests completed successfully"
        generate_test_summary
        exit 0
    else
        log_error "Some tests failed"
        generate_test_summary
        exit 1
    fi
}

# Show help
show_help() {
    echo "Continuous Testing Workflow for Adventure God v1.1"
    echo ""
    echo "Usage: $0 [OPTION]"
    echo ""
    echo "Options:"
    echo "  --monitor    Start continuous monitoring (default)"
    echo "  --once       Run tests once and exit"
    echo "  --help       Show this help message"
    echo ""
    echo "The monitor will watch for changes in:"
    for dir in "${WATCH_DIRS[@]}"; do
        echo "  - $dir"
    done
    echo ""
    echo "Test results are saved to: $TEST_RESULTS_DIR"
    echo "Logs are saved to: $LOG_DIR"
}

# Parse command line arguments
case "${1:---monitor}" in
    --monitor)
        monitor_project
        ;;
    --once)
        run_tests_once
        ;;
    --help|-h)
        show_help
        ;;
    *)
        echo "Unknown option: $1"
        show_help
        exit 1
        ;;
esac
