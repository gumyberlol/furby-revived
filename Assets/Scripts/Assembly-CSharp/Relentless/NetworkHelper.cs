using System;
using System.Text;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public static class NetworkHelper
	{
		private const string HexDigits = "0123456789abcdef";

		public static string OSVer
		{
			get
			{
				Logging.Log("SystemInfo.operatingSystem = " + SystemInfo.operatingSystem);
				string empty = string.Empty;
				switch (ServerPlatform)
				{
				case Platforms.iOS:
					empty = GetIOSVersion();
					break;
				case Platforms.OSX:
					empty = GetMacOsXVersion();
					break;
				case Platforms.Android:
					empty = GetAndroidVersion();
					break;
				case Platforms.NaCl:
					empty = GetNaClVersion();
					break;
				case Platforms.WebPlayer:
					empty = GetWebPlayerVersion();
					break;
				default:
					empty = GetWindowsVersion();
					break;
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = "1";
				}
				return empty.Replace(" ", string.Empty);
			}
		}

		public static Platforms ServerPlatform
		{
			get
			{
				return Convert(Application.platform);
			}
		}

		public static Platforms Convert(RuntimePlatform unity3DPlatform)
		{
			switch (unity3DPlatform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.OSXDashboardPlayer:
				return Platforms.OSX;
			case RuntimePlatform.IPhonePlayer:
				return Platforms.iOS;
			case RuntimePlatform.Android:
				return Platforms.Android;
			case RuntimePlatform.NaCl:
				return Platforms.NaCl;
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				return Platforms.WebPlayer;
			default:
				return Platforms.Windows32;
			}
		}

		public static string GetSysInfo()
		{
			string oldDeviceId = DeviceIdManager.OldDeviceId;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SystemInfo.deviceMode='{0}';SystemInfo.deviceType='{2}';SystemInfo.deviceUniqueIdentifier='{3}';SystemInfo.graphicsDeviceID='{4}';SystemInfo.graphicsDeviceName='{5}';SystemInfo.graphicsDeviceVendor='{6}';SystemInfo.graphicsDeviceVendorID='{7}';SystemInfo.graphicsDeviceVersion='{8}';SystemInfo.graphicsMemorySize='{9}';SystemInfo.graphicsPixelFillrate='{10}';SystemInfo.graphicsShaderLevel='{11}';SystemInfo.maxTextureSize='{12}';SystemInfo.npotSupport='{13}';SystemInfo.operatingSystem='{14}';SystemInfo.processorCount='{15}';SystemInfo.processorType='{16}';SystemInfo.supportedRenderTargetCount='{17}';SystemInfo.supports3DTextures='{18}';SystemInfo.supportsAccelerometer='{19}';SystemInfo.supportsComputeShaders='{20}';SystemInfo.supportsGyroscope='{21}';SystemInfo.supportsImageEffects='{22}';SystemInfo.supportsInstancing='{23}';SystemInfo.supportsLocationService='{24}';SystemInfo.supportsRenderTextures='{25}';SystemInfo.supportsShadows='{26}';SystemInfo.supportsVertexPrograms='{27}';SystemInfo.supportsVibration='{28}';SystemInfo.systemMemorySize='{29}';Application.Platform='{30}';OldDeviceId='{31}';", SystemInfo.deviceModel, string.Empty, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier, SystemInfo.graphicsDeviceID, SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVendor, SystemInfo.graphicsDeviceVendorID, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMemorySize, SystemInfo.graphicsPixelFillrate, SystemInfo.graphicsShaderLevel, SystemInfo.maxTextureSize, SystemInfo.npotSupport, SystemInfo.operatingSystem, SystemInfo.processorCount, SystemInfo.processorType, SystemInfo.supportedRenderTargetCount, SystemInfo.supports3DTextures, SystemInfo.supportsAccelerometer, SystemInfo.supportsComputeShaders, SystemInfo.supportsGyroscope, SystemInfo.supportsImageEffects, SystemInfo.supportsInstancing, SystemInfo.supportsLocationService, SystemInfo.supportsRenderTextures, SystemInfo.supportsShadows, SystemInfo.supportsVertexPrograms, SystemInfo.supportsVibration, SystemInfo.systemMemorySize, Application.platform, oldDeviceId);
			stringBuilder.AppendFormat("iPhone.generation='unknown';");
			return stringBuilder.ToString();
		}

		public static string GetLanguageCode(SystemLanguage language)
		{
			switch (language)
			{
			case SystemLanguage.Italian:
				return "ITA";
			case SystemLanguage.German:
				return "DEU";
			case SystemLanguage.French:
				return "FRE";
			case SystemLanguage.Swedish:
				return "SWE";
			case SystemLanguage.Spanish:
				return "SPA";
			case SystemLanguage.Chinese:
				return "CHI";
			case SystemLanguage.Danish:
				return "DAN";
			case SystemLanguage.Dutch:
				return "DUT";
			case SystemLanguage.Finnish:
				return "FIN";
			case SystemLanguage.Norwegian:
				return "NOR";
			case SystemLanguage.Portuguese:
				return "POR";
			case SystemLanguage.Japanese:
				return "JPN";
			case SystemLanguage.Korean:
				return "KOR";
			case SystemLanguage.Russian:
				return "RUS";
			case SystemLanguage.Polish:
				return "POL";
			case SystemLanguage.Turkish:
				return "TUR";
			default:
				return "ENG";
			}
		}

		public static string GetLanguageCode_FromLocale(Locale locale)
		{
			switch (locale)
			{
			case Locale.DEU_Germany:
				return "DEU";
			case Locale.DNK_Denmark:
				return "DNK";
			case Locale.ESP_Spain:
				return "ESP";
			case Locale.FIN_Finland:
				return "FIN";
			case Locale.FRA_France:
				return "FRA";
			case Locale.ITA_Italy:
				return "ITA";
			case Locale.NLD_Holland:
				return "NLD";
			case Locale.NOR_Norway:
				return "NOR";
			case Locale.POL_Poland:
				return "POL";
			case Locale.PRT_Portugal:
				return "PRT";
			case Locale.RUS_Russia:
				return "RUS";
			case Locale.SWE_Sweden:
				return "SWE";
			case Locale.TUR_Turkish:
				return "TUR";
			case Locale.USA_USA:
				return "ENG";
			case Locale.BRP_BrazilianPortuguese:
				return "BRP";
			case Locale.INE_International_English:
			case Locale.IUK_International_English_UK_Spelling:
			case Locale.IUS_International_English_US_Spelling:
				return "ENG";
			case Locale.PRL_Pre_Locale:
				return "ENG";
			case Locale.AUS_Austria:
			case Locale.CZE_Czech_Republic:
			case Locale.GRC_Greek:
				return "ENG";
			default:
				return "ENG";
			}
		}

		public static SystemLanguage GetSystemLanguage(Locale locale)
		{
			switch (locale)
			{
			case Locale.AUS_Austria:
			case Locale.DEU_Germany:
				return SystemLanguage.German;
			case Locale.CZE_Czech_Republic:
				return SystemLanguage.Czech;
			case Locale.DNK_Denmark:
				return SystemLanguage.Danish;
			case Locale.ESP_Spain:
				return SystemLanguage.Spanish;
			case Locale.FIN_Finland:
				return SystemLanguage.Finnish;
			case Locale.FRA_France:
				return SystemLanguage.French;
			case Locale.GRC_Greek:
				return SystemLanguage.Greek;
			case Locale.ITA_Italy:
				return SystemLanguage.Italian;
			case Locale.NLD_Holland:
				return SystemLanguage.Dutch;
			case Locale.NOR_Norway:
				return SystemLanguage.Norwegian;
			case Locale.POL_Poland:
				return SystemLanguage.Polish;
			case Locale.PRT_Portugal:
				return SystemLanguage.Portuguese;
			case Locale.RUS_Russia:
				return SystemLanguage.Russian;
			case Locale.SWE_Sweden:
				return SystemLanguage.Swedish;
			case Locale.TUR_Turkish:
				return SystemLanguage.Turkish;
			case Locale.BRP_BrazilianPortuguese:
				return SystemLanguage.Portuguese;
			default:
				return SystemLanguage.English;
			}
		}

		public static Guid NewGuid()
		{
			return Guid.NewGuid();
		}

		private static string GetMacOsXVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("OSX ", string.Empty);
			int num = operatingSystem.IndexOf('.');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return operatingSystem;
		}

		private static string GetNaClVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("OSX ", string.Empty);
			int num = operatingSystem.IndexOf('.');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return "1";
		}

		private static string GetWebPlayerVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("OSX ", string.Empty);
			int num = operatingSystem.IndexOf('.');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return "1";
		}

		private static string GetIOSVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("iPhone OS ", string.Empty);
			int num = operatingSystem.IndexOf('.');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return operatingSystem;
		}

		private static string GetAndroidVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("Android OS ", string.Empty);
			int num = operatingSystem.IndexOf('.');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return operatingSystem;
		}

		private static string GetWindowsVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			operatingSystem = operatingSystem.Replace("Windows ", string.Empty);
			int num = operatingSystem.IndexOf(' ');
			if (num != -1)
			{
				operatingSystem = operatingSystem.Substring(0, num);
			}
			return operatingSystem;
		}
	}
}
