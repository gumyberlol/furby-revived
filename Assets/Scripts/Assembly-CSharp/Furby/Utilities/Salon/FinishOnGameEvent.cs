using System;
using HutongGames.PlayMaker;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	[HutongGames.PlayMaker.Tooltip("Finishes the current state when a game event occurs.")]
	[ActionCategory("Relentless")]
	public class FinishOnGameEvent : FsmStateAction
	{
		private bool m_seen;

		public SalonGameEvent m_event;

		public override void OnEnter()
		{
			m_seen = false;
			GameEventRouter.AddDelegateForType(typeof(SalonGameEvent), React);
		}

		public override void OnUpdate()
		{
			if (m_seen)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			GameEventRouter.RemoveDelegateForType(typeof(SalonGameEvent), React);
		}

		private void React(Enum eventType, GameObject sender, params object[] parameters)
		{
			if (eventType.Equals(m_event))
			{
				m_seen = true;
			}
		}
	}
}
