using UnityEngine;
using UnityEngine.AI;

public static class RuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureAIHost()
    {
        var gate = Object.FindObjectOfType<DiceGate>();
        if (gate != null) return; // already set up

        var host = new GameObject("AIHost");
        var planner = host.AddComponent<Planner>();
		// BrainClient talks to external gateway. Keep it but allow local demo to run without it.
		if (Application.isEditor)
		{
			host.AddComponent<LocalIntentDemo>();
			host.AddComponent<BrainClient>();
		}
		else
		{
			host.AddComponent<BrainClient>();
		}
        host.AddComponent<DiceGate>();
        host.AddComponent<ModernGameUI>(); // Modern Apple-inspired UI
		host.AddComponent<DMHud>();

		// Ensure three agents exist
		var agent1 = GameObject.Find("Agent-1");
		if (agent1 == null)
		{
			agent1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			agent1.name = "Agent-1";
			agent1.transform.position = new Vector3(0, 0, 0);
			agent1.AddComponent<AgentHighlighter>();
			agent1.AddComponent<AgentTag>().actorId = "adv-1";
		}
		planner.agent = agent1.transform;

		var agent2 = GameObject.Find("Agent-2");
		if (agent2 == null)
		{
			agent2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			agent2.name = "Agent-2";
			agent2.transform.position = new Vector3(2, 0, 0);
			agent2.AddComponent<AgentHighlighter>();
			agent2.AddComponent<AgentTag>().actorId = "adv-2";
		}
		planner.agent2 = agent2.transform;

		var agent3 = GameObject.Find("Agent-3");
		if (agent3 == null)
		{
			agent3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			agent3.name = "Agent-3";
			agent3.transform.position = new Vector3(-2, 0, 0);
			agent3.AddComponent<AgentHighlighter>();
			agent3.AddComponent<AgentTag>().actorId = "adv-3";
		}
		planner.agent2 = agent2.transform;
		planner.agent3 = agent3.transform;

        // Camera follow
        var cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        var tdc = cam.gameObject.GetComponent<TopDownCamera>();
        if (tdc == null) tdc = cam.gameObject.AddComponent<TopDownCamera>();
		tdc.target = agent1.transform;

		// Labels for each agent
		var lab1 = host.AddComponent<AgentLabel>();
		lab1.target = agent1.transform;
		var lab2 = host.AddComponent<AgentLabel>();
		lab2.target = agent2.transform;
		var lab3 = host.AddComponent<AgentLabel>();
		lab3.target = agent3.transform;
    }
}



