using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurballDisplayScoreInfo : RelentlessMonoBehaviour
	{
		[SerializeField]
		private int m_scoreMultiplier = 10;

		[SerializeField]
		private UILabel m_scoreLabel;

		[SerializeField]
		private UILabel m_roundsRemainingLabel;

		private FurBallCurrentGame m_currentGameInfo;

		private void Start()
		{
			m_currentGameInfo = (FurBallCurrentGame)Object.FindObjectOfType(typeof(FurBallCurrentGame));
		}

		private void Update()
		{
			if (m_scoreLabel != null)
			{
				m_scoreLabel.text = (m_currentGameInfo.GetScore() * m_scoreMultiplier).ToString();
			}
			if (m_roundsRemainingLabel != null)
			{
				m_roundsRemainingLabel.text = m_currentGameInfo.GetTurnsRemaining().ToString();
			}
		}
	}
}
