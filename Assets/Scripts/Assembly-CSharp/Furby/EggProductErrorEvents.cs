using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum EggProductErrorEvents
	{
		TimeoutFailedToRead = 0,
		UnrecognizedQRCode = 1,
		AlreadyUsedQRCode = 2
	}
}
