### Adventure_God v1.1 — Project Charter (Vision & Scope)

**Vision**

Build a DM-driven tactical adventure sandbox where a “Gateway” AI proposes intents for a 3‑adventurer party and a Unity client renders, simulates, and executes those intents. Outcomes are gated by d20 checks and DM-defined DCs. The experience should be deterministic, debuggable, and easy to extend.

**Scope (v1.1 MVP)**

- **Scene orchestration**: DM configures scenes, NPCs, POIs, and checks.
- **3 AI party members**: LLM-based intent proposal (stubbed first), planner/executor runs in Unity.
- **Contract-first**: Only two public JSON messages across the wire: `PerceptionEvent` in, `IntentProposal` out.
- **Deterministic loop**: Unity ticks, sends perceptions, Gateway replies with intent proposals; dice/DC gate outcomes; Unity updates state.
- **Persistence**: Save/load simple JSON snapshots of state and history.

**Architecture (Ground Truth)**

- `gateway/` (Node 22 + TypeScript + Zod v4)
  - Exposes WebSocket at `ws://127.0.0.1:8787`.
  - Receives `PerceptionEvent`, returns `IntentProposal`.
  - Zod validates inputs/outputs at boundaries.
  - Future: LLMs live here; planner stays in Unity.
- `unity/` (URP, Api Compatibility = .NET Framework)
  - Renders, simulates, and executes actions.
  - `BrainClient` connects to the Gateway.
  - Planner/executor handles intents and applies dice/DC gates.

**Contracts (Source of Truth summarized; full spec in docs/20_contracts.md)**

- `PerceptionEvent` and `IntentProposal` are enforced by Zod in `gateway/` and DTOs in `unity/`.
- For action `"move"`, params MUST be `{ destDX:number, destDZ:number }`.

**Non-goals (for v1.1)**

- No complex LLM tool-use; no cloud infra; no secrets.
- No heavy packages without justification.
- No reflection-based gameplay hacks.

**Success Criteria**

- End-to-end loop: Unity sends `PerceptionEvent` → Gateway replies `IntentProposal` → Unity executes → state saved/loaded.
- Contracts are stable, documented, and validated on both ends.
- Small, reviewable diffs with manual test steps.


