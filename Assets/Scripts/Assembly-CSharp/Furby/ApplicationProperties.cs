using Relentless;
using UnityEngine;

namespace Furby
{
	public class ApplicationProperties
	{
		public string m_OSVersion;

		public Relentless.Platforms m_ServerPlatform;

		public ProfileFriendlyName m_ProfileFriendlyName;

		public ProfileType m_ProfileType;

		public Relentless.Resolution m_Resolution;

		public string m_GameVersion;

		public string m_SysInfo;

		public SystemLanguage m_Language;

		public string m_LanguageCode;

		public void CacheApplicationProperties()
		{
			m_OSVersion = NetworkHelper.OSVer;
			m_ServerPlatform = NetworkHelper.ServerPlatform;
			m_ProfileFriendlyName = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName;
			m_ProfileType = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType;
			m_Resolution = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution;
			m_GameVersion = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleVersion;
			m_SysInfo = NetworkHelper.GetSysInfo();
		}
	}
}
