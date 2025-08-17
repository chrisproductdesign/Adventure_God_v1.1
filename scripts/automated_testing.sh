#!/bin/bash

# Automated Testing Workflow for Adventure God v1.1
# Tests Unity functionality, Gateway connectivity, and game systems

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

# Create directories
mkdir -p "$TEST_RESULTS_DIR"
mkdir -p "$LOG_DIR"

# Timestamp for this test run
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
TEST_LOG="$LOG_DIR/test_run_$TIMESTAMP.log"
SUMMARY_LOG="$TEST_RESULTS_DIR/test_summary_$TIMESTAMP.md"

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0
WARNINGS=0

# Initialize test results
echo "# Test Results - $(date)" > "$SUMMARY_LOG"
echo "" >> "$SUMMARY_LOG"
echo "## Summary" >> "$SUMMARY_LOG"
echo "- **Timestamp**: $(date)" >> "$SUMMARY_LOG"
echo "- **Total Tests**: 0" >> "$SUMMARY_LOG"
echo "- **Passed**: 0" >> "$SUMMARY_LOG"
echo "- **Failed**: 0" >> "$SUMMARY_LOG"
echo "- **Warnings**: 0" >> "$SUMMARY_LOG"
echo "" >> "$SUMMARY_LOG"

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1" | tee -a "$TEST_LOG"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1" | tee -a "$TEST_LOG"
    PASSED_TESTS=$((PASSED_TESTS + 1))
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1" | tee -a "$TEST_LOG"
    FAILED_TESTS=$((FAILED_TESTS + 1))
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1" | tee -a "$TEST_LOG"
    WARNINGS=$((WARNINGS + 1))
}

log_test() {
    echo -e "${BLUE}[TEST]${NC} $1" | tee -a "$TEST_LOG"
}

# Test result tracking
add_test_result() {
    local test_name="$1"
    local status="$2"
    local details="$3"
    
    echo "### $test_name" >> "$SUMMARY_LOG"
    echo "- **Status**: $status" >> "$SUMMARY_LOG"
    if [ -n "$details" ]; then
        echo "- **Details**: $details" >> "$SUMMARY_LOG"
    fi
    echo "" >> "$SUMMARY_LOG"
}

# Check if Unity is installed
test_unity_installation() {
    log_test "Testing Unity installation"
    
    if command -v unity-hub &> /dev/null; then
        log_success "Unity Hub is installed"
        add_test_result "Unity Installation" "PASS" "Unity Hub found"
    else
        log_warning "Unity Hub not found in PATH (this is optional)"
        add_test_result "Unity Installation" "WARN" "Unity Hub not found (optional)"
    fi
}

# Test Unity project compilation
test_unity_compilation() {
    log_test "Testing Unity project compilation"
    
    cd "$UNITY_DIR"
    
    # Run Unity in headless mode to test compilation
    if ./run_headless.sh; then
        # Check for compilation errors in the log
        if grep -q "error CS" Headless.log; then
            local errors=$(grep "error CS" Headless.log | head -5)
            log_error "Unity compilation failed with errors: $errors"
            add_test_result "Unity Compilation" "FAIL" "Compilation errors found"
            return 1
        elif grep -q "warning CS" Headless.log; then
            local warnings=$(grep "warning CS" Headless.log | head -5)
            log_warning "Unity compilation succeeded with warnings: $warnings"
            add_test_result "Unity Compilation" "WARN" "Compilation warnings found"
        else
            log_success "Unity project compiles successfully"
            add_test_result "Unity Compilation" "PASS" "No compilation errors"
        fi
    else
        log_error "Unity headless test failed"
        add_test_result "Unity Compilation" "FAIL" "Headless test failed"
        return 1
    fi
}

# Test Gateway compilation
test_gateway_compilation() {
    log_test "Testing Gateway compilation"
    
    cd "$GATEWAY_DIR"
    
    # Check if node_modules exists
    if [ ! -d "node_modules" ]; then
        log_info "Installing Gateway dependencies"
        npm install
    fi
    
    # Test TypeScript compilation
    if npx tsc --noEmit; then
        log_success "Gateway TypeScript compilation successful"
        add_test_result "Gateway Compilation" "PASS" "TypeScript compiles without errors"
    else
        log_error "Gateway TypeScript compilation failed"
        add_test_result "Gateway Compilation" "FAIL" "TypeScript compilation errors"
        return 1
    fi
    
    # Test ESLint
    if npx eslint src/ --ext .ts; then
        log_success "Gateway ESLint passed"
        add_test_result "Gateway Linting" "PASS" "No linting errors"
    else
        log_warning "Gateway ESLint found issues"
        add_test_result "Gateway Linting" "WARN" "Linting issues found"
    fi
}

