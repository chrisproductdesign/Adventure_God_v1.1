## Adventure_God v1.1

Top-down D&D-style game. Unity (URP) client with DM UI and three AI adventurers; Node/TS gateway provides intents over WebSocket with Zod-validated contracts.

### Quick start

Gateway
- Node 22.x recommended
- Install deps: `cd gateway && npm i`
- Run server: `npx ts-node src/index.ts`
- Demo sender: `npx ts-node scripts/send_demo.ts`

Unity
- Editor: 2022.3.62f1 (Api Compatibility = .NET Framework)
- Open project at `unity/`, press Play
- Or headless: `"/Applications/Unity/Hub/Editor/2022.3.62f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/chris/Ai/Games/Adventure_God_v1.1/unity" -batchmode -nographics -quit -executeMethod HeadlessHarness.Run`

Save/Load
- In Play, use the on-screen Save/Load buttons (runtime UI) to write/read `save.json` in `Application.persistentDataPath`.

### Contracts
See `docs/20_contracts.md` for `PerceptionEvent` and `IntentProposal`. Changes must update both ends and the docs in the same commit.

### Dev workflow
- See `docs/10_working_rules_for_cursor.md` for session ritual and rules.
- Keep diffs small with a manual test in each PR.


