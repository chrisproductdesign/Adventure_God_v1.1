### Unity ↔ Gateway JSON Contracts (Source of Truth)

These contracts are authoritative. Any change requires updating this file and both implementations (Zod in `gateway/`, DTOs in `unity/`). A CI-style local check exists in `gateway/` as `npm run check:contracts`.

#### PerceptionEvent

```json
{
  "type": "PerceptionEvent",
  "actorId": "string",
  "tick": "number?",
  "observations": [
    {
      "kind": "enemy" | "object" | "trap" | "ally" | "info",
      "id": "string",
      "distance": "number?"
    }
  ]?,
  "world": {
    "poiId": "string?",
    "time": "string?",
    "threat": "string?"
  }?
}
```

#### IntentProposal

```json
{
  "type": "IntentProposal",
  "actorId": "string",
  "goal": "string",
  "intent": "string",
  "rationale": "string?",
  "suggestedDC": "number?",
  "candidateActions": [
    {
      "action": "string",
      "params": { "[k]": "unknown" }
    }
  ]
}
```

Specialization for `action: "move"`:

```json
{
  "action": "move",
  "params": { "destDX": "number", "destDZ": "number" }
}
```

Notes

- Enforced with Zod v4 in `gateway/src/schema/events.ts` and with Newtonsoft.Json DTOs in Unity.
- `candidateActions` must be a non-empty array.
- The Gateway must only emit schema-valid `IntentProposal` messages.


