using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class EventReactions_GET : MonoBehaviour
	{
		public EventReactionsLibrary library;

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(typeof(PoopStationEvent), React);
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PoopStationEvent), React);
			}
		}

		public void React(Enum eventType, GameObject origin, params object[] parameters)
		{
			try
			{
				FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
				FurbyAction action = library.GetAction(personality, eventType);
				Logging.Log(string.Concat(personality, " is ", action, " at ", eventType));
				Singleton<FurbyDataChannel>.Instance.PostAction(action, null);
			}
			catch (EventReactionsLibrary.NoActionException)
			{
			}
		}
	}
}
