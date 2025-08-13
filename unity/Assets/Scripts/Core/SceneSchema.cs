using System.Collections.Generic;

// DTOs for scene seeding via JSON
public class SceneSpec
{
	public List<PoiSpec> pois;
	public List<NpcSpec> npcs;
}

public class PoiSpec
{
	public string id;
	public string name;
	public float x;
	public float y;
	public float z;
}

public class NpcSpec
{
	public string id;
	public string name;
	public float x;
	public float y;
	public float z;
}


