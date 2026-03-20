using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class MessageRequestContent
	{
		public string GameVersion = "1.6.0";

		public string SessionId;

		public string DeviceId;

		public string RequestId;

		public ProfileFriendlyName ProfileFriendlyName;

		public ProfileType ProfileType;

		public Resolution Resolution;

		public string SysInfo;

		public string VersionNumber;

		public SystemLanguage Language;

		public string LanguageCode;

		public string CountryCode;
	}
}
