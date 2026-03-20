using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class InstanceRegister : MonoBehaviour
	{
		private GameObject m_InstanceReference;

		private string m_AssetBundleName = string.Empty;

		public void RegisterInstance(GameObject prefabInstance, string assetBundleName)
		{
			Logging.Log("InstanceRegister (" + base.name + ") Register -> " + prefabInstance.name);
			DeregisterInstance();
			m_InstanceReference = prefabInstance;
			m_AssetBundleName = assetBundleName;
		}

		public void DeregisterInstance()
		{
			if (m_InstanceReference != null)
			{
				Object.Destroy(m_InstanceReference);
				Resources.UnloadUnusedAssets();
			}
		}

		public bool AlreadyRegistered(string assetBundleName)
		{
			if (m_InstanceReference != null && !string.IsNullOrEmpty(m_AssetBundleName))
			{
				return m_AssetBundleName.Equals(assetBundleName);
			}
			return false;
		}
	}
}
