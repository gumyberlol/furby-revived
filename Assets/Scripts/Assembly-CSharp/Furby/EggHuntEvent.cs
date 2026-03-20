using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum EggHuntEvent
	{
		Enter = 0,
		Exit = 1,
		CameraInitialised = 16,
		CameraError = 17,
		ButtonScan = 32,
		ButtonContinue = 33,
		ScanStarted = 48,
		ScanSucceeded = 49,
		ScanFailed = 50,
		ScanTimedOut = 51,
		ScanUnrecognisedCode = 52,
		ScanFakeScanStarted = 53,
		ScanFailedAlreadyHaveEgg = 54,
		ScanFailedCartonFull = 55,
		ScanFailedCameraNotFound = 56,
		ScannerHitsBottom = 64,
		ScannerHitsTop = 65,
		PresentationFinished = 80
	}
}
