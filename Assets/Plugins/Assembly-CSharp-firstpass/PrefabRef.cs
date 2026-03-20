using System;
using UnityEngine;

[Serializable]
public class PrefabRef
{
	public string name = string.Empty;

	public string newName = string.Empty;

	public GameObject prefab;

	public PrefabRef(string name, GameObject prefab)
	{
		this.name = name;
		newName = name;
		this.prefab = prefab;
	}

	public PrefabRef(string name)
	{
		this.name = name;
		newName = name;
	}

	public PrefabRef()
	{
	}
}
