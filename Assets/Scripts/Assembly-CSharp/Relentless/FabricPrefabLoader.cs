using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricPrefabLoader")]
	public class FabricPrefabLoader : RelentlessMonoBehaviour
	{
		public string assetPath;

		public string targetComponent = string.Empty;

		private void Awake()
		{
			string[] array = assetPath.Split('/');
			int length = array.GetLength(0);
			if (length <= 1)
			{
				return;
			}
			string prefabPath = targetComponent + "_" + array[length - 1];
			if (!FabricPrefabManager.Instance.IsPrefabLoaded(prefabPath))
			{
				if (assetPath != string.Empty && targetComponent != string.Empty)
				{
					FabricManager instance = FabricManager.Instance;
					if (instance != null)
					{
						instance.LoadAsset(assetPath, targetComponent);
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

		private void Update()
		{
		}
	}
}
