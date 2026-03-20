using HutongGames.PlayMaker;
using Relentless;

namespace Furby.Incubator
{
	[ActionCategory("Furby")]
	public class IncubatorGameEventFSMAction : FsmStateAction
	{
		public IncubatorGameEvent m_event;

		public FsmFloat m_floatParam;

		public FsmGameObject m_origin;

		public override void OnEnter()
		{
			GameEventRouter.SendEvent(m_event, m_origin.Value, m_floatParam.Value);
			Finish();
		}
	}
}
