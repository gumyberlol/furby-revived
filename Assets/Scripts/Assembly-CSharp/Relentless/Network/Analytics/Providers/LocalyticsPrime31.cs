using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Relentless.Network.Analytics.Providers
{
	public class LocalyticsPrime31 : TelemetryProviderBase
	{
		[SerializeField]
		private string m_developmentApiKey;

		[SerializeField]
		private string m_productionApiKey;

		protected override string ProviderName
		{
			get
			{
				return "LocalyticsPrime31";
			}
		}

		public override void Awake()
		{
			base.ShouldPumpQueueOnCoroutine = true;
			base.Awake();
			Logging.Log(string.Format("LocalyticsPrime31:Awake ProviderType = {0}, ApiKey= {1}", ToString(), m_productionApiKey));
		}

		public override void StartSession()
		{
			LocalyticsAndroid.startSession(m_productionApiKey);
		}

		public override void EndSession()
		{
			LocalyticsAndroid.endSession();
		}

		public override void TagScreen(string screenName)
		{
			LocalyticsAndroid.tagScreen(screenName);
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			LocalyticsAndroid.tagEventWithAttributes(telemetryEvent.Event, telemetryEvent.Params);
			Logging.Log(telemetryEvent.Params.Select((KeyValuePair<string, string> x) => string.Format("{0} = {1}", x.Key, x.Value)).Aggregate(string.Format("Localytics (Android) tagged event {0}", telemetryEvent.Event), (string x, string y) => x + "\n" + y));
			return true;
		}
	}
}
