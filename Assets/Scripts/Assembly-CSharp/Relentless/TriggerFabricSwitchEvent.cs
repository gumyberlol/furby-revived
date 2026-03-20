using Fabric;
using HutongGames.PlayMaker;

namespace Relentless
{
	[Tooltip("Triggers a FabricSwitchEvent")]
	[ActionCategory("Fabric")]
	public class TriggerFabricSwitchEvent : FsmStateAction
	{
		private Event _event = new Event();

		[Tooltip("Game Object emitting the audio.")]
		public FsmOwnerDefault gameObject;

		public string m_eventName = string.Empty;

		public string m_switchValue = string.Empty;

		public float _delay;

		public bool _ignoreGameObject = true;

		public override void OnEnter()
		{
			_event._eventName = m_eventName;
			_event.EventAction = EventAction.SetSwitch;
			_event._delay = _delay;
			if (!_ignoreGameObject)
			{
				_event.parentGameObject = base.Fsm.GetOwnerDefaultTarget(gameObject);
			}
			else
			{
				_event.parentGameObject = null;
			}
			_event._parameter = m_switchValue;
			EventManager.Instance.PostEvent(_event);
			Finish();
		}
	}
}
