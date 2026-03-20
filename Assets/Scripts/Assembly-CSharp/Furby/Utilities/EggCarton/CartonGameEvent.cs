using Relentless;

namespace Furby.Utilities.EggCarton
{
	[GameEventEnum]
	public enum CartonGameEvent
	{
		EggSelectedToIncubate = 0,
		EggBouncesIntoLeftRow = 1,
		EggBouncesIntoRightRow = 2,
		EggClickedUpon = 3,
		GiftButtonClicked = 4,
		EggDialogGenericAccept = 5,
		EggDialogGenericDecline = 6,
		SendEggTimedOut = 7,
		EggDeclined = 8,
		EggReceived = 9,
		EggCannotBeIncubated = 10,
		ReceivedHandshake = 11,
		ReceivedHandshakeCode = 12,
		EggCartonStartedNoEggsAdded = 13,
		EggCartonStartedEggsWillBeAdded = 14,
		D2DTransferStarted = 15,
		D2DTransferFailed = 16,
		D2DTransferSucceeded = 17,
		D2DTransferStartedReceiver = 18,
		D2DTransferFailedReceiver = 19,
		D2DTransferSucceededReceiver = 20,
		EggWasDeleted = 21,
		EggsFinishedEntrancing = 22,
		EggDeleteCompleted = 23,
		EggGiftChosen = 24,
		EggGiftFailedDialogTryAgain = 25,
		EggGiftFailedDialogCancel = 26
	}
}
