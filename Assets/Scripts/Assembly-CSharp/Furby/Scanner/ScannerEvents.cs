using Relentless;

namespace Furby.Scanner
{
	[GameEventEnum]
	public enum ScannerEvents
	{
		ScanButtonPressed = 0,
		ScanningSucceeded = 1,
		ScanningFailed = 2,
		ScanningStarted = 3,
		RequiresInitialScan = 4,
		InitialScanComplete = 5,
		InitialScanFailed = 6,
		PlayWithoutFurbyTemporarily = 7,
		GoBackToModeChoice = 8,
		InitialScanCompleteNamingNotRequired = 9,
		IncorrectFurbyFound = 10,
		NoFurbyFound = 11,
		FirstScanDisableInput = 12,
		OldFurbyFound = 13,
		ScanningCancelled = 14,
		GoBackToSettingsScreen = 15
	}
}
