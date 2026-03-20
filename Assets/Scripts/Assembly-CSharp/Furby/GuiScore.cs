using UnityEngine;

namespace Furby
{
	public class GuiScore : MonoBehaviour
	{
		public int Score;

		public UILabel ScoreLabel;

		private void Start()
		{
			Score = 0;
		}

		public void setScore(string score)
		{
			ScoreLabel.text = score;
		}

		private void ResetScore()
		{
			Score = 0;
		}
	}
}
