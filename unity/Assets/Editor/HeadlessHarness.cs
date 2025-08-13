using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class HeadlessHarness
{
	public static void Run()
	{
		SetupScene();
		StartPlayAndExitAfterSeconds(8.0);
	}

	private static void SetupScene()
	{
		EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		var agent = GameObject.CreatePrimitive(PrimitiveType.Cube);
		agent.name = "Agent-1";
		agent.transform.position = Vector3.zero;
		agent.AddComponent<NavMeshAgent>();
		var agent2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		agent2.name = "Agent-2";
		agent2.transform.position = new Vector3(2, 0, 0);
		agent2.AddComponent<NavMeshAgent>();
		var agent3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		agent3.name = "Agent-3";
		agent3.transform.position = new Vector3(-2, 0, 0);
		agent3.AddComponent<NavMeshAgent>();
		agent.AddComponent<AgentHighlighter>();
		agent2.AddComponent<AgentHighlighter>();
		agent3.AddComponent<AgentHighlighter>();
		agent.AddComponent<AgentTag>().actorId = "adv-1";
		agent2.AddComponent<AgentTag>().actorId = "adv-2";
		agent3.AddComponent<AgentTag>().actorId = "adv-3";

		var host = new GameObject("AIHost");
		var planner = host.AddComponent<Planner>();
		host.AddComponent<BrainClient>();
		host.AddComponent<DiceGate>();
		host.AddComponent<PartyState>();
		host.AddComponent<PartyHUD>();
		host.AddComponent<SaveLoadManager>();
		host.AddComponent<SaveLoadUI>();
		var sceneLoader = host.AddComponent<SceneLoader>();
		planner.agent = agent.transform;
		planner.agent2 = agent2.transform;
		planner.agent3 = agent3.transform;

		// Setup simple top-down camera following Agent-1 (for Editor runs)
		var cam = Camera.main;
		if (cam == null)
		{
			var camGO = new GameObject("Main Camera");
			cam = camGO.AddComponent<Camera>();
			cam.tag = "MainCamera";
		}
		var tdc = cam.gameObject.AddComponent<TopDownCamera>();
		tdc.target = agent.transform;

		// Labels
		var lab = host.AddComponent<AgentLabel>();
		lab.target = agent.transform;
		var lab2 = host.AddComponent<AgentLabel>();
		lab2.target = agent2.transform;
		var lab3 = host.AddComponent<AgentLabel>();
		lab3.target = agent3.transform;

		// Load default scene JSON if present at Assets/Scenes/default_scene.json
		var ta = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>("Assets/Scenes/default_scene.json");
		if (ta != null)
		{
			sceneLoader.defaultSceneJson = ta;
			sceneLoader.LoadDefault();
		}
	}

	private static void StartPlayAndExitAfterSeconds(double seconds)
	{
		double start = EditorApplication.timeSinceStartup;
		EditorApplication.isPlaying = true;
		EditorApplication.update += Tick;

		void Tick()
		{
			if (!EditorApplication.isPlaying) return;
			double elapsed = EditorApplication.timeSinceStartup - start;
			if (elapsed >= seconds)
			{
				EditorApplication.update -= Tick;
				EditorApplication.Exit(0);
			}
		}
	}
}


