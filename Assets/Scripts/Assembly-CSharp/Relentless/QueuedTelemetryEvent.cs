using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class QueuedTelemetryEvent
	{
		public int EventVersion = 1;

		public DateTime Timestamp;

		public int SessionIndex;

		public string Event;

		public string Description;

		public string Version;

		public string OSVersion;

		public string DevicePlatform;

		public string DeviceModel;

		public string UserID;

		public Dictionary<string, string> Params;

		public int QueueLength;

		public QueuedTelemetryEvent()
		{
			Timestamp = DateTime.MinValue;
		}
	}
}
