using Relentless;

namespace Furby.Utilities.VoiceChanger
{
	[GameEventEnum]
	public enum VoiceChangerEvent
	{
		VoiceChangerUtilityStarted = 0,
		PotionItemClicked = 1,
		PotionItemSelected = 2,
		FurbyBoredOfWaitingForPotion = 3,
		PotionGiven = 4,
		PotionBounced = 5,
		PotionFlicked = 6,
		PotionTimedOut = 7,
		FurbyReactingToPotion = 8,
		CommunicationsError = 9,
		NoFurby = 10,
		ResetButtonClicked = 11,
		ErrorDialogOK = 12
	}
}
