using UnityEngine;

namespace Relentless.Core.DesignPatterns.ProviderManager
{
	public static class ProviderDevicesHelper
	{
		public static ProviderDevices GetDevice()
		{
			if (Application.isEditor)
			{
				return ProviderDevices.Editor;
			}
			if (string.IsNullOrEmpty(SystemInfo.deviceModel))
			{
				return ProviderDevices.Any;
			}
			string text = SystemInfo.deviceModel.ToLower();
			if (text.Contains("kindle") || text.Contains("KFTT") || text.Contains("amazon"))
			{
				return ProviderDevices.Kindle;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				return ProviderDevices.AndroidOther;
			}
			return ProviderDevices.Any;
		}

		public static bool IsDeviceValid(ProviderDevices currentDevice, ProviderDevices deviceToCheck)
		{
			if (deviceToCheck == ProviderDevices.Any)
			{
				return true;
			}
			return currentDevice == deviceToCheck;
		}
	}
}
