using Relentless;

namespace Furby.EggTransfer
{
	[GameEventEnum]
	public enum EggTransferGameEvent
	{
		EggTransferOpened = 0,
		EggTransferClosed = 1,
		EggTransferStartSync = 2,
		EggTransferFinishSync = 3,
		EggTransferStartTransfer = 4,
		EggTransferFinishTransfer = 5,
		EggCartonFull = 6,
		EggTransferIncorrectFurbyInFriendMode = 7,
		EggTransferIncorrectFurbyInMyFurbyMode = 8,
		EggTransferLookingForFriendEgg = 9,
		EggTransferLookingForMyFurbyEgg = 10,
		EggTransferFriendEggSentTooRecently = 11,
		EggTransferNoEggsReady = 12,
		EggTransferNoFurbyFound = 13,
		EggTransferLookingForFriendAdviceClosed = 14
	}
}
