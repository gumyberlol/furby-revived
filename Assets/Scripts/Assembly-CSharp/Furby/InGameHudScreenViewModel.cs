using Relentless;
using UnityEngine;

namespace Furby
{
	public class InGameHudScreenViewModel : RelentlessMonoBehaviour, IScreenViewModel
	{
		public bool m_AlwaysActive;

		private int m_playerScore;

		private string m_playerName;

		public void OnShow()
		{
			SetChildLabelText("PlayerName", FurbyGlobals.Player.FullName);
		}

		public void OnExit()
		{
		}

		public void OnHide()
		{
			if (m_AlwaysActive)
			{
				base.gameObject.SetActive(true);
			}
		}

		public void Start()
		{
		}

		public void OnPlayerScoreChanged(int score)
		{
			m_playerScore = score;
			UpdateScore();
		}

		private void UpdateScore()
		{
			SetChildLabelText("ScoreCount", m_playerScore.ToString());
		}

		private void SetChildLabelText(string gameObjectName, string newText)
		{
			UILabel labelOn = GetLabelOn(gameObjectName);
			if (!(labelOn == null))
			{
				labelOn.text = newText;
			}
		}

		private UILabel GetLabelOn(string nameOfGameObject)
		{
			GameObject childGameObject = base.gameObject.GetChildGameObject(nameOfGameObject);
			if (childGameObject == null)
			{
				Logging.LogError("Failed to get child game object " + nameOfGameObject + " of " + base.gameObject);
				return null;
			}
			UILabel component = childGameObject.GetComponent<UILabel>();
			if (component == null)
			{
				Logging.LogError("Failed to get UILabel component on game object " + nameOfGameObject);
				return null;
			}
			return component;
		}
	}
}
