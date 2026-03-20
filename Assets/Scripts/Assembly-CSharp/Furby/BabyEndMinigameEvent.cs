using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum BabyEndMinigameEvent
	{
		ShowDialog = 0,
		HideDialog = 1,
		SetScore = 16,
		ReturnToPlayroom = 32,
		PlayAgain = 33,
		DialogFinished = 48,
		VideoThenReturnToPlayroom = 64,
		VideoThenPlayAgain = 65,
		ShowScoreLabel = 80,
		ShowScoreIncrease = 81,
		ShowScoreNoIncrease = 82,
		ShowStarLabel = 96,
		ShowStar1 = 97,
		ShowStar2 = 98,
		ShowStar3 = 99,
		ShowMoneyLabel = 112,
		ShowMoneyIncrease = 113,
		ShowXpLabel = 128,
		ShowXpIncrease = 129,
		ShowXpComplete = 130,
		ReCheckStartingFurbucks = 144
	}
}
