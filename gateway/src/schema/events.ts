import { z } from "zod";

export const ParamBag = z.record(z.string(), z.unknown()).default({});

export const PerceptionEvent = z.object({
  type: z.literal("PerceptionEvent"),
  actorId: z.string(),
  tick: z.number().optional(),
  observations: z
    .array(
      z.object({
        kind: z.enum(["enemy", "object", "trap", "ally", "info"]),
        id: z.string(),
        distance: z.number().optional()
      })
    )
    .optional(),
  world: z
    .object({
      poiId: z.string().optional(),
      time: z.string().optional(),
      threat: z.string().optional()
    })
    .optional()
});

export const IntentProposal = z.object({
  type: z.literal("IntentProposal"),
  actorId: z.string(),
  goal: z.string(),
  intent: z.string(),
  rationale: z.string().optional(),
  suggestedDC: z.number().optional(),
  candidateActions: z
    .array(
      z.object({
        action: z.string(),
        params: z.union([
          z
            .object({
              destDX: z.number(),
              destDZ: z.number()
            })
            .passthrough(),
          ParamBag
        ])
      })
    )
    .min(1)
});

// Enforce that when action === 'move', the params include numeric destDX/destDZ
export const IntentProposalStrict = IntentProposal.superRefine((proposal, ctx) => {
  for (let i = 0; i < proposal.candidateActions.length; i++) {
    const item = proposal.candidateActions[i] as unknown as { action: string; params: unknown };
    if (item.action === "move") {
      const MoveParams = z.object({ destDX: z.number(), destDZ: z.number() });
      const res = MoveParams.safeParse(item.params);
      if (!res.success) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: `candidateActions[${i}] action=move requires params { destDX:number, destDZ:number }`
        });
      }
    }
  }
});

export type PerceptionEventT = z.infer<typeof PerceptionEvent>;
export type IntentProposalT = z.infer<typeof IntentProposal>;
export type IntentProposalStrictT = z.infer<typeof IntentProposalStrict>;
