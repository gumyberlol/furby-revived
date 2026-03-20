using System;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class LowStatusDirty_Controller : LowStatusController
	{
		public AnimationClip m_AnimationClip;

		public float m_TimeDelaySecs = 1.25f;

		public GameObject m_FlyVFX;

		public override void InitializeController()
		{
			m_AnimationClip.wrapMode = WrapMode.Once;
			GameEventRouter.SendEvent(PlayroomGameEvent.LowStatus_FliesPresent);
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
			if (playroomGameEvent == PlayroomGameEvent.ExitPlayroom)
			{
				Invoke("Suspend", m_TimeDelaySecs);
			}
		}

		private void Suspend()
		{
			m_FlyVFX.SetActive(false);
		}

		private void Resume()
		{
			m_ModelInstance.animation.PlayQueued(m_AnimationClip.name, QueueMode.CompleteOthers, PlayMode.StopSameLayer);
			m_FlyVFX.SetActive(true);
		}
	}
}
