using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurbyModeChoice
	{
		PlayWithFurby = 0,
		PlayWithNoFurby = 1,
		BackButtonClicked = 2,
		SettingsButtonClicked = 3,
		GoToScanningScreen = 4,
		GoToDashboard = 5,
		GoToGlobalSettings = 6,
		GoToTitlePage = 7,
		GoToSaveSlotSelect = 8,
		DashboardArrival_Furby = 9,
		DashboardArrival_NoFurby = 10,
		DashboardArrival_Conversion = 11
	}
}
