using UnityEngine;

namespace Furby
{
	public class EndGameScoringPanel_RemovePlayAgain : MonoBehaviour
	{
		[SerializeField]
		private SpsBackToScreen m_playRoomButton;

		[SerializeField]
		private BabyEndMinigameFlow m_endGame;

		private BabyEndMinigameFlow.SectionShowHandler m_handler;

		private EndGameScoringPanel_RemovePlayAgain()
		{
			m_handler = delegate(GameObject panelRoot)
			{
				foreach (Transform item in panelRoot.transform)
				{
					if (item != m_playRoomButton.transform)
					{
						Object.Destroy(item.gameObject);
					}
				}
				m_playRoomButton.transform.localPosition = Vector3.zero;
			};
		}

		public void OnEnable()
		{
			m_endGame.ShowingPlayAgain += m_handler;
		}

		public void OnDisable()
		{
			m_endGame.ShowingPlayAgain -= m_handler;
		}
	}
}
