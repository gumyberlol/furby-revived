using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum ShopGameEvents
	{
		ShopTypeSelected = 0,
		ShopItemSelectedPurchase = 1,
		ShopItemSelectedTooCostly = 2,
		ShopItemConfirmPurchase = 3,
		ShopItemCancelPurchase = 4,
		ShopItemsUpdated = 5,
		ShopRealMoneyShopUnavailable = 6,
		ShopRealMoneyAttemptPurchase = 7,
		ShopRealMoneyProcessing = 8,
		ShopRealMoneyProcessComplete = 9,
		ShopRealMoneyProcessCompleteSuccess = 10,
		ShopRealMoneyProcessCompleteFailure = 11,
		EggBought = 12
	}
}
