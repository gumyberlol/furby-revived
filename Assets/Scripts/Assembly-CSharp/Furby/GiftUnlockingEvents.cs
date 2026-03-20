using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum GiftUnlockingEvents
	{
		Gifting_ChooseGift_WaitForGiftToBeChosen = 0,
		Gifting_ChooseGift_Chosen = 1,
		Gifting_PayToUnwrapGift_Confirm = 2,
		Gifting_PayToUnwrapGift_Decline = 3,
		Gifting_PayToUnwrapGift_BuyFurbyBoom = 4,
		Gifting_Interaction_TimedOut = 5,
		Gifting_Interaction_BouncedOffWall = 6,
		Gifting_Interaction_ItemSentToFurby = 7,
		Gifting_Interaction_TonesAcceptedByFurby = 8,
		Gifting_Interaction_Cancelled = 9,
		Gifting_Interaction_Flicked = 10,
		Gifting_BurstScreen_SequenceStarted = 11,
		Gifting_BurstScreen_SequenceCompleted = 12,
		Gifting_BurstScreen_BackToGiftSelect = 13
	}
}
