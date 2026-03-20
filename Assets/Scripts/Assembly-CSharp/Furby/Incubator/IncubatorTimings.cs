using System;
using System.Collections.Generic;
using System.Linq;

namespace Furby.Incubator
{
	[Serializable]
	public class IncubatorTimings
	{
		public List<IncubatorLevel> m_TimingArray;

		public string Validate()
		{
			m_TimingArray.Sort((IncubatorLevel x, IncubatorLevel y) => x.LevelIndex - y.LevelIndex);
			if (m_TimingArray.Count == 0)
			{
				return "Timing list is empty!";
			}
			for (int num = 0; num < m_TimingArray.Count; num++)
			{
				IncubatorLevel incubatorLevel = m_TimingArray[num];
				if (num != incubatorLevel.LevelIndex)
				{
					return "Egg counts (LevelIndex) must be continuous and start at 0 (one-egg)";
				}
				if (incubatorLevel.IncubationTime < 20 || incubatorLevel.IncubationTime > 172800)
				{
					return "Incubation time must be in the range of 20 .. 172800 seconds";
				}
				if (incubatorLevel.MinAttentionInterval != 0f || incubatorLevel.MaxAttentionInterval != 0f)
				{
					return "Attention min/max intervals must be zero";
				}
			}
			return null;
		}

		public IncubatorLevel GetExactLevel(int ordinal)
		{
			return m_TimingArray.FirstOrDefault((IncubatorLevel i) => i.LevelIndex == ordinal);
		}

		public IncubatorLevel GetMinimumLevel(int ordinal)
		{
			return m_TimingArray.LastOrDefault((IncubatorLevel i) => i.LevelIndex <= ordinal);
		}
	}
}
