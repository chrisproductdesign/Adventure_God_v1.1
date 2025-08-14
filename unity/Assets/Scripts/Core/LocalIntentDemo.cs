using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

// Generates IntentProposal locally so Unity can run without the gateway
public class LocalIntentDemo : MonoBehaviour
{
    public bool enabledDemo = true;
    public float intervalSeconds = 2f;
    private string[] actors = new[] { "adv-1", "adv-2", "adv-3" };
    private Coroutine _runner;

    void OnEnable()
    {
        if (enabledDemo && _runner == null) _runner = StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        var idx = 0;
        while (enabledDemo)
        {
            var actor = actors[idx % actors.Length];
            idx++;

            // Alternate intents: move → talk → inspect
            int mod = idx % 3;
            var action = mod == 0 ? "move" : (mod == 1 ? "talk" : "inspect");
            var intent = action;
            var goal = action == "move" ? "advance" : (action == "talk" ? "communicate" : "observe");

            var candidate = new CandidateAction
            {
                action = action,
                @params = action == "move"
                    ? new Dictionary<string, object> { { "destDX", 0 }, { "destDZ", 3 } }
                    : new Dictionary<string, object>()
            };

            var proposal = new IntentProposal
            {
                type = "IntentProposal",
                actorId = actor,
                goal = goal,
                intent = intent,
                rationale = "local-demo",
                suggestedDC = action == "move" ? 10 : 8,
                candidateActions = new List<CandidateAction> { candidate }
            };

            var gate = GetComponent<DiceGate>();
            if (gate == null) gate = gameObject.AddComponent<DiceGate>();
            var planner = GetComponent<Planner>();
            if (planner == null) planner = gameObject.AddComponent<Planner>();

            gate.ProcessProposal(proposal, planner != null ? ResolveAgent(planner, actor) : null);

            yield return new WaitForSeconds(intervalSeconds);
        }
        _runner = null;
    }

    private Transform ResolveAgent(Planner planner, string actorId)
    {
        return planner != null ? planner.ResolveAgentFor(actorId) : null;
    }

    public void SetEnabled(bool on)
    {
        enabledDemo = on;
        if (on)
        {
            if (_runner == null) _runner = StartCoroutine(Loop());
        }
        else
        {
            if (_runner != null)
            {
                StopCoroutine(_runner);
                _runner = null;
            }
        }
    }
}


