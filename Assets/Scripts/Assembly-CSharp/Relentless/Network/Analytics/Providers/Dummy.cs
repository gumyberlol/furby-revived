namespace Relentless.Network.Analytics.Providers
{
	public class Dummy : TelemetryProviderBase
	{
		protected override string ProviderName
		{
			get
			{
				return "DummyTelemetry";
			}
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			return true;
		}
	}
}
