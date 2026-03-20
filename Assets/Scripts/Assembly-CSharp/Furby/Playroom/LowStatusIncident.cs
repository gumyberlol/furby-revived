using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class LowStatusIncident
	{
		public GameObject m_ObjectToActivate;

		public bool m_HaveEvent;

		public PlayroomGameEvent m_EventToRaise;
	}
}
