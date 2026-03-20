using UnityEngine;

namespace Relentless
{
	public class HideOnStart : MonoBehaviour
	{
		public bool Hide = true;

		private bool m_hide = true;

		private void LateUpdate()
		{
			if (Hide && m_hide)
			{
				base.gameObject.SetActive(false);
				m_hide = false;
			}
		}
	}
}
