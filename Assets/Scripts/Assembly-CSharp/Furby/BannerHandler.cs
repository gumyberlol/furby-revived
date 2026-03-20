using UnityEngine;

namespace Furby
{
	public class BannerHandler : MonoBehaviour
	{
		[SerializeField]
		public GameObject m_LeaveAppConfirmationPrefab;

		[SerializeField]
		private Transform m_DialogSpawnPoint;

		[SerializeField]
		private string m_URL = string.Empty;

		public string URL
		{
			get
			{
				return m_URL;
			}
			set
			{
				m_URL = value;
			}
		}

		private void OnClick()
		{
			if (!string.IsNullOrEmpty(m_URL))
			{
				GameObject gameObject = Object.Instantiate(m_LeaveAppConfirmationPrefab) as GameObject;
				Transform transform = gameObject.transform;
				transform.parent = m_DialogSpawnPoint;
				transform.localPosition = Vector3.zero;
				transform.localScale = Vector3.one;
				transform.gameObject.layer = base.gameObject.layer;
				LeavingAppConfirmationHandler component = gameObject.GetComponent<LeavingAppConfirmationHandler>();
				component.m_PayloadTarget.m_URL = m_URL;
			}
		}
	}
}
