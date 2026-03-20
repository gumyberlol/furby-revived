using System;

namespace Furby
{
	[Serializable]
	public class HintExpiryCausation
	{
		public ExpiryCausation m_Causation;

		public HintEvents m_EventExpiry;

		public float m_DurationExpiry;

		public HintExpiryCausation()
		{
			m_Causation = ExpiryCausation.CloseOnEvent;
		}
	}
}
