namespace Relentless.Network.Analytics.Providers
{
	public abstract class CloudTelemeteryProvider : TelemetryProviderBase
	{
		public override void Awake()
		{
			base.Awake();
			Logging.Log(string.Format("CloudTelemeteryProvider:Awake ProviderType = {0}", ToString()));
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			if (string.Compare(telemetryEvent.Event, "StartSession") == 0 && telemetryEvent.Params != null && !telemetryEvent.Params.ContainsKey("SysInfo"))
			{
				telemetryEvent.Params.Add("SysInfo", base.SysInfo);
			}
			return true;
		}
	}
}
