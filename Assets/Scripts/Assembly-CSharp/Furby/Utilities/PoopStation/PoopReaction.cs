using System;
using System.Collections.Generic;

namespace Furby.Utilities.PoopStation
{
	[Serializable]
	public class PoopReaction
	{
		public Poop.PoopType m_type;

		public List<FurbyAction> m_actions;
	}
}
