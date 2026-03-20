using System.Collections.Generic;
using UnityEngine;

public abstract class TestScript : ScriptableObject
{
	[HideInInspector]
	public float startTime;

	[HideInInspector]
	public List<GameObject> exGhosts;

	[HideInInspector]
	public List<GameObject> created = new List<GameObject>();

	[HideInInspector]
	public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

	protected GameObject New(GameObject prefab)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
		created.Add(gameObject);
		return gameObject;
	}

	protected T New<T>(GameObject prefab) where T : Component
	{
		GameObject gameObject = New(prefab);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	protected GameObject New(GameObject prefab, Vector3 position)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab, position, prefab.transform.rotation);
		created.Add(gameObject);
		return gameObject;
	}

	protected T New<T>(GameObject prefab, Vector3 position) where T : Component
	{
		GameObject gameObject = New(prefab, position);
		return gameObject.GetComponent<T>();
	}

	protected GameObject New(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab, position, rotation);
		created.Add(gameObject);
		return gameObject;
	}

	protected T New<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab, position, rotation);
		created.Add(gameObject);
		return gameObject.GetComponent<T>();
	}

	protected GameObject New(string prefabName)
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return null;
		}
		GameObject prefab = prefabs[prefabName];
		return New(prefab);
	}

	protected T New<T>(string prefabName) where T : Component
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return (T)null;
		}
		GameObject prefab = prefabs[prefabName];
		return New<T>(prefab);
	}

	protected GameObject New(string prefabName, Vector3 position)
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return null;
		}
		GameObject prefab = prefabs[prefabName];
		return New(prefab, position);
	}

	protected T New<T>(string prefabName, Vector3 position) where T : Component
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return (T)null;
		}
		GameObject prefab = prefabs[prefabName];
		return New<T>(prefab, position);
	}

	protected GameObject New(string prefabName, Vector3 position, Quaternion rotation)
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return null;
		}
		GameObject prefab = prefabs[prefabName];
		return New(prefab, position, rotation);
	}

	protected T New<T>(string prefabName, Vector3 position, Quaternion rotation) where T : Component
	{
		if (!prefabs.ContainsKey(prefabName))
		{
			return (T)null;
		}
		GameObject prefab = prefabs[prefabName];
		return New<T>(prefab, position, rotation);
	}

	protected GameObject GetExGhost(string name)
	{
		return Get(name, true);
	}

	protected GameObject[] GetExGhosts(string name)
	{
		return GetAll(name, true);
	}

	protected T GetExGhost<T>() where T : Component
	{
		return Get<T>(true);
	}

	protected T[] GetExGhosts<T>() where T : Component
	{
		return GetAll<T>(true);
	}

	protected T GetExGhost<T>(string name) where T : Component
	{
		return Get<T>(name, true);
	}

	protected T[] GetExGhosts<T>(string name) where T : Component
	{
		return GetAll<T>(name, true);
	}

	protected GameObject GetCreated(string name)
	{
		return Get(name, false);
	}

	protected GameObject[] GetCreateds(string name)
	{
		return GetAll(name, false);
	}

	protected T GetCreated<T>() where T : Component
	{
		return Get<T>(false);
	}

	protected T[] GetCreateds<T>() where T : Component
	{
		return GetAll<T>(false);
	}

	protected T GetCreated<T>(string name) where T : Component
	{
		return Get<T>(name, false);
	}

	protected T[] GetCreateds<T>(string name) where T : Component
	{
		return GetAll<T>(name, false);
	}

	private List<GameObject> GetList(bool ghost)
	{
		if (ghost)
		{
			return exGhosts;
		}
		return created;
	}

	private GameObject Get(string name, bool ghost)
	{
		return GetList(ghost).Find((GameObject go) => go.name == name);
	}

	private GameObject[] GetAll(string name, bool ghost)
	{
		return GetList(ghost).FindAll((GameObject go) => go.name == name).ToArray();
	}

	private T Get<T>(bool ghost) where T : Component
	{
		GameObject gameObject = GetList(ghost).Find((GameObject g) => g.GetComponent<T>() != null);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	private T[] GetAll<T>(bool ghost) where T : Component
	{
		List<T> list = new List<T>();
		List<GameObject> list2 = GetList(ghost).FindAll((GameObject go) => go.GetComponent<T>() != null);
		foreach (GameObject item in list2)
		{
			list.Add(item.GetComponent<T>());
		}
		return list.ToArray();
	}

	private T Get<T>(string name, bool ghost) where T : Component
	{
		GameObject gameObject = Get(name, ghost);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	private T[] GetAll<T>(string name, bool ghost) where T : Component
	{
		List<T> list = new List<T>();
		GameObject[] all = GetAll(name, ghost);
		foreach (GameObject gameObject in all)
		{
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list.ToArray();
	}

	public virtual void SetUp()
	{
	}

	public virtual void TearDown()
	{
	}
}
