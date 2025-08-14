using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
	private PartyState _party;
	private string _savePath;

	void Awake()
	{
		_party = GetComponent<PartyState>();
		if (_party == null) _party = gameObject.AddComponent<PartyState>();
		_savePath = Path.Combine(Application.persistentDataPath, "save.json");
	}

	public void Save()
	{
		var data = new SaveData { actors = new List<ActorRecord>(), dmNotes = DMNarration.GetAllNotes() };
		foreach (var a in _party.Enumerate())
		{
			data.actors.Add(new ActorRecord
			{
				id = a.id,
				x = a.position.x,
				y = a.position.y,
				z = a.position.z,
				hp = a.hp,
				inventory = a.inventory?.ToArray() ?? new string[0]
			});
		}
		var json = JsonConvert.SerializeObject(data, Formatting.Indented);
		File.WriteAllText(_savePath, json);
		Debug.Log($"[SaveLoad] Saved {data.actors.Count} actors → {_savePath}");
	}

	public void Load()
	{
		if (!File.Exists(_savePath)) { Debug.LogWarning("[SaveLoad] No save file"); return; }
		var json = File.ReadAllText(_savePath);
		var data = JsonConvert.DeserializeObject<SaveData>(json);
		if (data?.actors == null) { Debug.LogWarning("[SaveLoad] Empty save"); return; }
		foreach (var rec in data.actors)
		{
			var st = _party.EnsureActor(rec.id);
			st.hp = rec.hp;
			st.position = new Vector3(rec.x, rec.y, rec.z);
			st.inventory = new List<string>(rec.inventory ?? new string[0]);
		}
		// Restore DM notes
		if (data.dmNotes != null)
		{
			foreach (var kv in data.dmNotes)
			{
				DMNarration.SetLastNote(kv.Key, kv.Value);
			}
		}
		Debug.Log($"[SaveLoad] Loaded {data.actors.Count} actors from {_savePath}");
	}

	class SaveData { public List<ActorRecord> actors; public Dictionary<string,string> dmNotes; }
	class ActorRecord { public string id; public float x,y,z; public int hp; public string[] inventory; }
}


