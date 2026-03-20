using System;
using System.Collections.Generic;

namespace Furby.Utilities.PoopStation
{
	[Serializable]
	public class EventReactionList
	{
		public FurbyPersonality personality;

		public List<EventReaction> reactions;
	}
}
