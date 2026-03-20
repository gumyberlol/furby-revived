using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricPrefabResourceLoader")]
	public class FabricPrefabResourceLoader : RelentlessMonoBehaviour
	{
		public ResourcesReference m_assetPath;

		public FabricHierarchyReference m_targetComponent;

		private void Awake()
		{
			string[] array = m_assetPath.Path.Split('/');
			int length = array.GetLength(0);
			if (length <= 1)
			{
				return;
			}
			string prefabPath = m_targetComponent.Path + "_" + array[length - 1];
			if (!FabricPrefabManager.Instance.IsPrefabLoaded(prefabPath))
			{
				if (Debug.isDebugBuild && Resources.Load(m_assetPath.Path) == null)
				{
					Logging.LogError("Audio: Could not find Audio Prefab: " + m_assetPath);
				}
				if (!(m_assetPath.Path != string.Empty) || !(m_targetComponent.Path != string.Empty))
				{
					return;
				}
				FabricManager instance = FabricManager.Instance;
				if (instance != null)
				{
					GroupComponent[] componentsInChildren = instance.GetComponentsInChildren<GroupComponent>();
					if (componentsInChildren.Length > 0)
					{
						instance.LoadAsset(m_assetPath.Path, m_targetComponent.Path);
						FabricPrefabManager.Instance.AddPrefab(prefabPath);
						base.enabled = false;
					}
				}
			}
			else
			{
				base.enabled = false;
			}
		}
	}
}
