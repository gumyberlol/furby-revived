using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class PrefabPool : SingletonInstance<PrefabPool>
	{
		private Dictionary<GameObject, Queue<GameObject>> m_prefabPools = new Dictionary<GameObject, Queue<GameObject>>();

		private Dictionary<GameObject, GameObject> m_prefabPoolRoots = new Dictionary<GameObject, GameObject>();

		private Dictionary<GameObject, int> m_numRegistered = new Dictionary<GameObject, int>();

		private GameObject m_unpooledRoot;

		public GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			if (IsPoolable(prefab))
			{
				if (!m_prefabPools.ContainsKey(prefab))
				{
					CreatePrefabPoolInternal(prefab, 1, false);
				}
				if (m_prefabPools[prefab].Count > 0)
				{
					GameObject gameObject = m_prefabPools[prefab].Dequeue();
					gameObject.SetActive(true);
					gameObject.BroadcastMessage("ResetForPool", SendMessageOptions.DontRequireReceiver);
					gameObject.transform.position = position;
					gameObject.transform.rotation = rotation;
					return gameObject;
				}
				GameObject gameObject2 = (GameObject)Object.Instantiate(prefab, position, rotation);
				PooledObject componentInChildrenIncludeInactive = gameObject2.GetComponentInChildrenIncludeInactive<PooledObject>();
				componentInChildrenIncludeInactive.SetPoolSettings(prefab);
				gameObject2.SetActive(true);
				componentInChildrenIncludeInactive.BroadcastMessage("ResetForPool", SendMessageOptions.DontRequireReceiver);
				gameObject2.transform.parent = m_prefabPoolRoots[prefab].transform;
				return gameObject2;
			}
			GameObject gameObject3 = (GameObject)Object.Instantiate(prefab, position, rotation);
			gameObject3.SetActive(true);
			gameObject3.BroadcastMessage("ResetForPool", SendMessageOptions.DontRequireReceiver);
			if (Debug.isDebugBuild)
			{
				if ((bool)gameObject3.GetComponentInChildren<PooledObject>() && !gameObject3.GetComponent<PooledObject>())
				{
					Logging.Log("RsPooledObject component is found in prefab " + prefab.name + ", but not in root. This will cause problems.");
				}
				if (gameObject3.GetComponentsInChildren<PooledObject>().Length > 1)
				{
					Logging.Log("Multiple RsPooledObject components is found in prefab " + prefab.name + ". This will cause problems.");
				}
			}
			PooledObject pooledObject = gameObject3.AddComponent<PooledObject>();
			pooledObject.SetPoolSettings(null);
			if (m_unpooledRoot == null)
			{
				m_unpooledRoot = new GameObject("UnpooledRoot");
			}
			pooledObject.transform.parent = m_unpooledRoot.transform;
			return gameObject3;
		}

		public void RegisterPrefabUser(GameObject prefab)
		{
			PooledObject potentiallyInactivePoolableComponent = GetPotentiallyInactivePoolableComponent(prefab);
			if ((bool)potentiallyInactivePoolableComponent)
			{
				int defaultPooledObjectsPerUser = potentiallyInactivePoolableComponent.m_defaultPooledObjectsPerUser;
				CreatePrefabPoolInternal(prefab, defaultPooledObjectsPerUser, true);
			}
		}

		public void UnregisterPrefabUser(GameObject prefab)
		{
			PooledObject potentiallyInactivePoolableComponent = GetPotentiallyInactivePoolableComponent(prefab);
			if ((bool)potentiallyInactivePoolableComponent)
			{
				int defaultPooledObjectsPerUser = potentiallyInactivePoolableComponent.m_defaultPooledObjectsPerUser;
				if (m_numRegistered.ContainsKey(prefab))
				{
					Dictionary<GameObject, int> numRegistered;
					Dictionary<GameObject, int> dictionary = (numRegistered = m_numRegistered);
					GameObject key2;
					GameObject key = (key2 = prefab);
					int num = numRegistered[key2];
					dictionary[key] = num - defaultPooledObjectsPerUser;
				}
			}
		}

		public void CreatePrefabPool(GameObject prefab, int size)
		{
			if (IsPoolable(prefab))
			{
				CreatePrefabPoolInternal(prefab, size, false);
			}
		}

		private void CreatePrefabPoolInternal(GameObject prefab, int size, bool add)
		{
			if (!m_prefabPools.ContainsKey(prefab))
			{
				m_prefabPools[prefab] = new Queue<GameObject>();
				m_prefabPoolRoots[prefab] = new GameObject(prefab.name + "_Pool");
				m_numRegistered[prefab] = size;
			}
			else if (add)
			{
				Dictionary<GameObject, int> numRegistered;
				Dictionary<GameObject, int> dictionary = (numRegistered = m_numRegistered);
				GameObject key2;
				GameObject key = (key2 = prefab);
				int num = numRegistered[key2];
				dictionary[key] = num + size;
			}
			else
			{
				m_numRegistered[prefab] = Mathf.Max(m_numRegistered[prefab], size);
			}
			while (m_prefabPools[prefab].Count < m_numRegistered[prefab])
			{
				GameObject gameObject = (GameObject)Object.Instantiate(prefab);
				m_prefabPools[prefab].Enqueue(gameObject);
				PooledObject component = gameObject.GetComponent<PooledObject>();
				component.SetPoolSettings(prefab);
				component.SetInPool(true);
				gameObject.transform.parent = m_prefabPoolRoots[prefab].transform;
				gameObject.SetActive(false);
			}
		}

		public void ReturnToPool(GameObject pooledObject)
		{
			PooledObject potentiallyInactivePoolableComponent = GetPotentiallyInactivePoolableComponent(pooledObject);
			if (potentiallyInactivePoolableComponent == null)
			{
				Logging.Log("ReturnToPool called on non-pooled object: " + pooledObject.name);
				Object.Destroy(pooledObject);
				return;
			}
			GameObject gameObject = potentiallyInactivePoolableComponent.gameObject;
			if (potentiallyInactivePoolableComponent.GetPool() != null)
			{
				if (!potentiallyInactivePoolableComponent.IsInPool())
				{
					m_prefabPools[potentiallyInactivePoolableComponent.GetPool()].Enqueue(gameObject);
					potentiallyInactivePoolableComponent.SetInPool(true);
				}
				gameObject.BroadcastMessage("OnDestroy", SendMessageOptions.DontRequireReceiver);
				gameObject.SetActive(false);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}

		private PooledObject GetPotentiallyInactivePoolableComponent(GameObject objectToCheck)
		{
			PooledObject pooledObject = objectToCheck.GetComponent<PooledObject>();
			if (pooledObject != null)
			{
				return pooledObject;
			}
			if (objectToCheck.transform.parent != null)
			{
				pooledObject = GetPotentiallyInactivePoolableComponent(objectToCheck.transform.parent.gameObject);
			}
			return pooledObject;
		}

		private bool IsPoolable(GameObject objectToCheck)
		{
			return GetPotentiallyInactivePoolableComponent(objectToCheck) != null;
		}
	}
}
