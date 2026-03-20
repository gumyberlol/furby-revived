using System;
using System.Collections.Generic;

namespace Relentless.Telemetry.Models
{
	public class GameRun
	{
		public Guid GameRunId { get; set; }

		public Guid? StartedFromToolRunId { get; set; }

		public string ConsoleName { get; set; }

		public string Firmware { get; set; }

		public DateTime? StartTime { get; set; }

		public string Args { get; set; }

		public string User { get; set; }

		public int? Memory { get; set; }

		public virtual Console Console { get; set; }

		public virtual ToolRun ToolRun { get; set; }

		public virtual ICollection<LogEvent> LogEvents { get; set; }

		public virtual ICollection<TelemetryEvent> TelemetryEvents { get; set; }

		public GameRun()
		{
			LogEvents = new HashSet<LogEvent>();
			TelemetryEvents = new HashSet<TelemetryEvent>();
		}
	}
}
