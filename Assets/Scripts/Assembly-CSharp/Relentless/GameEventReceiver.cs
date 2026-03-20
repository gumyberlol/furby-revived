using System;
using UnityEngine;

namespace Relentless
{
	public abstract class GameEventReceiver : RelentlessMonoBehaviour
	{
		public abstract Type EventType { get; }

		protected virtual void OnEnable()
		{
			if (EventType == null)
			{
				throw new ApplicationException(string.Format("Null event type for {0}", base.gameObject.name));
			}
			GameEventRouter.AddDelegateForType(EventType, OnEvent);
		}

		protected virtual void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(EventType, OnEvent);
			}
		}

		protected abstract void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList);
	}
}
