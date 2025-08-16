### Working Rules for Cursor in Adventure_God v1.1

**Always do this first**

1) Read `cursor-rules.md` then skim `docs/20_contracts.md` for any recent changes.
2) Restate your plan in 2-4 bullets before making edits.
3) Make a small, single-logical edit; include manual test steps in your message.

**Non‑negotiable guardrails**

- Do not change public schemas without updating both ends and `docs/20_contracts.md` in the same commit.
- Keep diffs small; one logical change per PR/commit; include manual test steps.
- No secrets; no heavy packages without justification & docs.

**Git & Unity Workflow (CRITICAL)**

- **Always maintain comprehensive .gitignore for Unity projects**
- **Never commit Unity Library/, Temp/, Logs/, UserSettings/ folders**
- **Before pushing**: run `git status --porcelain | wc -l` to check change count
- **Commit frequency**: after each logical feature completion, not every small change
- **If change count > 1000**: investigate and clean up Unity-generated files
- **Use .gitattributes for large binary files if needed**

**Environment Management (CRITICAL)**

- **Always check for existing processes**: `lsof -ti:8787` before starting gateway
- **Kill conflicting processes**: `lsof -ti:8787 | xargs -r kill -9`
- **Use nohup for long-running processes**: `nohup npx ts-node src/index.ts > /tmp/gateway.log 2>&1 &`
- **Document background processes in session notes**
- **Check Unity compilation errors in console before proceeding**

**Error Recovery Procedures**

- **Unity compilation errors**: check console, fix syntax, restart Unity
- **Gateway errors**: check TypeScript compilation, validate schemas, restart gateway
- **WebSocket issues**: verify gateway is running, check firewall, restart both ends
- **State corruption**: use save/load system, restart Unity if needed
- **Port conflicts**: kill existing processes, restart gateway

**Gateway requirements**

- TypeScript strictness; Zod v4 validation at all boundaries.
- Provide `npm run check:contracts`, `npm run lint`, and `npm run fix`.
- Prefer explicit, typed DTOs; avoid `any`.

**Unity requirements**

- Use Newtonsoft.Json for DTOs.
- No reflection hacks; keep gameplay logic modular and testable.

**Architecture expectations**

- `gateway/` exposes WS on `ws://127.0.0.1:8787`. Receives `PerceptionEvent`, returns `IntentProposal`.
- `unity/` connects via `BrainClient`, runs planner/executor, applies dice/DC gates, and updates state.

**Operating ritual (every session)**

1) Read rules → contracts → backlog.
2) Restate plan → confirm the one edit you'll make.
3) Make the small diff → show manual test.
4) If contracts changed, ensure `docs/20_contracts.md` is updated and run `npm run check:contracts` in `gateway/`.

**Ongoing usage (keep it tight)**

- Keep `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md` pinned in every new chat.
- Ask for one small change at a time; include a manual test plan in the message.
- If proposing contract changes: update both Gateway and Unity, update `docs/20_contracts.md` in the same commit, and call it out in the change summary.

- New feature area or major refactor:
  - Start a new chat to reduce context noise and stale assumptions.
  - Pin `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md`.
  - Restate the plan and explicitly reference applicable rules/contracts.
  - Re-run the session ritual and re-bootstrap the terminal (cwd, background jobs) as needed.


