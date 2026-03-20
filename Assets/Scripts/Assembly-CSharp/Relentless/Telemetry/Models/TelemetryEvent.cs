using System;

namespace Relentless.Telemetry.Models
{
	public class TelemetryEvent
	{
		public Guid TelemetryEventId { get; set; }

		public Guid? ToolRunId { get; set; }

		public Guid? GameRunId { get; set; }

		public string Event { get; set; }

		public virtual ToolRun ToolRun { get; set; }

		public virtual GameRun GameRun { get; set; }
	}
}
