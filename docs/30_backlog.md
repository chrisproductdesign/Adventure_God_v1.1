### Near-term backlog (v1.1)

- Gateway: Intent selector seam (done). Demo sender (done). Lint/Prettier (done).
- Gateway: keep Zod boundary; ensure `move` param specialization (done).
- Unity: typed DTO parsing + `ActionExecutor` (done).
- Unity: Dice/DC gate with minimal UI (done).
- Save/Load: minimal JSON snapshot of scene state and history (todo).

### Manual test steps (current MVP loop)

1. Start Gateway
   - In `gateway/`: `npx ts-node src/index.ts`
   - Expect: "AI Gateway listening on ws://127.0.0.1:8787"
2. Send demo events
   - In `gateway/`: `npx ts-node scripts/send_demo.ts`
   - Expect: move intents except when an enemy <= 2m (idle/wait)
3. Unity Play
   - Open `unity/` in Editor and press Play. Or run headless with `HeadlessHarness.Run`.
   - Expect: Dice gate logs; on success, `ActionExecutor` moves agent by {0,2}.
4. Contract guard
   - If editing files under `gateway/src/schema/`, update `docs/20_contracts.md` in the same commit and run `npm run check:contracts`.


