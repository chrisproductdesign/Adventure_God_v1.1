#!/bin/bash

# Feedback Processor for Adventure_God v1.1
# Processes feedback from various sources and updates creative_feedback.md

set -euo pipefail

# Configuration
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
FEEDBACK_FILE="$PROJECT_PATH/docs/creative_feedback.md"
FEEDBACK_LOG="$PROJECT_PATH/feedback_log.txt"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_director() {
    echo -e "${PURPLE}[DIRECTOR]${NC} $1"
}

# Add feedback from chat
add_chat_feedback() {
    local feedback="$1"
    local category="${2:-general}"
    local priority="${3:-medium}"
    
    log_director "Processing chat feedback..."
    
    # Create timestamp
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    
    # Add to feedback log
    echo "[$timestamp] CHAT FEEDBACK - $category - $priority: $feedback" >> "$FEEDBACK_LOG"
    
    # Update creative feedback document
    update_creative_feedback "$feedback" "$category" "$priority" "$timestamp"
    
    log_success "Feedback added to creative_feedback.md"
}

# Update creative feedback document
update_creative_feedback() {
    local feedback="$1"
    local category="$2"
    local priority="$3"
    local timestamp="$4"
    
    # Create backup
    cp "$FEEDBACK_FILE" "$FEEDBACK_FILE.backup"
    
    # Add to appropriate section
    case "$category" in
        "visual")
            add_to_section "Visual Direction" "$feedback" "$priority"
            ;;
        "gameplay")
            add_to_section "Gameplay Balance" "$feedback" "$priority"
            ;;
        "content")
            add_to_section "Content Ideas" "$feedback" "$priority"
            ;;
        "technical")
            add_to_section "Technical Feedback" "$feedback" "$priority"
            ;;
        "ui")
            add_to_section "UI/UX Improvements" "$feedback" "$priority"
            ;;
        *)
            add_to_section "General Feedback" "$feedback" "$priority"
            ;;
    esac
    
    # Add to feedback history
    add_to_history "$feedback" "$timestamp"
}

# Add feedback to specific section
add_to_section() {
    local section="$1"
    local feedback="$2"
    local priority="$3"
    
    # Create temporary file
    local temp_file=$(mktemp)
    
    # Process the file line by line
    local in_section=false
    local section_found=false
    
    while IFS= read -r line; do
        echo "$line" >> "$temp_file"
        
        # Check if we're entering the target section
        if [[ "$line" == "### $section" ]]; then
            in_section=true
            section_found=true
        fi
        
        # If we're in the section and hit the next section, add our feedback
        if $in_section && [[ "$line" == "### "* ]] && [[ "$line" != "### $section" ]]; then
            echo "- **[$priority] $feedback**" >> "$temp_file"
            in_section=false
        fi
        
        # If we're in the section and hit the end of the file, add our feedback
        if $in_section && [[ -z "$line" ]]; then
            echo "- **[$priority] $feedback**" >> "$temp_file"
            in_section=false
        fi
    done < "$FEEDBACK_FILE"
    
    # If section wasn't found, add it
    if ! $section_found; then
        echo "" >> "$temp_file"
        echo "### $section" >> "$temp_file"
        echo "- **[$priority] $feedback**" >> "$temp_file"
    fi
    
    # Replace original file
    mv "$temp_file" "$FEEDBACK_FILE"
}

# Add to feedback history
add_to_history() {
    local feedback="$1"
    local timestamp="$2"
    
    # Find the feedback history section and add entry
    local temp_file=$(mktemp)
    local in_history=false
    local added=false
    
    while IFS= read -r line; do
        echo "$line" >> "$temp_file"
        
        # Check if we're entering feedback history
        if [[ "$line" == "## Feedback History" ]]; then
            in_history=true
        fi
        
        # If we're in history and hit the next major section, add our entry
        if $in_history && [[ "$line" == "# "* ]] && ! $added; then
            echo "" >> "$temp_file"
            echo "### $timestamp" >> "$temp_file"
            echo "- $feedback" >> "$temp_file"
            added=true
        fi
        
        # If we're in history and hit the end, add our entry
        if $in_history && [[ -z "$line" ]] && ! $added; then
            echo "" >> "$temp_file"
            echo "### $timestamp" >> "$temp_file"
            echo "- $feedback" >> "$temp_file"
            added=true
        fi
    done < "$FEEDBACK_FILE"
    
    # If history section wasn't found, add it
    if ! $in_history; then
        echo "" >> "$temp_file"
        echo "## Feedback History" >> "$temp_file"
        echo "" >> "$temp_file"
        echo "### $timestamp" >> "$temp_file"
        echo "- $feedback" >> "$temp_file"
    fi
    
    # Replace original file
    mv "$temp_file" "$FEEDBACK_FILE"
}

