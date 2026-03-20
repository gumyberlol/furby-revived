using System;
using System.Collections.Generic;

namespace Furby.Utilities.Hose
{
	[Serializable]
	public class DroppedItemReaction
	{
		public FurbyPersonality m_personality;

		public DroppedItem.ItemType m_type;

		public List<FurbyAction> m_actions;
	}
}
