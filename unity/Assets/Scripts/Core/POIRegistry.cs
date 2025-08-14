using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class POIRegistry
{
	private static readonly Dictionary<string, Transform> _nameToTransform = new Dictionary<string, Transform>();

	public static void Register(string name, Transform t)
	{
		if (string.IsNullOrEmpty(name) || t == null) return;
		_nameToTransform[name] = t;
	}

	public static Transform Get(string name)
	{
		if (string.IsNullOrEmpty(name)) return null;
		return _nameToTransform.TryGetValue(name, out var t) ? t : null;
	}

	public static List<string> GetAllNames()
	{
		return _nameToTransform.Keys.OrderBy(n => n).ToList();
	}
}


