### Near-term backlog (v1.1)

- Gateway: Intent selector seam (done). Demo sender (done). Lint configured (done).
- Gateway: keep Zod boundary; ensure `move` param specialization (done).
- Unity: typed DTO parsing + `ActionExecutor` (done).
- Unity: Dice/DC gate with DM HUD (DC slider, roll, notes, DM input, candidate selector) (done).
- Unity: LocalIntentDemo for offline loop (done).
- Unity: Scene loader + POI registry (done).
- Unity: Party state + HUD (done).
- Unity: Save/Load party + DM notes (done).
- Unity: Remove NavMesh requirement; transform fallback movement for MVP (done).
- Docs: Contracts include suggestedDC (done).
- Git/GitHub: repo init and push (done).
- **NEW**: Git workflow optimization - comprehensive .gitignore and Unity Library management (done).
- **NEW**: Environment management - process management and port conflict resolution (done).
- **NEW**: Error recovery procedures - comprehensive troubleshooting guide (done).
- **NEW**: Testing strategy - comprehensive testing approach with unit, integration, performance, and UX tests (done).
- **NEW**: Performance guidelines - frame rate targets, memory management, optimization strategies (done).
- **NEW**: Documentation strategy - documentation maintenance and quality standards (done).
- **NEW**: Feature development workflow - branching strategy, feature flags, backward compatibility (done).
- **NEW**: Documentation optimization - cursor rules optimization, document hierarchy, information consolidation (done).

### Manual test steps (current MVP loop)

1. Start Gateway
   - In `gateway/`: `npx ts-node src/index.ts`
   - Expect: "AI Gateway listening on ws://127.0.0.1:8787"
2. Send demo events
   - In `gateway/`: `npx ts-node scripts/send_demo.ts`
   - Expect: move intents except when an enemy <= 2m (idle/wait)
3. Unity Play
   - Open `unity/` in Editor and press Play. Or run headless with `HeadlessHarness.Run`.
   - Expect: Dice gate logs; on success, `ActionExecutor` executes selected candidate (move/talk/inspect). Movement uses transform fallback.
   - DM: Type "adv-2 dm:multi" then Send→GW, select candidate via Cand ◄/► and Roll.
   - DM: Propose "adv-3 move 0 5", then Roll. Use Actor/POI selectors to nudge/snap.
4. Contract guard
   - If editing files under `gateway/src/schema/`, update `docs/20_contracts.md` in the same commit and run `npm run check:contracts`.

### Remaining items (next)

- **Performance monitoring implementation**: Add Unity Profiler integration and gateway performance monitoring.
- **Automated testing setup**: Implement Jest for gateway and Unity Test Runner for Unity.
- **Feature flag system**: Implement runtime feature flags for experimental features.
- **Documentation automation**: Set up automated documentation generation and validation.
- **CI/CD pipeline**: Set up automated testing and deployment pipeline.

### Documentation structure (optimized)

```
.cursor/rules/
├── 00-project-overview.mdc (essential workflow rules - 40 lines, always applied)
├── 01-project-charter.mdc (vision & scope - reference)
├── 02-contracts.mdc (technical contracts - reference)
└── 03-backlog.mdc (current progress - reference)

docs/
├── 00_project_charter.md (vision & scope - READ FIRST)
├── 10_working_rules_for_cursor.md (detailed workflow procedures - reference)
├── 20_contracts.md (source of truth for Unity ↔ Gateway contracts)
├── 30_backlog.md (project progress and remaining work)
├── 40_testing_strategy.md (comprehensive testing approach)
├── 50_performance_guidelines.md (performance targets and optimization)
├── 60_documentation_strategy.md (documentation maintenance and quality)
├── 70_feature_development.md (feature development workflow)
└── 80_document_hierarchy.md (NEW - document hierarchy and usage guide)

cursor-rules.md (quick reference for cursor rules - pinned in every chat)
```

### Quality assurance checklist

Before any release or major feature completion:
- [ ] **Git workflow**: Change count < 1000, no Unity Library files committed
- [ ] **Environment**: No port conflicts, all processes properly managed
- [ ] **Testing**: All unit, integration, and performance tests pass
- [ ] **Performance**: 60 FPS maintained, memory usage < 500MB
- [ ] **Documentation**: All changes documented, API docs updated
- [ ] **Contracts**: Schema validation passes, contracts documented
- [ ] **Error handling**: All error scenarios tested and handled
- [ ] **Documentation hierarchy**: All documents follow the established hierarchy and cross-references are accurate


