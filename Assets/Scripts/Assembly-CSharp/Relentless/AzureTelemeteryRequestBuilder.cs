using System;

namespace Relentless
{
	[Serializable]
	public class AzureTelemeteryRequestBuilder
	{
		public StaticRequestDetails StaticRequestDetails;

		public override string ToString()
		{
			return string.Format("{0}://{1}/api/{2}/telemetry/{3}", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion);
		}
	}
}
