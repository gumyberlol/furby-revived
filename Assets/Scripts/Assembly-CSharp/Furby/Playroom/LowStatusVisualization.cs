using System;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class LowStatusVisualization : MonoBehaviour
	{
		public AnimationClip m_OnAnimation;

		public AnimationClip m_OffAnimation;

		public float m_TimeDelaySecs = 1.25f;

		public bool m_Suspended;

		public bool Suspended
		{
			get
			{
				return m_Suspended;
			}
		}

		private void Start()
		{
			Singleton<PlayroomIdlingController>.Instance.Enable = false;
			GameObject.Find("FingerEyeTracking").GetComponent<LookAtTouch>().enabled = false;
			GameEventRouter.AddDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
			m_Suspended = false;
		}

		public void OnDisable()
		{
			GameEventRouter.RemoveDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.EnterPlayroom, PlayroomGameEvent.ExitPlayroom);
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
