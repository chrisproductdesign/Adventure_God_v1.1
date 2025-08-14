import { PerceptionEventT } from "../schema/events";
import { suggestedDC } from "./rollDice";

export function selectIntent(evt: PerceptionEventT) {
  const hasCloseEnemy = (evt.observations ?? []).some(
    (o) => o.kind === "enemy" && typeof o.distance === "number" && o.distance <= 2
  );
  const hasDmContext = (evt.observations ?? []).some(
    (o) => o.kind === "info" && typeof o.id === "string" && o.id.startsWith("dm:")
  );

  if (hasCloseEnemy) {
    return {
      type: "IntentProposal" as const,
      actorId: evt.actorId,
      goal: "stay-safe",
      intent: "wait",
      rationale: "Enemy nearby, holding position",
      suggestedDC: suggestedDC(12),
      candidateActions: [{ action: "idle", params: {} }]
    };
  }

  // When DM context present, include two candidates to exercise the selector
  if (hasDmContext) {
    return {
      type: "IntentProposal" as const,
      actorId: evt.actorId,
      goal: "advance",
      intent: "move",
      rationale: "DM-guided",
      suggestedDC: suggestedDC(10),
      candidateActions: [
        { action: "move", params: { destDX: 0, destDZ: 5 } },
        { action: "idle", params: {} }
      ]
    };
  }

  return {
    type: "IntentProposal" as const,
    actorId: evt.actorId,
    goal: "advance",
    intent: "move",
    rationale: "Advance cautiously",
    suggestedDC: suggestedDC(10),
    candidateActions: [{ action: "move", params: { destDX: 0, destDZ: 5 } }]
  };
}


