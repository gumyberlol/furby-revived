using System;
using System.Collections.Generic;

namespace Relentless.Network.Analytics.Providers
{
	[Serializable]
	public class PersistentQueue
	{
		public int Version;

		public List<QueuedTelemetryEvent> EventQueue;

		public DateTime LastUpdatedUtc;
	}
}
