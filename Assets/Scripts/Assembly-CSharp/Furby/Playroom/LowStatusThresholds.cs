using System;

namespace Furby.Playroom
{
	[Serializable]
	public class LowStatusThresholds
	{
		public float NeglectThreshold = 0.25f;

		public float HungrinessThreshold = 0.25f;

		public float DirtinessThreshold = 0.25f;
	}
}
