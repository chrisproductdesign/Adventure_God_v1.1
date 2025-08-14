using UnityEngine;
using UnityEngine.UI;

public class DiceGate : MonoBehaviour
{
	[Range(5, 20)] public int currentDC = 10;
	public bool logToConsole = true;

	private IntentProposal _lastProposal;
	private Transform _lastAgent;
	private ActionExecutor _executor;
	private DiceGateUI _ui;
	private int _candidateIndex = 0;

	void Awake()
	{
		_executor = GetComponent<ActionExecutor>();
		if (_executor == null) _executor = gameObject.AddComponent<ActionExecutor>();
		_ui = GetComponent<DiceGateUI>();
		if (_ui == null) _ui = gameObject.AddComponent<DiceGateUI>();
	}

	public void ProcessProposal(IntentProposal proposal, Transform agent)
	{
		_lastProposal = proposal;
		_lastAgent = agent;
		// Respect suggestedDC only if UI requested it
		var uiWantsSuggested = GetComponent<DiceGateUI>() != null && typeof(DiceGateUI).GetField("_respectSuggestedDc", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic) != null && (bool)typeof(DiceGateUI).GetField("_respectSuggestedDc", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(GetComponent<DiceGateUI>());
		if (proposal.suggestedDC.HasValue && uiWantsSuggested)
		{
			currentDC = Mathf.Clamp(proposal.suggestedDC.Value, 5, 20);
		}
		int roll = RollD20();
		bool success = roll >= currentDC;
		Log("[Dice] Roll d20=" + roll + " vs DC " + currentDC + " → " + (success ? "SUCCESS" : "FAIL"));
		OutcomeReporter.Report(proposal.actorId, success, roll, currentDC);
		var hl = agent != null ? agent.GetComponent<AgentHighlighter>() : null;
		if (hl == null && agent != null) hl = agent.gameObject.AddComponent<AgentHighlighter>();
		hl?.Flash(success);
		if (success)
		{
			// If UI cycled a candidate, execute only that one
			if (proposal.candidateActions != null && proposal.candidateActions.Count > 1)
			{
				var chosen = Mathf.Clamp(_candidateIndex, 0, proposal.candidateActions.Count - 1);
				var single = new IntentProposal
				{
					type = proposal.type,
					actorId = proposal.actorId,
					goal = proposal.goal,
					intent = proposal.intent,
					rationale = proposal.rationale,
					suggestedDC = proposal.suggestedDC,
					candidateActions = new System.Collections.Generic.List<CandidateAction> { proposal.candidateActions[chosen] }
				};
				_executor.Execute(single, agent);
			}
			else
			{
				_executor.Execute(proposal, agent);
			}
		}
		else
		{
			Log("[Dice] Action blocked by DC");
		}
	}

	public void SetCandidateIndex(int idx)
	{
		_candidateIndex = Mathf.Max(0, idx);
	}

	// Stages a proposal without rolling; DM can later press Roll to resolve
	public void StageProposal(IntentProposal proposal, Transform agent)
	{
		_lastProposal = proposal;
		_lastAgent = agent;
		Log("[Dice] Staged proposal for actor=" + proposal.actorId + ", intent=" + proposal.intent + ". Press Roll to resolve.");
		var ui = GetComponent<DiceGateUI>();
		if (ui != null) ui.SetCandidateMeta(proposal.candidateActions != null ? proposal.candidateActions.Count : 1);
		GetComponent<DiceGateUI>()?.ResetCandidateIndex();
	}

	public void RerollLast()
	{
		if (_lastProposal == null || _lastAgent == null)
		{
			Log("[Dice] No last proposal to reroll");
			return;
		}
		ProcessProposal(_lastProposal, _lastAgent);
	}

	public void SetDC(float dc)
	{
		currentDC = Mathf.Clamp(Mathf.RoundToInt(dc), 5, 20);
		Log($"[Dice] DC set to {currentDC}");
	}

	private int RollD20()
	{
		return Random.Range(1, 21);
	}

	private void Log(string message)
	{
		if (logToConsole) Debug.Log(message);
		_ui?.AppendLog(message);
	}
}


