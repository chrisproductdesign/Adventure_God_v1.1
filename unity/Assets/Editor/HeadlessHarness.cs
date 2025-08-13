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
		agent.name = "Agent";
		agent.transform.position = Vector3.zero;
		agent.AddComponent<NavMeshAgent>();

		var host = new GameObject("AIHost");
		var planner = host.AddComponent<Planner>();
		host.AddComponent<BrainClient>();
		planner.agent = agent.transform;
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


