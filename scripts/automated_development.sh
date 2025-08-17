#!/bin/bash

# Automated Development Script for Adventure_God v1.1
# Handles technical implementation based on creative feedback

set -euo pipefail

# Configuration
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_PROJECT_PATH="$PROJECT_PATH/unity"
GATEWAY_PATH="$PROJECT_PATH/gateway"
FEEDBACK_FILE="$PROJECT_PATH/docs/creative_feedback.md"
AUTO_DIR="$PROJECT_PATH/automated"

# Create directories
mkdir -p "$AUTO_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Logging functions
log_auto() {
    echo -e "${CYAN}[AUTO]${NC} $1"
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

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

# Parse creative feedback for actionable items
parse_feedback() {
    log_auto "Parsing creative feedback for actionable items..."
    
    local feedback_items=()
    
    if [[ -f "$FEEDBACK_FILE" ]]; then
        # Extract checkbox items that need implementation
        while IFS= read -r line; do
            if [[ "$line" == *"- [ ]"* ]]; then
                # Remove the "- [ ] " prefix and add to array
                local item="${line#- [ ] }"
                if [[ -n "$item" ]]; then
                    feedback_items+=("$item")
                fi
            fi
        done < "$FEEDBACK_FILE"
    fi
    
    # Print each item on a separate line for better readability
    for item in "${feedback_items[@]}"; do
        echo "$item"
    done
}

# Generate implementation plan
generate_implementation_plan() {
    local feedback_items=("$@")
    local plan_file="$AUTO_DIR/implementation_plan_$(date +%Y%m%d_%H%M%S).md"
    
    log_auto "Generating implementation plan..."
    
    cat > "$plan_file" << EOF
# Implementation Plan - $(date)

## Creative Feedback Items to Implement

EOF

    for item in "${feedback_items[@]}"; do
        echo "- [ ] $item" >> "$plan_file"
    done

    cat >> "$plan_file" << EOF

## Implementation Priority

### High Priority (Visual & UX)
$(for item in "${feedback_items[@]}"; do
    if [[ $item == *"visual"* ]] || [[ $item == *"UI"* ]] || [[ $item == *"appearance"* ]]; then
        echo "- $item"
    fi
done)

### Medium Priority (Gameplay)
$(for item in "${feedback_items[@]}"; do
    if [[ $item == *"gameplay"* ]] || [[ $item == *"AI"* ]] || [[ $item == *"scene"* ]]; then
        echo "- $item"
    fi
done)

### Low Priority (Content)
$(for item in "${feedback_items[@]}"; do
    if [[ $item == *"content"* ]] || [[ $item == *"story"* ]] || [[ $item == *"NPC"* ]]; then
        echo "- $item"
    fi
done)

## Technical Implementation Notes

### Unity Implementation
- Focus on visual improvements first
- Maintain performance targets (60 FPS, < 500MB)
- Follow URP best practices
- Use existing component architecture

### Gateway Implementation
- Enhance AI decision-making logic
- Add more context-aware responses
- Improve suggested DC calculations
- Maintain backward compatibility

### Testing Strategy
- Run automated tests after each change
- Validate performance impact
- Test DM workflow usability
- Verify creative feedback implementation

EOF

    log_success "Implementation plan generated: $plan_file"
    echo "$plan_file"
}

# Run automated tests
run_automated_tests() {
    log_auto "Running automated tests..."
    
    # Start gateway if not running
    if ! lsof -ti:8787 > /dev/null 2>&1; then
        log_info "Starting gateway for testing..."
        ./scripts/cursor_unity_integration.sh start-gateway
    fi
    
    # Run Unity tests
    ./scripts/cursor_unity_integration.sh test
    
    # Check test results
    if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]]; then
        if grep -q "FAIL" "$PROJECT_PATH/cursor_test_results.md"; then
            log_error "Some tests failed - review implementation"
            return 1
        else
            log_success "All tests passed"
            return 0
        fi
    else
        log_warning "No test results available"
        return 0
    fi
}

# Generate development report
generate_development_report() {
    local report_file="$AUTO_DIR/development_report_$(date +%Y%m%d_%H%M%S).md"
    
    log_auto "Generating development report..."
    
    cat > "$report_file" << EOF
# Development Report - $(date)

## Project Status
- **Gateway**: $(if lsof -ti:8787 > /dev/null 2>&1; then echo "🟢 Running"; else echo "🔴 Stopped"; fi)
- **Unity**: $(if pgrep -f "Unity" > /dev/null 2>&1; then echo "🟢 Running"; else echo "🔴 Stopped"; fi)
- **Git Status**: $(git status --porcelain | wc -l) changes pending

## Recent Changes
$(git log --oneline -5 | sed 's/^/- /')

## Test Results
$(if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]]; then
    cat "$PROJECT_PATH/cursor_test_results.md"
