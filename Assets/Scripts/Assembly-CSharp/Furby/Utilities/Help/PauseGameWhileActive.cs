using UnityEngine;

namespace Furby.Utilities.Help
{
	public class PauseGameWhileActive : MonoBehaviour
	{
		[SerializeField]
		private float m_timeScale;

		private float m_originalTimeScale;

		private void OnEnable()
		{
			m_originalTimeScale = Time.timeScale;
			Time.timeScale = m_timeScale;
		}

		private void OnDisable()
		{
			Time.timeScale = m_originalTimeScale;
		}
	}
}
