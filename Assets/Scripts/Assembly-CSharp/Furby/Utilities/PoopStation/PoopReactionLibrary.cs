using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class PoopReactionLibrary : ScriptableObject
	{
		[SerializeField]
		private List<PoopReactions> m_reactions;

		public FurbyAction GetReactionFor(FurbyPersonality personality, Poop.PoopType poop)
		{
			PoopReactions poopReactions = m_reactions.Find((PoopReactions r) => r.m_personality == personality);
			if (poopReactions == null)
			{
				throw new ApplicationException(string.Format("No poop reactions for {0}", personality));
			}
			return poopReactions.GetReactionFor(poop);
		}
	}
}
