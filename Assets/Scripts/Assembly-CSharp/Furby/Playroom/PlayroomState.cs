using Fabric;
using Relentless;

namespace Furby.Playroom
{
	internal class PlayroomState : RelentlessMonoBehaviour
	{
		private PlayroomStateEnum m_CurrentState;

		public PlayroomStateEnum CurrentState
		{
			get
			{
				return m_CurrentState;
			}
			set
			{
				m_CurrentState = value;
			}
		}

		private void Start()
		{
			BroadcastFabricEvent();
		}

		private void BroadcastFabricEvent()
		{
			Event obj = new Event();
			obj.EventAction = EventAction.SetSwitch;
			obj._eventName = "PlayroomStateSwitch";
			obj._parameter = ((m_CurrentState != PlayroomStateEnum.Normal) ? "levelup" : "normal");
			EventManager.Instance.PostEvent(obj);
		}
	}
}
