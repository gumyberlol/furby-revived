using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum HardwareSettingsEvents
	{
		HeadphonesArePluggedIn = 0,
		HeadphonesAreNOTPluggedIn = 1,
		HardwareVolumeIsTooLow = 2,
		HardwareVolumeIsOK = 3,
		ShowHeadphonesDialog = 4,
		HideHeadphonesDialog = 5,
		ShowTheVolumeMeters = 6,
		HideTheVolumeMeters = 7
	}
}
