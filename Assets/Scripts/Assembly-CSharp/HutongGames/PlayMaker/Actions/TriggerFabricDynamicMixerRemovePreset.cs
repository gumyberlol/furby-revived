using Fabric;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Triggers a Fabric DynamicMixerRemovePreset Event")]
	[ActionCategory("Fabric")]
	public class TriggerFabricDynamicMixerRemovePreset : FsmStateAction
	{
		private Event _event = new Event();

		[Tooltip("The name of the preset to Remove.")]
		public string m_presetName = string.Empty;

		[Tooltip("The time in seconds after which the preset should be removed")]
		public float m_delay;

		public override void OnEnter()
		{
			_event._eventName = "DynamicMixer";
			_event.EventAction = EventAction.RemovePreset;
			_event._delay = m_delay;
			_event.parentGameObject = null;
			_event._parameter = m_presetName;
			EventManager.Instance.PostEvent(_event);
			Finish();
		}
	}
}