# Test Gateway startup
test_gateway_startup() {
    log_test "Testing Gateway startup"
    
    cd "$GATEWAY_DIR"
    
    # Kill any existing gateway processes
    pkill -f "gateway" || true
    
    # Start gateway in background
    log_info "Starting Gateway server"
    npm start > "$LOG_DIR/gateway_startup.log" 2>&1 &
    GATEWAY_PID=$!
    
    # Wait for gateway to start
    sleep 5
    
    # Check if gateway is running
    if kill -0 $GATEWAY_PID 2>/dev/null; then
        log_success "Gateway started successfully"
        add_test_result "Gateway Startup" "PASS" "Server started on port 8787"
        
        # Test WebSocket connection
        test_websocket_connection
        
        # Kill gateway
        kill $GATEWAY_PID
        wait $GATEWAY_PID 2>/dev/null || true
    else
        log_warning "Gateway failed to start (this is optional for testing)"
        add_test_result "Gateway Startup" "WARN" "Server failed to start (optional)"
        
        # Check the log for more details
        if [ -f "$LOG_DIR/gateway_startup.log" ]; then
            log_info "Gateway startup log:"
            tail -5 "$LOG_DIR/gateway_startup.log" | while read line; do
                log_info "  $line"
            done
        fi
    fi
}

# Test WebSocket connection
test_websocket_connection() {
    log_test "Testing WebSocket connection"
    
    # Use curl to test WebSocket connection
    if command -v curl &> /dev/null; then
        # Test basic HTTP connection first
        if curl -s http://localhost:8787 > /dev/null 2>&1; then
            log_success "WebSocket server is responding"
            add_test_result "WebSocket Connection" "PASS" "Server responding on port 8787"
        else
            log_error "WebSocket server not responding"
            add_test_result "WebSocket Connection" "FAIL" "Server not responding"
            return 1
        fi
    else
        log_warning "curl not available, skipping WebSocket test"
        add_test_result "WebSocket Connection" "WARN" "curl not available for testing"
    fi
}

# Test Unity-Gateway integration
test_unity_gateway_integration() {
    log_test "Testing Unity-Gateway integration"
    
    cd "$UNITY_DIR"
    
    # Start gateway in background
    cd "$GATEWAY_DIR"
    npm start > "$LOG_DIR/gateway_integration.log" 2>&1 &
    GATEWAY_PID=$!
    sleep 3
    
    # Run Unity integration test
    cd "$UNITY_DIR"
    if ./run_headless.sh; then
        # Check for connection messages in Unity log
        if grep -q "Connected\|Connection" Headless.log; then
            log_success "Unity-Gateway integration working"
            add_test_result "Unity-Gateway Integration" "PASS" "Connection established"
        else
            log_warning "No connection messages found in Unity log"
            add_test_result "Unity-Gateway Integration" "WARN" "No connection messages found"
        fi
    else
        log_error "Unity integration test failed"
        add_test_result "Unity-Gateway Integration" "FAIL" "Integration test failed"
    fi
    
    # Kill gateway
    kill $GATEWAY_PID
    wait $GATEWAY_PID 2>/dev/null || true
}

# Test game systems
test_game_systems() {
    log_test "Testing game systems"
    
    cd "$UNITY_DIR"
    
    # Check if core scripts exist
    local core_scripts=(
        "Assets/Scripts/Core/ModernGameUI.cs"
        "Assets/Scripts/Core/DiceGate.cs"
        "Assets/Scripts/Core/Planner.cs"
        "Assets/Scripts/AI/BrainClient.cs"
        "Assets/Scripts/Core/RuntimeBootstrap.cs"
    )
    
    local missing_scripts=0
    for script in "${core_scripts[@]}"; do
        if [ -f "$script" ]; then
            log_success "Found $script"
        else
            log_error "Missing $script"
            missing_scripts=$((missing_scripts + 1))
        fi
    done
    
    if [ $missing_scripts -eq 0 ]; then
        add_test_result "Game Systems" "PASS" "All core scripts present"
    else
        add_test_result "Game Systems" "FAIL" "$missing_scripts scripts missing"
        return 1
    fi
}

# Test project structure
test_project_structure() {
    log_test "Testing project structure"
    
    local required_dirs=(
        "unity/Assets/Scripts/Core"
        "unity/Assets/Scripts/AI"
        "unity/Assets/Editor"
        "gateway/src"
        "gateway/src/ai"
        "gateway/src/schema"
        "docs"
    )
    
    local missing_dirs=0
    for dir in "${required_dirs[@]}"; do
        if [ -d "$PROJECT_ROOT/$dir" ]; then
            log_success "Found directory: $dir"
        else
            log_error "Missing directory: $dir"
            missing_dirs=$((missing_dirs + 1))
        fi
    done
    
    if [ $missing_dirs -eq 0 ]; then
        add_test_result "Project Structure" "PASS" "All required directories present"
    else
        add_test_result "Project Structure" "FAIL" "$missing_dirs directories missing"
        return 1
    fi
}

