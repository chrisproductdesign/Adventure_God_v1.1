using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class ActionExecutor : MonoBehaviour
{
    public void Execute(IntentProposal proposal, Transform agent)
    {
        if (proposal == null || proposal.candidateActions == null || proposal.candidateActions.Count == 0)
        {
            Debug.LogWarning("[Executor] Empty proposal or no actions");
            return;
        }

        var first = proposal.candidateActions[0];
        switch (first.action)
        {
            case "idle":
            case "wait":
                Debug.Log("[Executor] Idle/Wait action – no movement");
                break;
				case "talk":
					ExecuteTalk(proposal.actorId, agent);
					break;
				case "inspect":
					ExecuteInspect(proposal.actorId, agent);
					break;
            case "move":
                ExecuteMove(first.@params, agent, proposal.actorId);
                break;
            default:
                Debug.LogWarning($"[Executor] Unknown action '{first.action}'");
                break;
        }
    }

    private void ExecuteMove(Dictionary<string, object> paramBag, Transform agent, string actorId)
    {
        if (agent == null)
        {
            Debug.LogWarning("[Executor] Move requested but no agent assigned");
            return;
        }

        float dx = ReadFloat(paramBag, "destDX", 0f);
        float dz = ReadFloat(paramBag, "destDZ", 2f);

        var dest = agent.position + new Vector3(dx, 0f, dz);

        var nav = agent.GetComponent<NavMeshAgent>();
        if (nav != null && nav.isOnNavMesh)
        {
            nav.SetDestination(dest);
            Debug.Log($"[Executor] NavMesh move to {dest}");
        }
        else
        {
            agent.position = dest;
            Debug.Log($"[Executor] Teleport move to {dest}");
        }

        var party = GetComponent<PartyState>();
        if (party != null)
        {
            party.SyncFromTransform(string.IsNullOrEmpty(actorId) ? "adv-1" : actorId, agent);
        }
    }

	private void ExecuteTalk(string actorId, Transform agent)
	{
		Debug.Log($"[Executor] {actorId} talks to nearby NPC/object.");
		var highlighter = agent != null ? agent.GetComponent<AgentHighlighter>() : null;
		highlighter?.Flash(true);
		var party = GetComponent<PartyState>();
		if (party != null && agent != null)
		{
			party.SyncFromTransform(string.IsNullOrEmpty(actorId) ? "adv-1" : actorId, agent);
		}
	}

	private void ExecuteInspect(string actorId, Transform agent)
	{
		Debug.Log($"[Executor] {actorId} inspects surroundings.");
		var highlighter = agent != null ? agent.GetComponent<AgentHighlighter>() : null;
		highlighter?.Flash(false);
		var party = GetComponent<PartyState>();
		if (party != null && agent != null)
		{
			party.SyncFromTransform(string.IsNullOrEmpty(actorId) ? "adv-1" : actorId, agent);
		}
	}

    private static float ReadFloat(Dictionary<string, object> bag, string key, float fallback)
    {
        if (bag == null || !bag.TryGetValue(key, out var raw) || raw == null) return fallback;
        if (raw is float f) return f;
        if (raw is double d) return (float)d;
        if (raw is int i) return i;
        if (float.TryParse(raw.ToString(), out var parsed)) return parsed;
        return fallback;
    }
}


