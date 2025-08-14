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
- Git/GitHub: repo init and push (todo).

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

- Initialize Git and push to GitHub (with manual steps documented).
- Small test for gateway intent selection.
- Optional: voice input (post-MVP), NavMesh feature flag (post-MVP).


