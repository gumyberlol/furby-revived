using Relentless;

namespace Furby.Utilities.PoopStation
{
	[GameEventEnum]
	public enum PoopStationEvent
	{
		SprayPressed = 0,
		SprayReleased = 1,
		PoopPrepared = 2,
		LidActivated = 3,
		LidLiftCompleted = 4,
		MoveToToyStarted = 5,
		MoveToToyCompleted = 6,
		ReadyForPoop = 7,
		PoopCreated = 8,
		MoveFromToy = 9,
		FlushPressed = 10,
		FlushCompleted = 11,
		SmellSpreading = 12,
		SmellDefeated = 13,
		LidDownStarted = 14,
		LidDownCompleted = 15,
		RollPulled = 16
	}
}
