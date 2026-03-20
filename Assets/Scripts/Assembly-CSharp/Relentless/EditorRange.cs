using System;

namespace Relentless
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EditorRange : Attribute
	{
		public readonly float m_min;

		public readonly float m_max;

		public EditorRange(float min, float max)
		{
			m_min = min;
			m_max = max;
		}
	}
}
