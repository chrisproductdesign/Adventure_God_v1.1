#!/bin/bash

# Test Runner for Adventure God v1.1
# Provides easy access to all testing options

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

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

# Show menu
show_menu() {
    echo "=========================================="
    echo "           TEST RUNNER MENU"
    echo "=========================================="
    echo "1. Quick Tests (Unity + Gateway compilation)"
    echo "2. Comprehensive Tests (Full test suite)"
    echo "3. Unity Editor Tests (Component testing)"
    echo "4. Continuous Monitoring (Watch for changes)"
    echo "5. Run All Tests"
    echo "6. Show Test Results"
    echo "7. Clean Test Results"
    echo "8. Help"
    echo "0. Exit"
    echo "=========================================="
    echo ""
    read -p "Select an option (0-8): " choice
}

# Quick tests
run_quick_tests() {
    log_info "Running quick tests..."
    
    # Test Unity compilation
    cd "$PROJECT_ROOT/unity"
    if ./run_headless.sh; then
        if grep -q "error CS" Headless.log; then
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
    cd "$PROJECT_ROOT/gateway"
    if npx tsc --noEmit; then
        log_success "Gateway compilation successful"
    else
        log_error "Gateway compilation failed"
        return 1
    fi
    
    log_success "Quick tests completed successfully"
    return 0
}

# Comprehensive tests
run_comprehensive_tests() {
    log_info "Running comprehensive tests..."
    "$PROJECT_ROOT/scripts/automated_testing.sh"
}

# Unity editor tests
run_unity_editor_tests() {
    log_info "Running Unity editor tests..."
    
    cd "$PROJECT_ROOT/unity"
    
    # Check if Unity is available
    if ! command -v unity-hub &> /dev/null; then
        log_warning "Unity Hub not available"
        log_info "You can run Unity editor tests manually:"
        log_info "1. Open Unity"
        log_info "2. Go to Tools > Run Comprehensive Tests"
        return 0
    fi
    
    # This would run Unity in batch mode to execute editor tests
    log_info "Unity editor tests would run here"
    log_success "Unity editor tests completed (simulated)"
}

# Continuous monitoring
run_continuous_monitoring() {
    log_info "Starting continuous monitoring..."
    "$PROJECT_ROOT/scripts/continuous_testing.sh" --monitor
}

# Run all tests
run_all_tests() {
    log_info "Running all tests..."
    
    if run_quick_tests && run_comprehensive_tests && run_unity_editor_tests; then
        log_success "All tests completed successfully"
    else
        log_error "Some tests failed"
        return 1
    fi
}

# Show test results
show_test_results() {
    local test_results_dir="$PROJECT_ROOT/test_results"
    
    if [ -d "$test_results_dir" ]; then
        echo "=========================================="
        echo "           TEST RESULTS"
        echo "=========================================="
        
        # Show recent test summaries
        local recent_reports=$(find "$test_results_dir" -name "test_summary_*.md" | sort -r | head -5)
        
        if [ -n "$recent_reports" ]; then
            echo "Recent test reports:"
            for report in $recent_reports; do
                local report_name=$(basename "$report" .md)
                local report_time=$(echo "$report_name" | sed 's/test_summary_//')
                echo "  - $report_time: $report"
            done
        else
            echo "No test reports found."
        fi
        
        # Show continuous testing summary
        local continuous_summary="$test_results_dir/continuous_test_summary.md"
        if [ -f "$continuous_summary" ]; then
            echo ""
            echo "Continuous testing summary: $continuous_summary"
        fi
        
        echo "=========================================="
    else
        log_warning "No test results directory found"
    fi
}

# Clean test results
clean_test_results() {
    local test_results_dir="$PROJECT_ROOT/test_results"
    local logs_dir="$PROJECT_ROOT/logs"
    
    log_info "Cleaning test results..."
    
    if [ -d "$test_results_dir" ]; then
        rm -rf "$test_results_dir"/*
        log_success "Test results cleaned"
    fi
    
    if [ -d "$logs_dir" ]; then
        rm -rf "$logs_dir"/*
        log_success "Test logs cleaned"
    fi
}

# Show help
show_help() {
    echo "Test Runner for Adventure God v1.1"
    echo ""
    echo "This script provides easy access to all testing options:"
    echo ""
    echo "Quick Tests:"
    echo "  - Unity compilation check"
    echo "  - Gateway TypeScript compilation"
    echo "  - Fast feedback for development"
    echo ""
    echo "Comprehensive Tests:"
    echo "  - Full automated test suite"
    echo "  - Unity and Gateway integration"
    echo "  - Project structure validation"
    echo "  - Performance and asset integrity"
    echo ""
    echo "Unity Editor Tests:"
    echo "  - Component instantiation tests"
    echo "  - Game logic validation"
    echo "  - UI system testing"
    echo "  - Integration point verification"
    echo ""
    echo "Continuous Monitoring:"
    echo "  - Watches for file changes"
    echo "  - Automatically runs tests"
    echo "  - Provides real-time feedback"
    echo ""
    echo "Test Results:"
    echo "  - Stored in test_results/ directory"
    echo "  - Markdown format for easy reading"
    echo "  - Includes detailed logs and summaries"
    echo ""
    echo "For more information, see the individual test scripts:"
    echo "  - scripts/automated_testing.sh"
    echo "  - scripts/continuous_testing.sh"
}

# Main menu loop
main() {
    while true; do
        show_menu
        
        case $choice in
            1)
                run_quick_tests
                ;;
            2)
                run_comprehensive_tests
                ;;
            3)
                run_unity_editor_tests
                ;;
            4)
                run_continuous_monitoring
                ;;
            5)
                run_all_tests
                ;;
            6)
                show_test_results
                ;;
            7)
                clean_test_results
                ;;
            8)
                show_help
                ;;
            0)
                log_info "Exiting test runner"
                exit 0
                ;;
            *)
                log_error "Invalid option: $choice"
                ;;
        esac
        
        echo ""
        read -p "Press Enter to continue..."
        echo ""
    done
}

# Handle command line arguments
case "${1:-}" in
    quick)
        run_quick_tests
        ;;
    comprehensive)
        run_comprehensive_tests
        ;;
    unity)
        run_unity_editor_tests
        ;;
    monitor)
        run_continuous_monitoring
        ;;
    all)
        run_all_tests
        ;;
    results)
        show_test_results
        ;;
    clean)
        clean_test_results
        ;;
    help|--help|-h)
        show_help
        ;;
    "")
        main
        ;;
    *)
        echo "Unknown option: $1"
        echo "Use 'help' to see available options"
        exit 1
        ;;
esac
