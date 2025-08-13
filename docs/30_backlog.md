### Near-term backlog (v1.1)

- Gateway: tighten `IntentProposal.candidateActions` validation for `action:"move"` to require `{ destDX, destDZ }`.
- Gateway: add ESLint/Prettier and wire `npm run lint`/`fix`.
- Unity: add DTOs for `PerceptionEvent` and `IntentProposal` (Newtonsoft.Json), with `move` param shape.
- Unity: implement `BrainClient` message handling for the above DTOs.
- Save/Load: minimal JSON snapshot of scene state and history.

### Manual test steps (current MVP loop)

1. Start Gateway
   - In `gateway/`: `npm run dev`
   - Expect: "AI Gateway listening on ws://127.0.0.1:8787"
2. Send a `PerceptionEvent`
   - Connect via any WS client and send a minimal payload:
   ```json
   { "type": "PerceptionEvent", "actorId": "adv-1" }
   ```
   - Expect: a valid `IntentProposal` response.
3. Validate `move` specialization
   - If the Gateway emits `action:"move"`, ensure params include numeric `destDX` and `destDZ`.
4. Contract guard
   - If editing files under `gateway/src/schema/`, run `npm run check:contracts` and ensure it passes only when `docs/20_contracts.md` is updated in the same commit/stage.


