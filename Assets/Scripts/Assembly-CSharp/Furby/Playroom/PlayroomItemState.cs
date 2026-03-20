using System;

namespace Furby.Playroom
{
	[Serializable]
	public enum PlayroomItemState
	{
		AvailableFromTheStart = 0,
		BuyableInShop = 1,
		UnlockedByGoldenFurby = 2,
		UnlockedByGoldenFurbyOrComAir = 3,
		UnlockedByScannedQRCode = 4,
		UnlockedByComAirTone = 5,
		UnlockedAsSeasonalTheme = 6,
		UnlockedAsGift = 7,
		UnlockedAsCrystal = 8,
		UnlockedAsGoldenCrystal = 9
	}
}
