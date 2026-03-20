using System;
using Relentless.Core.DesignPatterns;

namespace Relentless.Network.Analytics.Providers
{
	public class GoogleAnalytics : TelemetryProviderBase
	{
		public static readonly string ToolName = "BlueToadGame";

		public static readonly string CurrentVersion = "1.0";

		private string AccountId = "UA-35886025-2";

		private string Domain = "www.relentless.co.uk";

		private string m_category;

		protected override string ProviderName
		{
			get
			{
				return "GoogleAnalyticsTelemetry";
			}
		}

		private void Start()
		{
			Relentless.Core.DesignPatterns.Singleton<GoogleAnalyticsHelper>.Instance.Initialise(AccountId, Domain);
			m_category = string.Format("{0}_{1}", ToolName, CurrentVersion);
		}

		public override void StartSession()
		{
			Relentless.Core.DesignPatterns.Singleton<GoogleAnalyticsHelper>.Instance.Initialise(AccountId, Domain);
			Relentless.Core.DesignPatterns.Singleton<GoogleAnalyticsHelper>.Instance.LogEvent("event", m_category, "game_start", string.Empty, 0.0);
		}

		public override void EndSession()
		{
			Relentless.Core.DesignPatterns.Singleton<GoogleAnalyticsHelper>.Instance.LogEvent("event", m_category, "game_end", string.Empty, 0.0);
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			throw new NotImplementedException();
		}
	}
}
