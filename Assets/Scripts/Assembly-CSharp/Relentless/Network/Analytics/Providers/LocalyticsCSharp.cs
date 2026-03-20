using System;

namespace Relentless.Network.Analytics.Providers
{
	public class LocalyticsCSharp : TelemetryProviderBase
	{
		protected override string ProviderName
		{
			get
			{
				return "LocalyticsCSharpTelemetry";
			}
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			throw new NotImplementedException();
		}
	}
}
