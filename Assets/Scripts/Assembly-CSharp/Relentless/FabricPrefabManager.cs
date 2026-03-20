using System.Collections;
using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricPrefabManager")]
	public class FabricPrefabManager : RelentlessMonoBehaviour
	{
		private ArrayList m_loadedFabricPrefabs;

		private static FabricPrefabManager m_instance;

		public static FabricPrefabManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = (FabricPrefabManager)Object.FindObjectOfType(typeof(FabricPrefabManager));
					if (m_instance == null)
					{
						GameObject gameObject = GameObject.Find("InGameAudio");
						if (gameObject == null)
						{
							gameObject = new GameObject("InGameAudio");
						}
						m_instance = gameObject.AddComponent<FabricPrefabManager>();
					}
				}
				return m_instance;
			}
		}

		private void Awake()
		{
			m_loadedFabricPrefabs = new ArrayList();
			Object.DontDestroyOnLoad(this);
		}

		private void Update()
		{
		}

		public bool IsPrefabLoaded(GameObject prefab)
		{
			return m_loadedFabricPrefabs.Contains(prefab);
		}

		public bool IsPrefabLoaded(string prefabPath)
		{
			return m_loadedFabricPrefabs.Contains(prefabPath);
		}

		public void AddPrefab(GameObject prefab)
		{
			if (!m_loadedFabricPrefabs.Contains(prefab))
			{
				m_loadedFabricPrefabs.Add(prefab);
			}
		}

		public void AddPrefab(string prefabPath)
		{
			if (!m_loadedFabricPrefabs.Contains(prefabPath))
			{
				m_loadedFabricPrefabs.Add(prefabPath);
			}
		}

		public void UnloadAll()
		{
			int count = m_loadedFabricPrefabs.Count;
			for (int i = 0; i < count; i++)
			{
				string text = (string)m_loadedFabricPrefabs[i];
				if (text != null)
				{
					FabricManager.Instance.UnloadAsset(text);
				}
			}
			m_loadedFabricPrefabs.Clear();
		}
	}
}
