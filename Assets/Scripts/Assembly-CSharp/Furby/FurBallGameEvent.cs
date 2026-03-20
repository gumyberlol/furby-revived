using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurBallGameEvent
	{
		FurballStartPlayingFurbyMode = 0,
		FurballStartPlayingFurblingMode = 1,
		FurballSelectFurbyGameMode = 2,
		FurballSelectFurblingGameMode = 3,
		FurballShowRotateDeviceDialog = 4,
		FurballEnter = 5,
		FurballGoalScored = 6,
		FurballBallDeflected = 7,
		FurbyModeEnded = 8,
		FurballChooseBackToModeSelect = 9,
		FurbyModeTiltLeft = 10,
		FurbyModeTiltRight = 11,
		FurballStartBallRound = 12,
		FurballBallShotsEnded = 13,
		FurballReadyToShootBall = 14,
		FurballGameFinished = 15,
		FurballBallStart = 16,
		FurballShowReturnDeviceDialog = 17,
		FurballHideRotateReturnDialog = 18,
		FurballFurblingKicksBall = 19,
		FurballBouncerHitsBall = 20,
		FurballQuitRequested = 21,
		FurballOpponentScoresGoal = 22,
		FurballOpponentMissesGoal = 23,
		FurballPlayerScoresGoal = 24,
		FurballPlayerMissesGoal = 25,
		FurballFurbyModeGoodResult = 26,
		FurballFurbyModeOKResult = 27,
		FurballFurbyModeBadResult = 28,
		FurballFurblingModeGoodResult = 29,
		FurballFurblingModeOKResult = 30,
		FurballFurblingModeBadResult = 31,
		FurballOpponentFurblingKicksBall = 32,
		FurballPlayerFurblingKicksBall = 33,
		FurballFurblingModeGoodRound = 34,
		FurballFurblingModeBadRound = 35,
		FurballFurbyModeGoodRound = 36,
		FurballFurbyModeBadRound = 37,
		FurballFurblingModeActivateStartRound = 38,
		FurballFurblingModeStartRoundDelay = 39
	}
}
