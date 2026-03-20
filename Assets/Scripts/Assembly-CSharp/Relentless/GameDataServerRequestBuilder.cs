using System;

namespace Relentless
{
	[Serializable]
	public class GameDataServerRequestBuilder
	{
		public StaticRequestDetails StaticRequestDetails;

		public Platforms OS;

		public string OSVer = "6";

		public override string ToString()
		{
			return string.Format("{0}://{1}/api/{2}/gamedata/{3}/{4}/{5}", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, OS, OSVer);
		}
	}
}
