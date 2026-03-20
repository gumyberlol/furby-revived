using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricPrefabPlaySound")]
	public class FabricPrefabPlaySound : RelentlessMonoBehaviour
	{
		public string m_eventName;

		private void OnEnable()
		{
			EventManager.Instance.PostEvent(m_eventName, EventAction.PlaySound, null, base.gameObject);
		}

		private void Update()
		{
		}
	}
}
