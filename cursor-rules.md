### Cursor quick rules (pin me)

- Always read these before edits: `docs/10_working_rules_for_cursor.md`, `docs/20_contracts.md`, `docs/30_backlog.md`.
- Contracts are the source of truth. If you change `gateway/src/schema/**` or Unity DTOs, you must update `docs/20_contracts.md` in the same commit and run `npm run check:contracts`.
- Keep diffs small; one logical change per commit; include manual test steps.
- Gateway: strict TS + Zod at boundaries; provide `lint`/`fix` scripts.
- Unity: DTOs via Newtonsoft.Json; no reflection hacks; modular gameplay logic.

Operating ritual each session: read rules → restate plan → make small diff → show manual test.

Ongoing usage (keep it tight):
- Keep `cursor-rules.md`, `docs/10_working_rules_for_cursor.md`, and `docs/20_contracts.md` pinned in every new chat.
- Ask for one small change at a time; include a manual test plan in the message.
- If proposing contract changes: update both Gateway and Unity, update `docs/20_contracts.md` in the same commit, and call it out in the change summary.


