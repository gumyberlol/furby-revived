using System.Collections.Generic;

namespace Relentless.Network.Analytics
{
	public interface ITelemetry : IProvider
	{
		void LogEvent(string eventName, bool isTimed);

		void LogEvent(string eventName, TelemetryParams telemParams, bool isTimed);

		void EndTimedEvent(string eventName);

		void EndTimedEvent(string eventName, Dictionary<string, string> parameters);

		void SetAge(int age);

		void SetGender(string gender);

		void SetUserId(string userId);

		void SetSessionReportsOnCloseEnabled(bool sendSessionReportsOnClose);

		void SetSessionReportsOnPauseEnabled(bool setSessionReportsOnPauseEnabled);
	}
}
