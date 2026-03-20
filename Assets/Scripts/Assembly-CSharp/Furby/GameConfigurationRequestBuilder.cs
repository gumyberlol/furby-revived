using System;
using Relentless;

namespace Furby
{
	[Serializable]
	public class GameConfigurationRequestBuilder
	{
		public StaticRequestDetails StaticRequestDetails;

		public Relentless.Platforms OS;

		public string OSVer = "6";

		public string GetUrl(string filename)
		{
			return string.Format("{0}://{1}/api/{2}/gameconfig/{3}/{4}/{5}/gameconfig/{6}", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, OS, OSVer, filename);
		}
	}
}
