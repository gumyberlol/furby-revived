using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class LowStatusHungry_Controller : LowStatusController
	{
		public List<AnimationClip> m_AnimationClips;

		private int m_CurrentAnimIndex;

		private string m_LastAnimationName = string.Empty;

		public float m_TimeDelaySecs = 1.25f;

		public override void InitializeController()
		{
			m_CurrentAnimIndex = 0;
			GameEventRouter.AddDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
		}

		public void OnDisable()
		{
			GameEventRouter.RemoveDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
		}

		public override void UpdateController()
		{
			if (!m_Suspended && m_ModelInstance != null)
			{
				bool flag = m_ModelInstance.animation.IsPlaying("playRoom_enter");
				bool flag2 = m_ModelInstance.animation.IsPlaying("playRoom_exit");
				bool flag3 = m_ModelInstance.animation.IsPlaying(m_LastAnimationName);
				if (!flag2 && !flag && !flag3)
				{
					InvokeAnimationClip(GetAnimationClip());
					m_CurrentAnimIndex++;
				}
			}
		}

		private void InvokeAnimationClip(AnimationClip clip)
		{
			clip.wrapMode = WrapMode.Once;
			m_ModelInstance.animation.wrapMode = WrapMode.Once;
			m_ModelInstance.animation.Play(clip.name, PlayMode.StopSameLayer);
			m_LastAnimationName = clip.name;
			if (m_CurrentAnimIndex == 0)
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.LowStatus_FeelsHungry);
			}
		}

		private AnimationClip GetAnimationClip()
		{
			if (m_CurrentAnimIndex >= m_AnimationClips.Count)
			{
				m_CurrentAnimIndex = 0;
			}
			return m_AnimationClips[m_CurrentAnimIndex];
		}

		private void InterceptPlayroomEvents(Enum enumValue, GameObject gameObject, params object[] parameters)
		{
			PlayroomGameEvent playroomGameEvent = (PlayroomGameEvent)(object)enumValue;
			if (playroomGameEvent == PlayroomGameEvent.EnterPlayroom)
			{
				m_Suspended = false;
			}
			if (playroomGameEvent == PlayroomGameEvent.ExitPlayroom)
			{
				m_Suspended = true;
			}
		}
	}
}
