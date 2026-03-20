using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum SettingsPageEvents
	{
		VolumeSliderReleased = 0,
		VolumeSliderValueChanged = 1,
		ShowRenamePrompt = 2,
		ConfirmRename = 3,
		GenericDialogAccept = 4,
		GenericDialogCancel = 5,
		ShowChangeFurbyPrompt = 6,
		ConfirmChangeFurby = 7,
		ShowUpgradePrompt = 8,
		ConfirmUpgrade = 9,
		ShowDowngradePrompt = 10,
		ConfirmDowngrade = 11,
		SwitchToHelpScreen = 12,
		SwitchToPreferencesScreen = 13,
		SwitchToManageGameScreen = 14,
		SwitchToPrivacyPolicyScreen = 15,
		SwitchToChangeUserScreen = 16,
		BackButtonClicked = 17,
		SwitchToDashboard = 18,
		DeleteABaby = 19,
		SwitchToNeighbourhood = 20,
		DeleteAnEgg = 21,
		SwitchToEggCarton = 22,
		ShowDeleteGamePrompt = 23,
		ConfirmDelete = 24,
		ChangeTheme = 25,
		RefreshFurbyStatuses = 26,
		SwitchToDeleteSaveGameScreen = 27,
		SwitchToAdvancedSettingsScreen = 28,
		SwitchToPreviousScreen = 29,
		ChangeLanguage = 30,
		SwitchToFurbyCommsScreen = 31,
		SwitchToScreenOrientationScreen = 32,
		CommsSliderValueChanged = 33,
		CommsSliderReleased = 34,
		TestFurbyComms = 35,
		SpeakerAtTop = 36,
		SpeakerAtBottom = 37,
		PrivacyPolicyButtonClicked = 38,
		ChangeUserButtonClicked = 39,
		ChangeLanguageButtonClicked = 40,
		ChangeCrystalTheme = 41,
		RestorePurchases = 42
	}
}
