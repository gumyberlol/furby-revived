using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurblingProductUnlockingEvents
	{
		WaitingForComAirTone = 0,
		ReceivedComAirTone_ValidForUnlocking = 1,
		YouHaveUnlockedSequenceStarted = 2,
		YouHaveUnlockedSequenceEnded = 3
	}
}
