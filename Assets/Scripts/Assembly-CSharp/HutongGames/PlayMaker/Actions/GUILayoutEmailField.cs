namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutEmailField : GUILayoutAction
	{
		[UIHint(UIHint.Variable)]
		public FsmString text;

		public FsmInt maxLength;

		public FsmString style;

		public FsmEvent changedEvent;

		public FsmBool valid;

		public override void Reset()
		{
			text = null;
			maxLength = 25;
			style = "TextField";
			valid = true;
			changedEvent = null;
		}
	}
}
