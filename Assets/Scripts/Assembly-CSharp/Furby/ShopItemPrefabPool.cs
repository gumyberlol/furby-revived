using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class ShopItemPrefabPool : MonoBehaviour
	{
		public int m_NumberToPool = 100;

		public GameObject m_SourcePrefab;

		public List<GameObject> m_InstancedPrefabs = new List<GameObject>();

		public List<bool> m_InOrOut = new List<bool>();

		public int m_NumberInPool;

		public int m_PoolIncreaseSize = 10;

		private bool m_Initialised;

		public bool Initialised
		{
			get
			{
				return m_Initialised;
			}
			set
			{
				m_Initialised = value;
			}
		}

		private void Awake()
		{
			InitialisePool();
		}

		private void OnDestroy()
		{
			ClearPool();
			m_InstancedPrefabs.Clear();
			m_InOrOut.Clear();
		}

		private void InitialisePool()
		{
			IncreasePoolSize(m_NumberToPool);
			Initialised = true;
		}

		public void ClearPool()
		{
			int num = 0;
			UISprite[] array = null;
			for (int i = 0; i < m_NumberInPool; i++)
			{
				if (m_InOrOut[i])
				{
					array = m_InstancedPrefabs[i].GetComponentsInChildren<UISprite>();
					for (int j = 0; j < array.Length; j++)
					{
						array[j].atlas = null;
						array[j].material = null;
					}
					Object.Destroy(m_InstancedPrefabs[i]);
					m_InOrOut[i] = false;
					num++;
				}
			}
		}

		private int GetIndexOfUnusedPrefab()
		{
			for (int i = 0; i < m_NumberInPool; i++)
			{
				if (m_InOrOut[i])
				{
					return i;
				}
			}
			IncreasePoolSize(m_PoolIncreaseSize);
			return GetIndexOfUnusedPrefab();
		}

		private void IncreasePoolSize(int size)
		{
			for (int i = m_NumberInPool; i < m_NumberInPool + size; i++)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(m_SourcePrefab, base.gameObject.transform.position, base.gameObject.transform.rotation);
				gameObject.name = m_SourcePrefab.name + "_Pooled_" + i;
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.transform.position = base.gameObject.transform.position;
				m_InstancedPrefabs.Add(gameObject);
				m_InOrOut.Add(true);
			}
			m_NumberInPool += size;
		}

		public GameObject GetPrefabInstanceFromPool()
		{
			int indexOfUnusedPrefab = GetIndexOfUnusedPrefab();
			m_InOrOut[indexOfUnusedPrefab] = false;
			GameObject result = m_InstancedPrefabs[indexOfUnusedPrefab];
			m_InstancedPrefabs[indexOfUnusedPrefab] = null;
			return result;
		}

		public GameObject GetPrefabInstanceFromPool(Vector3 pos, Quaternion rot)
		{
			GameObject prefabInstanceFromPool = GetPrefabInstanceFromPool();
			prefabInstanceFromPool.transform.position = pos;
			prefabInstanceFromPool.transform.rotation = rot;
			prefabInstanceFromPool.transform.localScale = new Vector3(1f, 1f, 1f);
			return prefabInstanceFromPool;
		}

		public void ReturnPrefabInstanceToPool(GameObject unpooledObject)
		{
			int num = 0;
			for (int i = 0; i < m_NumberInPool; i++)
			{
				if (!m_InOrOut[i])
				{
					num = i;
					break;
				}
			}
			if (num <= m_InstancedPrefabs.Count)
			{
				m_InstancedPrefabs[num] = unpooledObject;
				m_InOrOut[num] = true;
			}
		}
	}
}
