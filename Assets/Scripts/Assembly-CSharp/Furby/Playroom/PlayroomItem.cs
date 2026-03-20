using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomItem
	{
		public string m_AssetBundleName;

		private GameObject m_InstancedItem;

		public void InstanceItemIntoScene(GameObject prefab, GameObject targetRoot)
		{
			if (!(prefab != null))
			{
				return;
			}
			InstanceRegister instanceRegister = (InstanceRegister)targetRoot.GetComponent("InstanceRegister");
			if (!instanceRegister.AlreadyRegistered(m_AssetBundleName))
			{
				instanceRegister.DeregisterInstance();
				m_InstancedItem = (GameObject)UnityEngine.Object.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
				m_InstancedItem.SetActive(true);
				m_InstancedItem.transform.parent = targetRoot.transform;
				m_InstancedItem.transform.Rotate(new Vector3(0f, 180f, 0f));
				m_InstancedItem.name = prefab.name;
				m_InstancedItem.layer = targetRoot.layer;
				Transform[] componentsInChildren = m_InstancedItem.GetComponentsInChildren<Transform>();
				Transform[] array = componentsInChildren;
				foreach (Transform transform in array)
				{
					transform.gameObject.layer = targetRoot.layer;
				}
				instanceRegister.RegisterInstance(m_InstancedItem, m_AssetBundleName);
			}
		}

		public void Clear()
		{
			if ((bool)m_InstancedItem)
			{
				UnityEngine.Object.Destroy(m_InstancedItem);
				Resources.UnloadUnusedAssets();
			}
		}
	}
}
