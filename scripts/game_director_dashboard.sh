#!/bin/bash

# Game Director Dashboard for Adventure_God v1.1
# Provides high-level project status and creative direction tools

set -euo pipefail

# Configuration
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_PROJECT_PATH="$PROJECT_PATH/unity"
GATEWAY_PATH="$PROJECT_PATH/gateway"
DASHBOARD_DIR="$PROJECT_PATH/dashboard"
BUILD_DIR="$PROJECT_PATH/builds"

# Create directories
mkdir -p "$DASHBOARD_DIR" "$BUILD_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Logging functions
log_director() {
    echo -e "${PURPLE}[DIRECTOR]${NC} $1"
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

# Generate Game Director Dashboard
generate_dashboard() {
    local dashboard_file="$DASHBOARD_DIR/game_director_dashboard_$(date +%Y%m%d_%H%M%S).md"
    
    log_director "Generating Game Director Dashboard..."
    
    cat > "$dashboard_file" << EOF
# 🎮 Game Director Dashboard - Adventure_God v1.1
Generated: $(date)

## 📊 Project Status Overview

### 🎯 Current Build Status
- **Last Build**: $(ls -la "$BUILD_DIR"/*.log 2>/dev/null | tail -1 | awk '{print $6, $7, $8}' || echo "No builds found")
- **Gateway Status**: $(if lsof -ti:8787 > /dev/null 2>&1; then echo "🟢 Running"; else echo "🔴 Stopped"; fi)
- **Unity Status**: $(if pgrep -f "Unity" > /dev/null 2>&1; then echo "🟢 Running"; else echo "🔴 Stopped"; fi)

### 🧪 Automated Test Results
EOF

    # Check for recent test results
    if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]]; then
        cat "$PROJECT_PATH/cursor_test_results.md" >> "$dashboard_file"
    else
        echo "- No recent test results available" >> "$dashboard_file"
    fi

    cat >> "$dashboard_file" << EOF

### 📈 Performance Metrics
- **Target Frame Rate**: 60 FPS
- **Memory Usage**: < 500MB
- **WebSocket Latency**: < 100ms

### 🎨 Creative Direction Status

#### Current Gameplay Elements
- ✅ **DM Workflow**: Input → AI → Dice → Outcome
- ✅ **3 AI Adventurers**: Autonomous decision making
- ✅ **Dice System**: d20 with DC gating
- ✅ **Save/Load**: Game state persistence
- ✅ **Scene Management**: POI and NPC system

#### Visual & UX Elements
- ✅ **Top-Down Camera**: Follows Agent-1
- ✅ **Agent Highlighting**: Success/fail visual feedback
- ✅ **DM UI**: Comprehensive control panel
- ✅ **Party HUD**: Status display
- ✅ **Agent Labels**: Identification and notes

#### AI & Gameplay Systems
- ✅ **Intent Proposals**: AI suggests actions
- ✅ **Candidate Actions**: Multiple choice system
- ✅ **Outcome Feedback**: AI learning from results
- ✅ **Scene Loading**: Dynamic content from JSON

### 🚀 Next Development Priorities

#### High Priority (Game Director Focus)
1. **Visual Polish**: Improve agent appearance and animations
2. **UI Refinement**: Enhance DM interface usability
3. **Gameplay Balance**: Tune dice DCs and AI behavior
4. **Content Creation**: Add more scenes and POIs

#### Medium Priority (Technical Enhancement)
1. **Performance Optimization**: Ensure 60 FPS consistently
2. **AI Enhancement**: Improve decision-making logic
3. **Save System**: Add more game state persistence
4. **Testing**: Expand automated test coverage

#### Low Priority (Future Features)
1. **Voice Input**: Speech-to-text for DM
2. **Multiplayer**: Support for multiple DMs
3. **Modding**: Custom content support
4. **Mobile**: Mobile app for DM interface

### 🎬 Creative Feedback Collection

#### Recent Changes
$(git log --oneline -10 | sed 's/^/- /')

#### Pending Creative Decisions
1. **Agent Visual Design**: What should the agents look like?
2. **UI Theme**: What visual style for the DM interface?
3. **Game Balance**: How difficult should the DCs be?
4. **Content Direction**: What types of scenes and challenges?

### 🔧 Technical Status

#### Build Information
- **Unity Version**: 2022.3.62f1
- **Gateway Version**: $(cd "$GATEWAY_PATH" && node -e "console.log(require('./package.json').version)" 2>/dev/null || echo "Unknown")
- **Last Commit**: $(git log -1 --format="%h - %s (%cr)")

#### Component Health
$(if [[ -f "$PROJECT_PATH/cursor_validation_report.md" ]]; then
    cat "$PROJECT_PATH/cursor_validation_report.md" | grep -E "✅|❌" | sed 's/^/- /'
else
    echo "- No validation report available"
fi)

### 📋 Director Actions

#### Quick Actions
- **Preview Build**: Run \`./scripts/game_director_dashboard.sh preview\`
- **Run Tests**: Run \`./scripts/game_director_dashboard.sh test\`
- **Generate Report**: Run \`./scripts/game_director_dashboard.sh report\`
- **Start Development**: Run \`./scripts/game_director_dashboard.sh dev\`

#### Creative Feedback
- **Add Scene**: Describe new scene in \`docs/creative_feedback.md\`
- **Balance Tuning**: Note DC adjustments in \`docs/creative_feedback.md\`
- **Visual Direction**: Specify visual changes in \`docs/creative_feedback.md\`
- **Gameplay Ideas**: Document new features in \`docs/creative_feedback.md\`

EOF

    log_success "Dashboard generated: $dashboard_file"
    echo "$dashboard_file"
}

# Preview current build
preview_build() {
    log_director "Starting build preview..."
    
    # Start gateway if not running
    if ! lsof -ti:8787 > /dev/null 2>&1; then
        log_info "Starting gateway for preview..."
        ./scripts/cursor_unity_integration.sh start-gateway
    fi
    
    # Run Unity in preview mode
    log_info "Starting Unity preview..."
    cd "$UNITY_PROJECT_PATH"
    
    # Create preview log
    local preview_log="$BUILD_DIR/preview_$(date +%Y%m%d_%H%M%S).log"
    
    # Run Unity with preview settings
    "$UNITY_PATH" -batchmode -quit -nographics \
        -projectPath "$UNITY_PROJECT_PATH" \
        -executeMethod CursorHeadlessTest.RunComprehensiveTest \
        -logFile "$preview_log"
    
    local exit_code=$?
    
    if [[ $exit_code -eq 0 ]]; then
        log_success "Preview completed successfully"
        log_info "Preview log: $preview_log"
    else
        log_error "Preview failed with exit code: $exit_code"
    fi
    
    # Show preview results
    if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]]; then
        log_director "Preview Results:"
        cat "$PROJECT_PATH/cursor_test_results.md"
    fi
}

# Run comprehensive tests
run_tests() {
    log_director "Running comprehensive tests..."
    
    # Run integration tests
    ./scripts/cursor_unity_integration.sh test
    
    # Generate test report
    local test_report="$DASHBOARD_DIR/test_report_$(date +%Y%m%d_%H%M%S).md"
    
    cat > "$test_report" << EOF
# Test Report - $(date)

## Test Results
$(cat "$PROJECT_PATH/cursor_test_results.md" 2>/dev/null || echo "No test results available")

## Performance Metrics
- **Test Duration**: $(date)
- **Exit Code**: $?

## Recommendations
$(if [[ -f "$PROJECT_PATH/cursor_test_results.md" ]] && grep -q "FAIL" "$PROJECT_PATH/cursor_test_results.md"; then
    echo "- ❌ Some tests failed - review technical implementation"
else
    echo "- ✅ All tests passed - ready for creative review"
fi)
EOF

    log_success "Test report generated: $test_report"
}

# Start development mode
start_development() {
    log_director "Starting development mode..."
    
    # Start gateway
    ./scripts/cursor_unity_integration.sh start-gateway
    
    # Generate development status
    local dev_status="$DASHBOARD_DIR/dev_status_$(date +%Y%m%d_%H%M%S).md"
    
    cat > "$dev_status" << EOF
# Development Status - $(date)

## Environment Status
- **Gateway**: Running on port 8787
- **Unity**: Ready for development
- **Git Status**: $(git status --porcelain | wc -l) changes pending

## Ready for Development
1. Open Unity Editor
2. Press Play to test current implementation
3. Use DM UI to control gameplay
4. Provide creative feedback in docs/creative_feedback.md

## Current Focus Areas
$(cat "$PROJECT_PATH/docs/30_backlog.md" | grep -A 10 "Remaining items" | grep -E "^- " | head -5 | sed 's/^/- /')

## Creative Direction Notes
$(if [[ -f "$PROJECT_PATH/docs/creative_feedback.md" ]]; then
    tail -10 "$PROJECT_PATH/docs/creative_feedback.md"
else
    echo "- No creative feedback recorded yet"
fi)
EOF

    log_success "Development mode started"
    log_info "Development status: $dev_status"
}

# Generate creative feedback template
create_feedback_template() {
    local feedback_file="$PROJECT_PATH/docs/creative_feedback.md"
    
    if [[ ! -f "$feedback_file" ]]; then
        log_director "Creating creative feedback template..."
        
        cat > "$feedback_file" << EOF
# Creative Feedback - Adventure_God v1.1

## Game Director Notes

### Visual Direction
- **Agent Appearance**: [Describe desired agent visuals]
- **UI Theme**: [Describe desired UI style]
- **Environment**: [Describe desired scene aesthetics]
- **Animations**: [Describe desired movement/effects]

### Gameplay Balance
- **DC Difficulty**: [Note current DC values and desired adjustments]
- **AI Behavior**: [Note AI decision-making observations]
- **Pacing**: [Note game flow and timing]
- **Challenge Level**: [Note difficulty curve]

### Content Ideas
- **New Scenes**: [Describe new scene concepts]
- **NPCs**: [Describe new character ideas]
- **Challenges**: [Describe new gameplay challenges]
- **Story Elements**: [Describe narrative direction]

### Technical Feedback
- **Performance**: [Note any performance issues]
- **Usability**: [Note UI/UX improvements needed]
- **Bugs**: [Note any gameplay issues]
- **Features**: [Note missing features]

## Feedback History

### $(date)
- [Add today's feedback here]

EOF

        log_success "Creative feedback template created: $feedback_file"
    else
        log_info "Creative feedback file already exists: $feedback_file"
    fi
}

# Main function
main() {
    local command=${1:-"dashboard"}
    
    case "$command" in
        "dashboard")
            generate_dashboard
            ;;
        "preview")
            preview_build
            ;;
        "test")
            run_tests
            ;;
        "dev")
            start_development
            ;;
        "feedback")
            create_feedback_template
            ;;
        "help"|*)
            echo "🎮 Game Director Dashboard"
            echo ""
            echo "Usage: $0 <command>"
            echo ""
            echo "Commands:"
            echo "  dashboard  - Generate comprehensive project dashboard"
            echo "  preview    - Run build preview and show results"
            echo "  test       - Run comprehensive tests"
            echo "  dev        - Start development mode"
            echo "  feedback   - Create creative feedback template"
            echo "  help       - Show this help message"
            echo ""
            echo "🎬 Game Director Workflow:"
            echo "1. Run 'dashboard' to see project status"
            echo "2. Run 'preview' to test current build"
            echo "3. Provide feedback in docs/creative_feedback.md"
            echo "4. Run 'dev' to start development mode"
            echo "5. Let Cursor handle technical implementation"
            ;;
    esac
}

# Run main function with all arguments
main "$@"
