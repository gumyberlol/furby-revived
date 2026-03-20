using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class EventReactionsLibrary : ScriptableObject
	{
		public class NoActionException : ApplicationException
		{
			public NoActionException(FurbyPersonality personality, Enum evt)
				: base(GenerateMessage(personality, evt))
			{
			}

			private static string GenerateMessage(FurbyPersonality personality, Enum evt)
			{
				return string.Concat("No reaction for personality ", personality, " for event ", evt);
			}
		}

		public List<EventReactionList> reactions;

		public FurbyAction GetAction(FurbyPersonality personality, Enum eventType)
		{
			PoopStationEvent poopStationEvent = (PoopStationEvent)(object)eventType;
			EventReactionList eventReactionList = reactions.Find((EventReactionList x) => x.personality == personality);
			if (eventReactionList == null)
			{
				throw new NoActionException(personality, eventType);
			}
			EventReaction eventReaction = eventReactionList.reactions.Find((EventReaction x) => x.m_event == poopStationEvent);
			if (eventReaction == null)
			{
				throw new NoActionException(personality, eventType);
			}
			return eventReaction.ChooseAction();
		}
	}
}
