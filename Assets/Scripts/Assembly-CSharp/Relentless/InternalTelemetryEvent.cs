using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class InternalTelemetryEvent
	{
		public DateTime Timestamp;

		public int SessionIndex;

		public string Event;

		public string Description;

		public string Version;

		public string OSVersion;

		public string Platform;

		public string UserID;

		public Dictionary<string, string> Params;

		public int QueueLength;

		public InternalTelemetryEvent()
		{
			Timestamp = DateTime.MinValue;
		}
	}
}
