using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class PayWallEpisode
	{
		public bool Enabled;

		public string EpisodeId = string.Empty;

		public List<PayWallScene> Scenes;
	}
}
