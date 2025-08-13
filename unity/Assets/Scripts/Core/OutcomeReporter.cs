using System.Collections.Generic;

public static class OutcomeReporter
{
	private static readonly Dictionary<string, string> _actorToOutcome = new Dictionary<string, string>();

	public static void Report(string actorId, bool success, int roll, int dc)
	{
		_actorToOutcome[actorId] = success ? $"success({roll}/{dc})" : $"fail({roll}/{dc})";
	}

	public static string GetLastOutcome(string actorId)
	{
		return _actorToOutcome.TryGetValue(actorId, out var v) ? v : null;
	}
}


