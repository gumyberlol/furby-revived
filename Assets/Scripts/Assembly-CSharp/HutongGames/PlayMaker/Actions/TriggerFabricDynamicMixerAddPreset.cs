using Fabric;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Triggers a Fabric DynamicMixerAddPreset Event")]
	[ActionCategory("Fabric")]
	public class TriggerFabricDynamicMixerAddPreset : FsmStateAction
	{
		private Event _event = new Event();

		[Tooltip("The name of the preset to Add.")]
		public string m_presetName = string.Empty;

		[Tooltip("The time in seconds after which the preset should be added")]
		public float m_delay;

		public override void OnEnter()
		{
			_event._eventName = "DynamicMixer";
			_event.EventAction = EventAction.AddPreset;
			_event._delay = m_delay;
			_event.parentGameObject = null;
			_event._parameter = m_presetName;
			EventManager.Instance.PostEvent(_event);
			Finish();
		}
	}
}
