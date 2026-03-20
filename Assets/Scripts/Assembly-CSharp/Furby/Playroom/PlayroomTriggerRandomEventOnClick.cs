using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomTriggerRandomEventOnClick : RelentlessMonoBehaviour
	{
		public List<PlayroomGameEvent> m_Events = new List<PlayroomGameEvent>();

		private int m_PreviousGameEventIndex = -1;

		private void OnClick()
		{
			if (m_Events.Count > 0)
			{
				int randomIndex_FavouringDifferentToPrevious = GetRandomIndex_FavouringDifferentToPrevious();
				PlayroomGameEvent playroomGameEvent = m_Events[randomIndex_FavouringDifferentToPrevious];
				GameEventRouter.SendEvent(playroomGameEvent);
			}
		}

		private int GetRandomIndex_FavouringDifferentToPrevious()
		{
			int randomIndex = GetRandomIndex();
			if (randomIndex == m_PreviousGameEventIndex)
			{
				randomIndex = GetRandomIndex();
			}
			return randomIndex;
		}

		private int GetRandomIndex()
		{
			return Random.Range(0, m_Events.Count - 1);
		}
	}
}
