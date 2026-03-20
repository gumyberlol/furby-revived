using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum InGamePurchaseableEvent
	{
		PurchaseButtonClickedEnoughFurbucks = 0,
		PurchaseConfirmed = 1,
		PurchaseCancelled = 2,
		PurchaseButtonClickedNotEnoughFurbucks = 3
	}
}
