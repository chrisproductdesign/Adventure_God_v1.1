import { PerceptionEventT } from "../schema/events";

export function selectIntent(evt: PerceptionEventT) {
  const hasCloseEnemy = (evt.observations ?? []).some(
    (o) => o.kind === "enemy" && typeof o.distance === "number" && o.distance <= 2
  );

  if (hasCloseEnemy) {
    return {
      type: "IntentProposal" as const,
      actorId: evt.actorId,
      goal: "stay-safe",
      intent: "wait",
      rationale: "Enemy nearby, holding position",
      suggestedDC: 12,
      candidateActions: [{ action: "idle", params: {} }]
    };
  }

  return {
    type: "IntentProposal" as const,
    actorId: evt.actorId,
    goal: "advance",
    intent: "move",
    rationale: "Advance cautiously",
    suggestedDC: 10,
    candidateActions: [{ action: "move", params: { destDX: 0, destDZ: 2 } }]
  };
}


