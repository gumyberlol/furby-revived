using System.Collections.Generic;

namespace Relentless.Telemetry.Models
{
	public class Console
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public decimal? CurrentFirmware { get; set; }

		public virtual ICollection<GameRun> GameRuns { get; set; }

		public Console()
		{
			GameRuns = new HashSet<GameRun>();
		}
	}
}
