using System;

namespace Relentless
{
	[Serializable]
	public class AzureTelemetryServiceRequestContent
	{
		public string SessionId;

		public string DeviceId;

		public string RequestId;

		public string TelemeteryDataBase64;
	}
}
