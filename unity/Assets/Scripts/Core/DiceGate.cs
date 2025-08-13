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
		if (proposal.suggestedDC.HasValue)
		{
			currentDC = Mathf.Clamp(proposal.suggestedDC.Value, 5, 20);
		}
		int roll = RollD20();
		bool success = roll >= currentDC;
		Log($"[Dice] Roll d20={roll} vs DC {currentDC} → {(success ? "SUCCESS" : "FAIL")}");
		if (success)
		{
			_executor.Execute(proposal, agent);
		}
		else
		{
			Log("[Dice] Action blocked by DC");
		}
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


