### Cursor quick rules (pin me)

**Essential Session Ritual (ALWAYS DO FIRST)**
1. Read `docs/00_project_charter.md` for vision & scope
2. Read `docs/20_contracts.md` for technical contracts  
3. Read `docs/30_backlog.md` for current progress
4. Restate your plan in 2-4 bullets before making edits

**Core Workflow Rules**
- Keep diffs small; one logical change per commit; include manual test steps
- Contracts are source of truth; update `docs/20_contracts.md` + run `npm run check:contracts` if changing schemas
- No secrets; no heavy packages without justification & docs
- Git workflow: Check change count with `git status --porcelain | wc -l` before pushing

**Environment Management (CRITICAL)**
- Kill port conflicts: `lsof -ti:8787 | xargs -r kill -9` before starting gateway
- Check Unity compilation errors in console before proceeding
- Use nohup for gateway: `nohup npx ts-node src/index.ts > /tmp/gateway.log 2>&1 &`

**Error Recovery Procedures**
- Unity compilation errors: check console, fix syntax, restart Unity
- Gateway errors: check TypeScript compilation, validate schemas, restart gateway
- WebSocket issues: verify gateway is running, check firewall, restart both ends
- State corruption: use save/load system, restart Unity if needed
- Port conflicts: kill existing processes, restart gateway

**Architecture (Reference Only)**
- `gateway/` (Node + TypeScript + Zod) exposes `ws://127.0.0.1:8787`
- `unity/` (URP) renders, simulates, executes actions via `BrainClient`
- Two contracts: `PerceptionEvent` (Unity→Gateway), `IntentProposal` (Gateway→Unity)

**Quick Manual Test**
1. Start gateway: `cd gateway && npm run dev`
2. Send test: `npx ts-node scripts/send_demo.ts`  
3. Unity Play: Open Unity, press Play, test DM workflow
4. Verify: Dice rolls, candidate selection, save/load work

**Reference Documents**
- **Vision & Scope**: `docs/00_project_charter.md`
- **Technical Contracts**: `docs/20_contracts.md`
- **Current Progress**: `docs/30_backlog.md`
- **Testing Strategy**: `docs/40_testing_strategy.md`
- **Performance Guidelines**: `docs/50_performance_guidelines.md`
- **Documentation Strategy**: `docs/60_documentation_strategy.md`
- **Feature Development**: `docs/70_feature_development.md`

**Operating ritual each session**: read rules → restate plan → make small diff → show manual test.

**Ongoing usage (keep it tight)**:
- Keep `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md` pinned in every new chat
- Ask for one small change at a time; include a manual test plan in the message
- If proposing contract changes: update both Gateway and Unity, update `docs/20_contracts.md` in the same commit, and call it out in the change summary


