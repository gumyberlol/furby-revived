using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricPrefabDirectLoader")]
	public class FabricPrefabDirectLoader : RelentlessMonoBehaviour
	{
		public GameObject m_asset;

		public FabricHierarchyReference m_targetComponent;

		public bool m_unloadWhenDestroyed = true;

		private GameObject m_instantiatedAsset;

		private void Awake()
		{
			if (!(m_asset != null))
			{
				return;
			}
			FabricManager instance = FabricManager.Instance;
			if (instance != null)
			{
				GroupComponent[] componentsInChildren = instance.GetComponentsInChildren<GroupComponent>();
				if (componentsInChildren.Length > 0)
				{
					m_instantiatedAsset = (GameObject)Object.Instantiate(m_asset);
					instance.LoadAsset(m_instantiatedAsset, m_targetComponent.Path);
				}
				ModifiedVolumeMeter[] componentsInChildren2 = instance.GetComponentsInChildren<ModifiedVolumeMeter>();
				ModifiedVolumeMeter[] array = componentsInChildren2;
				foreach (ModifiedVolumeMeter modifiedVolumeMeter in array)
				{
					modifiedVolumeMeter.CollectAudioSources();
				}
			}
		}

		private void OnDestroy()
		{
			if (m_instantiatedAsset != null && m_unloadWhenDestroyed)
			{
				Fabric.Component component = m_instantiatedAsset.GetComponent<Fabric.Component>();
				component.Stop(true, true, true);
				FabricManager.Instance.UnloadAsset(m_targetComponent.Path + "_" + m_asset.name);
			}
		}
	}
}
