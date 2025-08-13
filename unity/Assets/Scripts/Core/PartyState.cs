using UnityEngine;
using System.Collections.Generic;

public class PartyState : MonoBehaviour
{
	[System.Serializable]
	public class ActorState
	{
		public string id;
		public Vector3 position;
		public int hp;
		public List<string> inventory;

		public ActorState(string id)
		{
			this.id = id;
			this.position = Vector3.zero;
			this.hp = 10;
			this.inventory = new List<string>();
		}
	}

	private readonly Dictionary<string, ActorState> _actors = new Dictionary<string, ActorState>();

	public ActorState EnsureActor(string actorId)
	{
		if (!_actors.TryGetValue(actorId, out var st))
		{
			st = new ActorState(actorId);
			_actors[actorId] = st;
		}
		return st;
	}

	public void SyncFromTransform(string actorId, Transform t)
	{
		if (t == null) return;
		var st = EnsureActor(actorId);
		st.position = t.position;
	}

	public IEnumerable<ActorState> Enumerate()
	{
		return _actors.Values;
	}
}


