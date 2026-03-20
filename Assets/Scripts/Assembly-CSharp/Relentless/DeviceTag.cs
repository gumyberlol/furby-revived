using Relentless.Core.Crypto;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public class DeviceTag : RelentlessMonoBehaviour
	{
		public Rect m_labelPosition;

		private void OnGUI()
		{
			string deviceId = DeviceIdManager.DeviceId;
			string text = string.Format("DeviceId: {0} ({1})", deviceId, Hash.ComputeHash(deviceId, Hash.Algorithm.SHA256, "DeviceId"));
			GUI.Label(m_labelPosition, text);
		}
	}
}
