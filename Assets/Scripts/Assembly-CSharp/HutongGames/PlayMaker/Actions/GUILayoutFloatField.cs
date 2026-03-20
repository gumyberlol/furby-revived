namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Text Field to edit a Float Variable. Optionally send an event if the text has been edited.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutFloatField : GUILayoutAction
	{
		[Tooltip("Float Variable to show in the edit field.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		[Tooltip("Optional event to send when the value changes.")]
		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			style = string.Empty;
			changedEvent = null;
		}
	}
}
