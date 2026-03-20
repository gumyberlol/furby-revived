using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum InAppConfirmationEvent
	{
		ShowConfirmation = 0,
		HideConfirmation = 1,
		PurchaseConfirmed = 2,
		PurchaseDeclined = 3
	}
}
