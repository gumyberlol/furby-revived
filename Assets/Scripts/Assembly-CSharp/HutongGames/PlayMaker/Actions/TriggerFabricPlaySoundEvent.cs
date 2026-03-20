using Fabric;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Fabric")]
	[Tooltip("Triggers a Fabric PlaySound Event")]
	public class TriggerFabricPlaySoundEvent : FsmStateAction
	{
		private Event _event = new Event();

		[Tooltip("Game Object emitting the audio.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The name of the event to trigger.")]
		public string _eventName = string.Empty;

		[Tooltip("The delay in seconds after which the event should be triggered")]
		public float _delay;

		[Tooltip("Ignore the position of the GameObject if placing this sound in 3D")]
		public bool _ignoreGameObject = true;

		public override void OnEnter()
		{
			_event._eventName = _eventName;
			_event.EventAction = EventAction.PlaySound;
			_event._delay = _delay;
			if (!_ignoreGameObject)
			{
				_event.parentGameObject = base.Fsm.GetOwnerDefaultTarget(gameObject);
			}
			else
			{
				_event.parentGameObject = null;
			}
			_event._parameter = string.Empty;
			EventManager.Instance.PostEvent(_event);
			Finish();
		}
	}
}
