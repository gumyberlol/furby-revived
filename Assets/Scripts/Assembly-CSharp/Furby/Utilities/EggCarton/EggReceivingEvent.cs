using Relentless;

namespace Furby.Utilities.EggCarton
{
	[GameEventEnum]
	public enum EggReceivingEvent
	{
		ReceivedInitiateTransferMessage = 0,
		ReceivedEggDetails = 1,
		EggCommunicationsTimedOut = 2,
		EggDialogGenericAccept = 3,
		EggDialogGenericCancel = 4,
		EggCartonFull = 5,
		ReceivedHandshakeReply = 6
	}
}
