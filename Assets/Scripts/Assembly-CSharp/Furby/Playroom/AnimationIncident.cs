using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class AnimationIncident
	{
		public AnimationClip m_Clip;

		public bool m_EventExists;

		public PlayroomGameEvent m_GameEvent;
	}
}
