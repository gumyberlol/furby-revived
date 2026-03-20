using System;

namespace Relentless.Telemetry.Models
{
	public class LogEvent
	{
		public Guid LogEventId { get; set; }

		public Guid? ToolRunId { get; set; }

		public Guid? ConsoleRunId { get; set; }

		public string Severity { get; set; }

		public int? ThreadId { get; set; }

		public DateTime? TimeStamp { get; set; }

		public string Context { get; set; }

		public string StackTrace { get; set; }

		public string Message { get; set; }

		public string Exception { get; set; }

		public virtual ToolRun ToolRun { get; set; }

		public virtual GameRun GameRun { get; set; }
	}
}
