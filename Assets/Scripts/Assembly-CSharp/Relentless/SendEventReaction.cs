using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class SendEventReaction : GameEventReaction
	{
		[SerializeField]
		public SerialisableEnum m_eventToSend;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			GameEventRouter.SendEvent(m_eventToSend, gameObject);
		}
	}
}