# Mark feedback as completed
mark_completed() {
    local feedback_text="$1"
    
    log_director "Marking feedback as completed..."
    
    # Create temporary file
    local temp_file=$(mktemp)
    
    while IFS= read -r line; do
        # Replace "[ ]" with "[x]" for matching feedback
        if [[ "$line" == *"$feedback_text"* ]] && [[ "$line" == *"- [ ]"* ]]; then
            echo "$line" | sed 's/- \[ \]/- [x]/' >> "$temp_file"
        else
            echo "$line" >> "$temp_file"
        fi
    done < "$FEEDBACK_FILE"
    
    # Replace original file
    mv "$temp_file" "$FEEDBACK_FILE"
    
    log_success "Feedback marked as completed"
}

# Show current feedback status
show_status() {
    log_director "Current Feedback Status:"
    echo ""
    
    if [[ -f "$FEEDBACK_FILE" ]]; then
        # Count pending items
        local pending=$(grep -c "- \[ \]" "$FEEDBACK_FILE" 2>/dev/null || echo "0")
        local completed=$(grep -c "- \[x\]" "$FEEDBACK_FILE" 2>/dev/null || echo "0")
        
        echo "📊 Feedback Summary:"
        echo "- Pending: $pending items"
        echo "- Completed: $completed items"
        echo ""
        
        echo "🎯 Recent Feedback:"
        tail -10 "$FEEDBACK_LOG" 2>/dev/null || echo "No feedback log available"
        echo ""
        
        echo "📝 Pending Items:"
        grep "- \[ \]" "$FEEDBACK_FILE" 2>/dev/null | head -5 | sed 's/^- \[ \] //' || echo "No pending items"
    else
        echo "No creative feedback file found"
    fi
}

# Process feedback from file
process_file_feedback() {
    local input_file="$1"
    
    if [[ ! -f "$input_file" ]]; then
        log_error "Input file not found: $input_file"
        return 1
    fi
    
    log_director "Processing feedback from file: $input_file"
    
    while IFS= read -r line; do
        if [[ -n "$line" ]]; then
            add_chat_feedback "$line" "general" "medium"
        fi
    done < "$input_file"
    
    log_success "File feedback processed"
}

# Main function
main() {
    local command=${1:-"help"}
    
    case "$command" in
        "add")
            local feedback="${2:-}"
            local category="${3:-general}"
            local priority="${4:-medium}"
            
            if [[ -z "$feedback" ]]; then
                log_error "No feedback provided"
                return 1
            fi
            
            add_chat_feedback "$feedback" "$category" "$priority"
            ;;
        "complete")
            local feedback="${2:-}"
            
            if [[ -z "$feedback" ]]; then
                log_error "No feedback text provided"
                return 1
            fi
            
            mark_completed "$feedback"
            ;;
        "status")
            show_status
            ;;
        "process")
            local file="${2:-}"
            
            if [[ -z "$file" ]]; then
                log_error "No file specified"
                return 1
            fi
            
            process_file_feedback "$file"
            ;;
        "help"|*)
            echo "🎬 Feedback Processor"
            echo ""
            echo "Usage: $0 <command> [options]"
            echo ""
            echo "Commands:"
            echo "  add <feedback> [category] [priority]  - Add feedback from chat"
            echo "  complete <feedback_text>              - Mark feedback as completed"
            echo "  status                                - Show current feedback status"
            echo "  process <file>                        - Process feedback from file"
            echo "  help                                  - Show this help message"
            echo ""
            echo "Categories: visual, gameplay, content, technical, ui, general"
            echo "Priorities: high, medium, low"
            echo ""
            echo "Examples:"
            echo "  $0 add \"Agents need better visuals\" visual high"
            echo "  $0 add \"UI feels cluttered\" ui medium"
            echo "  $0 complete \"Agents need better visuals\""
            echo "  $0 status"
            ;;
    esac
}

# Run main function with all arguments
main "$@"
