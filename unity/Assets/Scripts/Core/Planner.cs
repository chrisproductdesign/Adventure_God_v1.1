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
    public Transform agent;  // assign in Inspector
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
        gate.ProcessProposal(msg, _instance.agent);
    }
}