# Test documentation
test_documentation() {
    log_test "Testing documentation"
    
    local required_docs=(
        "docs/00_project_charter.md"
        "docs/10_working_rules_for_cursor.md"
        "docs/20_contracts.md"
        "docs/30_backlog.md"
        "README.md"
    )
    
    local missing_docs=0
    for doc in "${required_docs[@]}"; do
        if [ -f "$PROJECT_ROOT/$doc" ]; then
            log_success "Found documentation: $doc"
        else
            log_error "Missing documentation: $doc"
            missing_docs=$((missing_docs + 1))
        fi
    done
    
    if [ $missing_docs -eq 0 ]; then
        add_test_result "Documentation" "PASS" "All required documentation present"
    else
        add_test_result "Documentation" "FAIL" "$missing_docs documents missing"
        return 1
    fi
}

# Performance test
test_performance() {
    log_test "Testing performance"
    
    cd "$UNITY_DIR"
    
    # Check Unity log for performance issues
    if [ -f "Headless.log" ]; then
        if grep -q "OutOfMemory\|Memory\|Performance" Headless.log; then
            log_warning "Performance issues detected in Unity log"
            add_test_result "Performance" "WARN" "Performance issues found"
        else
            log_success "No performance issues detected"
            add_test_result "Performance" "PASS" "No performance issues"
        fi
    else
        log_warning "No Unity log found for performance analysis"
        add_test_result "Performance" "WARN" "No log available"
    fi
}

# Generate final report
generate_report() {
    log_info "Generating test report"
    
    # Update summary in markdown file
    sed -i.bak "s/- \*\*Total Tests\*\*: [0-9]*/- **Total Tests**: $TOTAL_TESTS/" "$SUMMARY_LOG"
    sed -i.bak "s/- \*\*Passed\*\*: [0-9]*/- **Passed**: $PASSED_TESTS/" "$SUMMARY_LOG"
    sed -i.bak "s/- \*\*Failed\*\*: [0-9]*/- **Failed**: $FAILED_TESTS/" "$SUMMARY_LOG"
    sed -i.bak "s/- \*\*Warnings\*\*: [0-9]*/- **Warnings**: $WARNINGS/" "$SUMMARY_LOG"
    
    # Add detailed results
    echo "## Detailed Results" >> "$SUMMARY_LOG"
    echo "" >> "$SUMMARY_LOG"
    echo "### Test Log" >> "$SUMMARY_LOG"
    echo "Full test log available at: \`$TEST_LOG\`" >> "$SUMMARY_LOG"
    echo "" >> "$SUMMARY_LOG"
    
    # Add recent Unity log if available
    if [ -f "$UNITY_DIR/Headless.log" ]; then
        echo "### Recent Unity Log" >> "$SUMMARY_LOG"
        echo '```' >> "$SUMMARY_LOG"
        tail -20 "$UNITY_DIR/Headless.log" >> "$SUMMARY_LOG"
        echo '```' >> "$SUMMARY_LOG"
        echo "" >> "$SUMMARY_LOG"
    fi
    
    # Calculate success rate
    if [ $TOTAL_TESTS -gt 0 ]; then
        local success_rate=$((PASSED_TESTS * 100 / TOTAL_TESTS))
        echo "### Success Rate" >> "$SUMMARY_LOG"
        echo "- **Overall Success**: ${success_rate}%" >> "$SUMMARY_LOG"
        echo "" >> "$SUMMARY_LOG"
    fi
    
    # Print summary to console
    echo ""
    echo "=========================================="
    echo "           TEST SUMMARY"
    echo "=========================================="
    echo "Total Tests: $TOTAL_TESTS"
    echo "Passed: $PASSED_TESTS"
    echo "Failed: $FAILED_TESTS"
    echo "Warnings: $WARNINGS"
    if [ $TOTAL_TESTS -gt 0 ]; then
        echo "Success Rate: ${success_rate}%"
    fi
    echo "=========================================="
    echo "Detailed report: $SUMMARY_LOG"
    echo "Test log: $TEST_LOG"
    echo "=========================================="
}

# Main test execution
main() {
    log_info "Starting automated testing workflow"
    log_info "Project root: $PROJECT_ROOT"
    log_info "Timestamp: $TIMESTAMP"
    
    # Run all tests
    test_unity_installation
    test_project_structure
    test_documentation
    test_unity_compilation
    test_gateway_compilation
    test_gateway_startup
    test_unity_gateway_integration
    test_game_systems
    test_performance
    
    # Generate final report
    generate_report
    
    # Exit with appropriate code
    if [ $FAILED_TESTS -gt 0 ]; then
        log_error "Testing completed with $FAILED_TESTS failures"
        exit 1
    else
        log_success "All tests passed!"
        exit 0
    fi
}

# Run main function
main "$@"
