using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class PoopReactions : ScriptableObject
	{
		[SerializeField]
		public FurbyPersonality m_personality;

		[SerializeField]
		private List<PoopReaction> m_reactions;

		public FurbyAction GetReactionFor(Poop.PoopType p)
		{
			PoopReaction poopReaction = m_reactions.Find((PoopReaction r) => r.m_type == p);
			if (poopReaction == null || poopReaction.m_actions.Count == 0)
			{
				throw new ApplicationException(string.Format("Peronality {0} has no reaction for Poop type {1}", m_personality, p));
			}
			int index = UnityEngine.Random.Range(0, poopReaction.m_actions.Count);
			return poopReaction.m_actions[index];
		}
	}
}
