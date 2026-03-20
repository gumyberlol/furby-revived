using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Relentless.Telemetry.Models
{
	[System.Runtime.Serialization.DataContract]
	public class Tool
	{
		[System.Runtime.Serialization.DataMember]
		public string Name { get; set; }

		[System.Runtime.Serialization.DataMember]
		public string CurrentVersion { get; set; }

		[System.Runtime.Serialization.DataMember]
		public virtual ICollection<ToolRun> ToolRuns { get; set; }

		[System.Runtime.Serialization.DataMember]
		public virtual ICollection<ToolSetting> ToolSettings { get; set; }

		public Tool()
		{
			ToolRuns = new HashSet<ToolRun>();
			ToolSettings = new HashSet<ToolSetting>();
		}
	}
}
