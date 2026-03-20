using UnityEngine;

namespace Furby
{
	public class BuyNowButton : MonoBehaviour
	{
		[SerializeField]
		private BuyNowLeavingAppDialog m_leavingAppDialogPrefab;

		[SerializeField]
		private Transform m_dialogSpawnPoint;

		public void OnClick()
		{
			BuyNowLeavingAppDialog buyNowLeavingAppDialog = Object.Instantiate(m_leavingAppDialogPrefab) as BuyNowLeavingAppDialog;
			Transform transform = buyNowLeavingAppDialog.transform;
			transform.parent = m_dialogSpawnPoint;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			buyNowLeavingAppDialog.gameObject.layer = m_dialogSpawnPoint.gameObject.layer;
		}
	}
}
