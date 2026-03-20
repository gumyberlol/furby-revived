using System;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class LowStatusSad_Controller : LowStatusController
	{
		public AnimationClip m_AnimationClip;

		public float m_TimeDelaySecs = 1.25f;

		public override void InitializeController()
		{
			m_AnimationClip.wrapMode = WrapMode.Once;
			GameEventRouter.AddDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
			Invoke("Resume", m_TimeDelaySecs);
		}

		public void OnDisable()
		{
			GameEventRouter.RemoveDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
		}

		public override void UpdateController()
		{
		}

		private void InterceptPlayroomEvents(Enum enumValue, GameObject gameObject, params object[] parameters)
		{
			PlayroomGameEvent playroomGameEvent = (PlayroomGameEvent)(object)enumValue;
			if (playroomGameEvent == PlayroomGameEvent.EnterPlayroom)
			{
				Invoke("Resume", m_TimeDelaySecs);
			}
		}

		private void Resume()
		{
			m_ModelInstance.animation.PlayQueued(m_AnimationClip.name, QueueMode.CompleteOthers, PlayMode.StopSameLayer);
		}
	}
}
