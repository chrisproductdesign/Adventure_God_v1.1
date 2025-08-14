using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SceneLoader : MonoBehaviour
{
	public TextAsset defaultSceneJson;

	public void LoadFrom(string path)
	{
		if (!File.Exists(path))
		{
			Debug.LogWarning($"[SceneLoader] File not found: {path}");
			return;
		}
		var json = File.ReadAllText(path);
		LoadFromJson(json);
	}

	public void LoadDefault()
	{
		if (defaultSceneJson == null)
		{
			Debug.LogWarning("[SceneLoader] No default scene assigned");
			return;
		}
		LoadFromJson(defaultSceneJson.text);
	}

	private void LoadFromJson(string json)
	{
		if (string.IsNullOrEmpty(json)) return;
		SceneSpec spec = null;
		try { spec = JsonConvert.DeserializeObject<SceneSpec>(json); }
		catch { Debug.LogWarning("[SceneLoader] Bad scene JSON"); }
		if (spec == null) return;

		if (spec.pois != null)
		{
			foreach (var p in spec.pois)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				go.name = string.IsNullOrEmpty(p.name) ? (p.id ?? "poi") : p.name;
				go.transform.position = new Vector3(p.x, p.y, p.z);
				POIRegistry.Register(go.name, go.transform);
			}
		}
		if (spec.npcs != null)
		{
			foreach (var n in spec.npcs)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				go.name = string.IsNullOrEmpty(n.name) ? (n.id ?? "npc") : n.name;
				go.transform.position = new Vector3(n.x, n.y, n.z);
			}
		}
		Debug.Log("[SceneLoader] Scene spawned");
	}
}


