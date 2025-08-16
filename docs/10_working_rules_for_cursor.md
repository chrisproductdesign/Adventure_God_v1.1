### Working Rules for Cursor in Adventure_God v1.1

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

**Gateway Requirements**
- TypeScript strictness; Zod v4 validation at all boundaries
- Provide `npm run check:contracts`, `npm run lint`, and `npm run fix`
- Prefer explicit, typed DTOs; avoid `any`

**Unity Requirements**
- Use Newtonsoft.Json for DTOs
- No reflection hacks; keep gameplay logic modular and testable

**Architecture Expectations**
- `gateway/` exposes WS on `ws://127.0.0.1:8787`. Receives `PerceptionEvent`, returns `IntentProposal`
- `unity/` connects via `BrainClient`, runs planner/executor, applies dice/DC gates, and updates state

**Operating Ritual (Every Session)**
1. Read rules → contracts → backlog
2. Restate plan → confirm the one edit you'll make
3. Make the small diff → show manual test
4. If contracts changed, ensure `docs/20_contracts.md` is updated and run `npm run check:contracts` in `gateway/`

**Ongoing Usage (Keep It Tight)**
- Keep `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md` pinned in every new chat
- Ask for one small change at a time; include a manual test plan in the message
- If proposing contract changes: update both Gateway and Unity, update `docs/20_contracts.md` in the same commit, and call it out in the change summary

**New Feature Area or Major Refactor**
- Start a new chat to reduce context noise and stale assumptions
- Pin `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md`
- Restate the plan and explicitly reference applicable rules/contracts
- Re-run the session ritual and re-bootstrap the terminal (cwd, background jobs) as needed


