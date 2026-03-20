using UnityEngine;

namespace Furby
{
	public class EggTransferCreateSettings : MonoBehaviour
	{
		[SerializeField]
		private bool m_friendMode;

		private void OnClick()
		{
			if (base.enabled)
			{
				GameObject gameObject = new GameObject("SETTING_TRANSFER_MODE");
				EggTransferSettings eggTransferSettings = gameObject.AddComponent<EggTransferSettings>();
				eggTransferSettings.m_transferringFriendsEgg = m_friendMode;
				Object.DontDestroyOnLoad(gameObject);
			}
		}
	}
}
