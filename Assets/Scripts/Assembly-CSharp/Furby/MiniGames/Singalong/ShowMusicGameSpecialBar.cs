using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class ShowMusicGameSpecialBar : MonoBehaviour
	{
		[SerializeField]
		private UISlider m_slider;

		private MusicGameScoring m_scoring;

		private DiscoModePlayer m_discoModePlayer;

		private void Start()
		{
			m_scoring = (MusicGameScoring)Object.FindObjectOfType(typeof(MusicGameScoring));
			m_discoModePlayer = (DiscoModePlayer)Object.FindObjectOfType(typeof(DiscoModePlayer));
			if (m_slider == null)
			{
				m_slider = GetComponent<UISlider>();
			}
		}

		private void Update()
		{
			if (m_discoModePlayer.IsDiscoModeActive())
			{
				m_slider.sliderValue = m_discoModePlayer.GetDiscoTimeRemainingT();
			}
			else
			{
				m_slider.sliderValue = m_scoring.GetBarT();
			}
		}
	}
}
