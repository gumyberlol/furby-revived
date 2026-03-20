using Relentless;
using UnityEngine;

namespace Furby
{
	public class InGameStateReceiverEnabler : RelentlessMonoBehaviour
	{
		public GameObject m_target;

		private bool m_wasActive;

		private void OnIntroState()
		{
			m_target.SetActive(false);
		}

		private void OnStartPlayingState()
		{
			m_target.SetActive(true);
		}

		private void OnEndPlayingState()
		{
			m_target.SetActive(false);
		}

		private void OnGamePause()
		{
			m_wasActive = m_target.activeSelf;
			m_target.SetActive(false);
		}

		private void OnGameUnPause()
		{
			m_target.SetActive(m_wasActive);
		}
	}
}
