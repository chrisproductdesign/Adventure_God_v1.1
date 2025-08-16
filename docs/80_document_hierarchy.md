### Document Hierarchy & Usage Guide for Adventure_God v1.1

This document explains the hierarchy and usage of all documentation files in the project, helping you understand what to read when and why.

## Document Hierarchy (Read Order)

### **Level 1: Essential Workflow (Always Read First)**
These documents contain the essential information needed for every development session.

#### **1. `.cursor/rules/00-project-overview.mdc` (Cursor Rules)**
- **Purpose**: Essential workflow rules for every session
- **Content**: Session ritual, core workflow rules, environment management
- **When to read**: Every time you start working on the project
- **Size**: ~40 lines (concise and actionable)
- **Status**: Always applied by Cursor

#### **2. `docs/00_project_charter.md` (Project Charter)**
- **Purpose**: Vision, scope, and architecture overview
- **Content**: Project vision, MVP scope, architecture, success criteria
- **When to read**: When starting work, when making architectural decisions
- **Size**: ~45 lines (high-level overview)
- **Status**: Reference document

#### **3. `docs/20_contracts.md` (Technical Contracts)**
- **Purpose**: Source of truth for Unity ↔ Gateway communication
- **Content**: JSON schemas, validation rules, contract enforcement
- **When to read**: When working with WebSocket communication, when changing schemas
- **Size**: ~62 lines (technical specifications)
- **Status**: Must be updated when contracts change

#### **4. `docs/30_backlog.md` (Current Progress)**
- **Purpose**: Track completed work and remaining tasks
- **Content**: Completed items, remaining work, manual test steps, quality checklist
- **When to read**: When planning work, when checking project status
- **Size**: ~71 lines (progress tracking)
- **Status**: Updated with each completed feature

### **Level 2: Workflow Support (Reference as Needed)**
These documents support the development workflow and should be referenced when working on specific areas.

#### **5. `docs/10_working_rules_for_cursor.md` (Working Rules)**
- **Purpose**: Detailed workflow procedures and requirements
- **Content**: Error recovery, gateway/Unity requirements, operating ritual
- **When to read**: When troubleshooting, when setting up environment
- **Size**: ~76 lines (detailed procedures)
- **Status**: Reference document

#### **6. `cursor-rules.md` (Main Cursor Rules)**
- **Purpose**: Quick reference for cursor rules
- **Content**: Essential workflow, reference documents, quick manual test
- **When to read**: When setting up new chat, when needing quick reference
- **Size**: ~50 lines (quick reference)
- **Status**: Pinned in every chat

### **Level 3: Strategy Documents (Reference for Planning)**
These documents contain comprehensive strategies and should be referenced when planning major work.

#### **7. `docs/40_testing_strategy.md` (Testing Strategy)**
- **Purpose**: Comprehensive testing approach
- **Content**: Testing pyramid, automated testing, manual procedures
- **When to read**: When implementing tests, when setting up CI/CD
- **Size**: ~203 lines (comprehensive strategy)
- **Status**: Reference for testing decisions

#### **8. `docs/50_performance_guidelines.md` (Performance Guidelines)**
- **Purpose**: Performance targets and optimization strategies
- **Content**: Frame rate targets, memory management, optimization techniques
- **When to read**: When optimizing performance, when debugging performance issues
- **Size**: ~217 lines (performance strategy)
- **Status**: Reference for performance decisions

#### **9. `docs/60_documentation_strategy.md` (Documentation Strategy)**
- **Purpose**: Documentation maintenance and quality standards
- **Content**: Documentation principles, maintenance procedures, quality checklist
- **When to read**: When updating documentation, when setting up documentation tools
- **Size**: ~223 lines (documentation strategy)
- **Status**: Reference for documentation decisions

#### **10. `docs/70_feature_development.md` (Feature Development)**
- **Purpose**: Feature development workflow and procedures
- **Content**: Feature lifecycle, branching strategy, backward compatibility
- **When to read**: When planning new features, when implementing feature flags
- **Size**: ~299 lines (feature development strategy)
- **Status**: Reference for feature development decisions

## Cursor Rules Integration

### **Cursor Rules Files (`.cursor/rules/`)**
These files are automatically applied by Cursor and provide quick reference information.

#### **`.cursor/rules/00-project-overview.mdc`**
- **Status**: Always applied
- **Content**: Essential workflow rules
- **Usage**: Automatic application by Cursor

#### **`.cursor/rules/01-project-charter.mdc`**
- **Status**: Available for reference
- **Content**: Project vision and scope
- **Usage**: Reference when making architectural decisions

#### **`.cursor/rules/02-contracts.mdc`**
- **Status**: Available for reference
- **Content**: Technical contracts and schemas
- **Usage**: Reference when working with WebSocket communication

#### **`.cursor/rules/03-backlog.mdc`**
- **Status**: Available for reference
- **Content**: Current progress and remaining work
- **Usage**: Reference when planning work

## Usage Patterns

### **New Development Session**
1. Read `.cursor/rules/00-project-overview.mdc` (automatic)
2. Read `docs/00_project_charter.md` for context
3. Read `docs/30_backlog.md` for current status
4. Read `docs/20_contracts.md` if working with communication

### **Troubleshooting Session**
1. Read `.cursor/rules/00-project-overview.mdc` (automatic)
2. Read `docs/10_working_rules_for_cursor.md` for error recovery
3. Read relevant strategy document based on issue type

### **Feature Planning Session**
1. Read `.cursor/rules/00-project-overview.mdc` (automatic)
2. Read `docs/00_project_charter.md` for scope
3. Read `docs/70_feature_development.md` for workflow
4. Read `docs/30_backlog.md` for current priorities

### **Performance Optimization Session**
1. Read `.cursor/rules/00-project-overview.mdc` (automatic)
2. Read `docs/50_performance_guidelines.md` for targets
3. Read `docs/40_testing_strategy.md` for testing approach

## Document Maintenance

### **Update Frequency**
- **Cursor Rules**: Update when workflow changes
- **Project Charter**: Update when vision/scope changes
- **Contracts**: Update when schemas change
- **Backlog**: Update with each completed feature
- **Strategy Documents**: Update when strategies change

### **Quality Assurance**
- **Accuracy**: All documents must reflect current state
- **Consistency**: Cross-references must be accurate
- **Completeness**: All public APIs must be documented
- **Clarity**: Language must be clear and actionable

## Future Enhancements

### **Planned Improvements**
- **Interactive Documentation**: Clickable diagrams and examples
- **Search Functionality**: Full-text search across all documents
- **Version Control**: Track documentation changes over time
- **Automated Validation**: Ensure documentation accuracy
- **Integration**: Better integration with development tools

### **Documentation Metrics**
- **Coverage**: Percentage of code covered by documentation
- **Freshness**: Time since last documentation update
- **Usage**: How often documentation is accessed
- **Feedback**: User satisfaction with documentation
- **Maintenance**: Time spent on documentation updates
