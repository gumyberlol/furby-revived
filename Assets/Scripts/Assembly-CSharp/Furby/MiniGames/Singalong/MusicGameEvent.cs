using Relentless;

namespace Furby.MiniGames.Singalong
{
	[GameEventEnum]
	public enum MusicGameEvent
	{
		Enter = 0,
		Exit = 1,
		EnterPause = 2,
		ExitPause = 3,
		SongSelected = 4,
		SongStartedRock = 5,
		SongStartedDance = 6,
		SongStartedDisco = 7,
		SongStarted = 8,
		NotePressed = 9,
		NoteHit = 10,
		NoteMissed = 11,
		DiscoModeBegin = 12,
		DiscoModeEnd = 13,
		SongFinished = 14,
		GameEnded = 15,
		ReactionPositive = 16,
		ReactionNeutral = 17,
		ReactionNegative = 18,
		Restart = 19,
		HiEnergyDanceBegin = 20,
		HiEnergyDanceEnd = 21,
		RequestDiscoMode = 22,
		DiscoModeAvailable = 23,
		NoteMissedTapped = 24,
		NoteMissedExpired = 25,
		DiscoModeOfferExpired = 26,
		NoteMissedTappedNotSpecialMode = 27,
		NoteMissedExpiredNotSpecialMode = 28,
		SpecialModeNoteHitDisco = 29,
		SpecialModeNoteHitDance = 30,
		SpecialModeNoteHitRock = 31,
		FurbyNotFound = 32
	}
}
