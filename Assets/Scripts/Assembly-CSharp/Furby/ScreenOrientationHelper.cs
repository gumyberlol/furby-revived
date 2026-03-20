using Relentless;
using UnityEngine;

namespace Furby
{
	public static class ScreenOrientationHelper
	{
		public static ScreenOrientation GetReversedOrientation()
		{
			switch (Screen.orientation)
			{
			case ScreenOrientation.Portrait:
				return ScreenOrientation.PortraitUpsideDown;
			case ScreenOrientation.PortraitUpsideDown:
				return ScreenOrientation.Portrait;
			default:
				return Screen.orientation;
			}
		}

		public static ScreenOrientation GetCorrectOrientationForDevice()
		{
			return FurbyGlobals.DeviceSettings.GetScreenOrientationForDevice();
		}

		public static ScreenOrientation GetCorrectOrientationForDevice_OrCustomizedOverride()
		{
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.m_ScreenOrientation_UserCustomized)
			{
				return Singleton<GameDataStoreObject>.Instance.GlobalData.GetCustomizedScreenOrientation();
			}
			return FurbyGlobals.DeviceSettings.GetScreenOrientationForDevice();
		}

		public static ScreenOrientation GetCurrentOrientation()
		{
			return Screen.orientation;
		}
	}
}
