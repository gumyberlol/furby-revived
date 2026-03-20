using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class DisplayMusicGameScore : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_labelToSet;

		[SerializeField]
		private int m_multiplier = 10;

		private MusicGameScoring m_scoring;

		private void Start()
		{
			m_scoring = (MusicGameScoring)Object.FindObjectOfType(typeof(MusicGameScoring));
		}

		private void Update()
		{
			m_labelToSet.text = (m_scoring.GetScore() * m_multiplier).ToString();
		}
	}
}
