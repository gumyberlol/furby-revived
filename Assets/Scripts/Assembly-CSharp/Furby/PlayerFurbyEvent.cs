using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum PlayerFurbyEvent
	{
		StatusUpdated = 0,
		NoFurbyModeActivated = 1,
		FurbyModeActivated = 2,
		AdultLevelGained = 16,
		AdultEggReady = 17,
		AdultNewVirtualFriendUnlocked = 18,
		AdultGainedXP = 19,
		AdultNewMinigameUnlocked = 20,
		AdultIsHungry = 32,
		AdultIsUnhappy = 33,
		AdultIsDirty = 34,
		AdultNeedsToilet = 35,
		EggNeedsAttention = 48,
		BabyNeedsAttention = 64,
		SpecialFullMoon = 80,
		BabyEarnedXP = 96,
		BabyGainedXP = 97,
		BabyGraduated = 98
	}
}
