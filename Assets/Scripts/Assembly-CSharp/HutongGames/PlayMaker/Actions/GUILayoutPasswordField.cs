namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutPasswordField : GUILayoutAction
	{
		[UIHint(UIHint.Variable)]
		public FsmString text;

		public FsmInt maxLength;

		public FsmString style;

		public FsmEvent changedEvent;

		public FsmString mask;

		public override void Reset()
		{
			text = null;
			maxLength = 25;
			style = "TextField";
			mask = "*";
			changedEvent = null;
		}
	}
}
