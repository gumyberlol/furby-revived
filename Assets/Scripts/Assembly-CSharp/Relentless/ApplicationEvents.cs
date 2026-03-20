using Relentless.Network.Analytics;

namespace Relentless
{
	public class ApplicationEvents : RelentlessMonoBehaviour
	{
		public void DidReceiveMemoryWarning(string message)
		{
			TelemetryParams telemParams = new TelemetryParams("Event", "DidReceiveMemoryWarning");
			SingletonInstance<TelemetryManager>.Instance.LogEvent("ApplicationEvents", telemParams, false);
			Logging.Log("DidReceiveMemoryWarning");
		}

		public void DidFinishLaunchingWithOptions(string message)
		{
			TelemetryParams telemParams = new TelemetryParams("Event", "DidFinishLaunchingWithOptions");
			SingletonInstance<TelemetryManager>.Instance.LogEvent("ApplicationEvents", telemParams, false);
			Logging.Log("DidFinishLaunchingWithOptions");
		}

		public void DidEnterBackground(string message)
		{
			Logging.Log("DidEnterBackground");
		}

		public void WillEnterForeground(string message)
		{
			Logging.Log("WillEnterForeground");
		}

		public void DidBecomeActive(string message)
		{
			Logging.Log("DidBecomeActive");
		}

		public void WillResignActive(string message)
		{
			Logging.Log("WillResignActive");
		}

		public void WillTerminate(string message)
		{
			Logging.Log("WillTerminate");
		}
	}
}
