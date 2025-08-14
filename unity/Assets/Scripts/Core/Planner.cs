using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

// DTOs aligned with Gateway → Unity contract (v1)
public class CandidateAction
{
    public string action;
    public Dictionary<string, object> @params;
}

public class IntentProposal
{
    public string type;
    public string actorId;
    public string goal;
    public string intent;
    public string rationale;
    public int? suggestedDC;
    public List<CandidateAction> candidateActions;
}

public class Planner : MonoBehaviour
{
    public Transform agent;  // legacy single agent
    public Transform agent2;
    public Transform agent3;
    private static Planner _instance;
    void Awake() { _instance = this; }

    // Called by BrainClient with raw JSON from the gateway
    public static void TryConsume(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        IntentProposal msg = null;
        try { msg = JsonConvert.DeserializeObject<IntentProposal>(json); }
        catch { /* ignore malformed frames */ }
        if (msg == null) return;
        if (msg.type != "IntentProposal") return;

        if (_instance == null)
        {
            Debug.LogWarning("[Planner] No Planner instance available");
            return;
        }

        // Delegate through Dice/DC gate
        var gate = _instance.GetComponent<DiceGate>();
        if (gate == null) gate = _instance.gameObject.AddComponent<DiceGate>();
        // Track candidate count for UI purposes
        var agent = _instance.ResolveAgentFor(msg.actorId);
        var ui = _instance.GetComponent<DiceGateUI>();
        if (ui != null) ui.SetCandidateMeta(msg.candidateActions != null ? msg.candidateActions.Count : 1);
        gate.ProcessProposal(msg, agent);
    }

    public Transform ResolveAgentFor(string actorId)
    {
        if (string.IsNullOrEmpty(actorId) || actorId == "adv-1") return agent;
        if (actorId == "adv-2") return agent2 != null ? agent2 : agent;
        if (actorId == "adv-3") return agent3 != null ? agent3 : agent;
        return agent;
    }
}
