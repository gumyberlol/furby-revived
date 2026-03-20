using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	[Serializable]
	public class EventReaction
	{
		public PoopStationEvent m_event;

		[SerializeField]
		public List<FurbyAction> m_actions;

		public FurbyAction ChooseAction()
		{
			System.Random random = new System.Random();
			return m_actions[random.Next(m_actions.Count)];
		}
	}
}