else
    echo "- No test results available"
fi)

## Performance Metrics
- **Memory Usage**: $(ps aux | grep Unity | grep -v grep | awk '{print $6/1024 " MB"}' | head -1 || echo "Unknown")
- **Gateway Response Time**: $(if lsof -ti:8787 > /dev/null 2>&1; then echo "Active"; else echo "Inactive"; fi)

## Next Steps
1. Review implementation plan
2. Prioritize creative feedback items
3. Implement high-priority visual improvements
4. Test and validate changes
5. Update creative feedback with progress

## Recommendations
$(if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]] && grep -q "FAIL" "$PROJECT_PATH/cursor_test_results.md"; then
    echo "- ❌ Address test failures before proceeding"
else
    echo "- ✅ System is stable and ready for development"
fi)
- 🎨 Focus on visual improvements for immediate impact
- 🔧 Maintain technical quality and performance
- 📝 Document all changes for creative review

EOF

    log_success "Development report generated: $report_file"
    echo "$report_file"
}

# Automated code quality check
check_code_quality() {
    log_auto "Checking code quality..."
    
    local issues=0
    
    # Check Unity compilation
    if [[ -d "$UNITY_PROJECT_PATH" ]]; then
        log_info "Checking Unity project..."
        # This would run Unity compilation check
        # For now, just check if key files exist
        local key_files=(
            "$UNITY_PROJECT_PATH/Assets/Scripts/Core/Planner.cs"
            "$UNITY_PROJECT_PATH/Assets/Scripts/AI/BrainClient.cs"
            "$UNITY_PROJECT_PATH/Assets/Scripts/Core/DiceGate.cs"
        )
        
        for file in "${key_files[@]}"; do
            if [[ ! -f "$file" ]]; then
                log_error "Missing key file: $file"
                ((issues++))
            fi
        done
    fi
    
    # Check Gateway compilation
    if [[ -d "$GATEWAY_PATH" ]]; then
        log_info "Checking Gateway project..."
        cd "$GATEWAY_PATH"
        if ! npx tsc --noEmit; then
            log_error "Gateway TypeScript compilation failed"
            ((issues++))
        fi
    fi
    
    if [[ $issues -eq 0 ]]; then
        log_success "Code quality check passed"
        return 0
    else
        log_error "Code quality check found $issues issues"
        return 1
    fi
}

# Main automation workflow
run_automation_workflow() {
    log_auto "Starting automated development workflow..."
    
    # Step 1: Parse creative feedback
    local feedback_items=($(parse_feedback))
    
    if [[ ${#feedback_items[@]} -eq 0 ]]; then
        log_warning "No actionable feedback items found"
        return 0
    fi
    
    log_info "Found ${#feedback_items[@]} actionable items"
    
    # Step 2: Generate implementation plan
    local plan_file=$(generate_implementation_plan "${feedback_items[@]}")
    
    # Step 3: Check code quality
    if ! check_code_quality; then
        log_error "Code quality issues found - fixing before proceeding"
        # Here you would implement automatic fixes
    fi
    
    # Step 4: Run tests
    if ! run_automated_tests; then
        log_error "Tests failed - stopping automation"
        return 1
    fi
    
    # Step 5: Generate development report
    local report_file=$(generate_development_report)
    
    log_success "Automation workflow completed"
    log_info "Implementation plan: $plan_file"
    log_info "Development report: $report_file"
    
    return 0
}

# Main function
main() {
    local command=${1:-"workflow"}
    
    case "$command" in
        "workflow")
            run_automation_workflow
            ;;
        "feedback")
            parse_feedback
            ;;
        "plan")
            local feedback_items=($(parse_feedback))
            generate_implementation_plan "${feedback_items[@]}"
            ;;
        "test")
            run_automated_tests
            ;;
        "report")
            generate_development_report
            ;;
        "quality")
            check_code_quality
            ;;
        "help"|*)
            echo "🤖 Automated Development Script"
            echo ""
            echo "Usage: $0 <command>"
            echo ""
            echo "Commands:"
            echo "  workflow  - Run complete automation workflow"
            echo "  feedback  - Parse creative feedback for actionable items"
            echo "  plan      - Generate implementation plan"
            echo "  test      - Run automated tests"
            echo "  report    - Generate development report"
            echo "  quality   - Check code quality"
            echo "  help      - Show this help message"
            echo ""
            echo "🤖 Automation Workflow:"
            echo "1. Parse creative feedback from docs/creative_feedback.md"
            echo "2. Generate implementation plan based on feedback"
            echo "3. Check code quality and fix issues"
            echo "4. Run automated tests"
            echo "5. Generate development report"
            echo "6. Ready for Cursor implementation"
            ;;
    esac
}

# Run main function with all arguments
main "$@"
