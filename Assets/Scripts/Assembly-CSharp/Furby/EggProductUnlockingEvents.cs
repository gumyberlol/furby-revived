using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum EggProductUnlockingEvents
	{
		ScanningForQRCode = 0,
		FoundValidQRCode = 1,
		SwirlVFXSequenceStarted = 2,
		SwirlVFXSequenceEnded = 3,
		YouHaveUnlockedSequenceStarted = 4,
		YouHaveUnlockedSequenceEnded = 5,
		UnlockChoiceOffered = 6,
		UnlockChoiceChosen = 7,
		UnlockChoiceNotOffered = 8
	}
}
