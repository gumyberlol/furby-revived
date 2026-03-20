using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class FurbyIdleAnimation
	{
		[SerializeField]
		public AnimationClip m_AnimationClip;

		[SerializeField]
		public bool m_AllowEyeTracking = true;

		[SerializeField]
		public bool m_HaveGameEvent;

		[SerializeField]
		public PlayroomGameEvent m_PlayroomGameEvent;
	}
}
