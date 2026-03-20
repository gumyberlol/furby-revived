using System;
using Relentless;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class FurMailRequestBuilder
	{
		public StaticRequestDetails StaticRequestDetails;

		public Relentless.Platforms OS;

		public string OSVer = "6";

		public override string ToString()
		{
			return string.Format("{0}://{1}/api/{2}/message/get/{3}/{4}/{5}", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, OS, OSVer);
		}
	}
}
