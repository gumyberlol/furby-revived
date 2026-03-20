using System;

namespace Relentless.Telemetry.Models
{
	public class ToolSetting
	{
		public Guid SettingEntryId { get; set; }

		public string Name { get; set; }

		public string User { get; set; }

		public string P4Username { get; set; }

		public string MachineName { get; set; }

		public string SettingName { get; set; }

		public string SettingValue { get; set; }

		public DateTime? LastUpdate { get; set; }

		public virtual Tool Tool { get; set; }
	}
}
