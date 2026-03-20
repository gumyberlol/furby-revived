using UnityEngine;

namespace Relentless
{
	public class SendGameRouterEvent : MonoBehaviour
	{
		public SerialisableEnum[] m_events;

		public void SendGameEvent(int index)
		{
			GameEventRouter.SendEvent(m_events[index]);
		}
	}
}
