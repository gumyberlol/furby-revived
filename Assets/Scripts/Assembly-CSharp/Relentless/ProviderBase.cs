using Relentless.Core.DesignPatterns.ProviderManager;
using UnityEngine;

namespace Relentless
{
	public abstract class ProviderBase : RelentlessMonoBehaviour
	{
		public RuntimePlatform[] Platforms;

		public ProviderPriority ProviderPriority;

		public ProfileFriendlyName ProfileFriendlyName;

		public ProfileType ProfileType;

		public Resolution Resolution;

		public ProviderDevices[] ProviderDevices;

		protected abstract string ProviderName { get; }

		public virtual void Initialise()
		{
		}

		public bool IsValid()
		{
			return IsValid(true);
		}

		public bool IsValid(bool printReason)
		{
			if (!base.enabled)
			{
				if (printReason)
				{
					Logging.Log("Skipping provider " + ProviderName + " as it's not enabled");
				}
				return false;
			}
			bool flag = false;
			RuntimePlatform[] platforms = Platforms;
			foreach (RuntimePlatform runtimePlatform in platforms)
			{
				if (Application.platform == runtimePlatform)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (printReason)
				{
					Logging.Log("Skipping provider " + ProviderName + " as it's not enabled for platform " + Application.platform);
				}
				return false;
			}
			ProviderDevices device = ProviderDevicesHelper.GetDevice();
			bool flag2 = false;
			if (ProviderDevices == null || ProviderDevices.Length == 0)
			{
				flag2 = true;
			}
			else
			{
				ProviderDevices[] providerDevices = ProviderDevices;
				foreach (ProviderDevices deviceToCheck in providerDevices)
				{
					if (ProviderDevicesHelper.IsDeviceValid(device, deviceToCheck))
					{
						flag2 = true;
					}
				}
			}
			if (!flag2)
			{
				flag = false;
				if (printReason)
				{
					Logging.Log("Skipping provider " + ProviderName + " as it's not enabled for device " + device);
					return false;
				}
			}
			if (SingletonInstance<ApplicationSettingsBehaviour>.Exists)
			{
				if (ProfileFriendlyName != ProfileFriendlyName.None && SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName != ProfileFriendlyName.None && ProfileFriendlyName != SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName)
				{
					if (printReason)
					{
						Logging.LogWarning(string.Concat("Skipping provider ", ProviderName, " because ProfileFriendlyName does not match this build (", ProfileFriendlyName, ")"));
						return false;
					}
				}
				else if (ProfileType != ProfileType.None && SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType != ProfileType.None && ProfileType != SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType)
				{
					if (printReason)
					{
						Logging.LogWarning(string.Concat("Skipping provider ", ProviderName, " because ProfileType does not match this build (", ProfileType, ")"));
						return false;
					}
				}
				else if (Resolution != Resolution.None && SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution != Resolution.None && Resolution != SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution && printReason)
				{
					Logging.LogWarning(string.Concat("Skipping provider ", ProviderName, " because Profile Resolution does not match this build (", Resolution, ")"));
					return false;
				}
			}
			return true;
		}
	}
}
