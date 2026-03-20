using System;
using System.Collections.Generic;

namespace Relentless.Telemetry.Models
{
	public class ToolRun
	{
		public Guid ToolRunId { get; set; }

		public Guid? StartedFromToolRunId { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public string FullPath { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime? EndTime { get; set; }

		public string Args { get; set; }

		public string User { get; set; }

		public string MachineName { get; set; }

		public int? Processors { get; set; }

		public int? Memory { get; set; }

		public string NormalisedPath { get; set; }

		public string P4Username { get; set; }

		public string P4ClientSpec { get; set; }

		public string P4Root { get; set; }

		public string CurrentDirectory { get; set; }

		public string NormalisedCurrentDirectory { get; set; }

		public bool? IsDebug { get; set; }

		public virtual ICollection<LogEvent> LogEvents { get; set; }

		public virtual ICollection<TelemetryEvent> TelemetryEvents { get; set; }

		public virtual Tool Tool { get; set; }

		public virtual ICollection<ToolRun> StartedFromToolRuns { get; set; }

		public virtual ToolRun StartedFromToolRun { get; set; }

		public virtual ICollection<GameRun> GameRuns { get; set; }

		public ToolRun()
		{
			LogEvents = new HashSet<LogEvent>();
			TelemetryEvents = new HashSet<TelemetryEvent>();
			StartedFromToolRuns = new HashSet<ToolRun>();
			GameRuns = new HashSet<GameRun>();
		}
	}
}
