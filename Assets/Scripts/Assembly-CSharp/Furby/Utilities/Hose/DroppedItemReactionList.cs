using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class DroppedItemReactionList : ScriptableObject
	{
		[SerializeField]
		private List<DroppedItemReaction> m_reactions;

		public FurbyAction GetActionFor(FurbyPersonality personality, DroppedItem.ItemType type)
		{
			DroppedItemReaction droppedItemReaction = m_reactions.Find((DroppedItemReaction r) => r.m_personality.Equals(personality) && r.m_type.Equals(type));
			if (droppedItemReaction == null || droppedItemReaction.m_actions.Count == 0)
			{
				throw new ApplicationException(string.Format("Personality {0} has no reaction for item type {1}", personality, type));
			}
			int index = UnityEngine.Random.Range(0, droppedItemReaction.m_actions.Count);
			return droppedItemReaction.m_actions[index];
		}
	}
}
