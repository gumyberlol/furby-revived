using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class InteractionIncident
	{
		[SerializeField]
		public Collider m_Collider;

		[SerializeField]
		public ActionType m_ActionTypeRequired;

		[SerializeField]
		public PlayroomGameEvent m_ReactionEvent;

		[SerializeField]
		[Range(0f, 10000f)]
		public float m_DistanceRequired = 500f;
	}
}
