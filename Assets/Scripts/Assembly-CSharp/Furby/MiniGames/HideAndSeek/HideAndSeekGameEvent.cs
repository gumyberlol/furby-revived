using Relentless;

namespace Furby.MiniGames.HideAndSeek
{
	[GameEventEnum]
	public enum HideAndSeekGameEvent
	{
		Enter = 0,
		Exit = 1,
		BalloonPopped = 2,
		TurnStarted = 3,
		TurnsReduced = 4,
		BallonPoppedSpecial = 5,
		ExtraTurnAdded = 6,
		ExtraTurnLost = 7,
		BalloonPopChainStarted = 8,
		BabyMoved = 9,
		OutOfTurns = 10,
		BabiesFound = 11,
		BabyReaction = 12,
		ScoreShown = 13,
		RoundOver = 32,
		RoundStarted = 33,
		RoundAnnounced = 34,
		RoundSetup = 35,
		RoundSetupComplete = 36,
		HitDistanceFound = 48,
		HitDistanceVeryHot = 49,
		HitDistanceHot = 50,
		HitDistanceWarm = 51,
		HitDistanceCold = 52,
		HitDistanceFreezing = 53,
		SpeechBubbleShown = 64,
		SpeechBubbleHidden = 65,
		TurnsChanged = 80,
		ScoreChanged = 81,
		BallonPopSequenceFinished = 96,
		GameWon = 112,
		GameLost = 113,
		ShortTimePassed = 128,
		LongTimePassed = 129,
		HitLastTurn = 144
	}
}
