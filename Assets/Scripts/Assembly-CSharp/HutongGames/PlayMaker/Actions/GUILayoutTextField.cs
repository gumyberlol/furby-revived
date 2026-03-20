namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Text Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutTextField : GUILayoutAction
	{
		[UIHint(UIHint.Variable)]
		public FsmString text;

		public FsmInt maxLength;

		public FsmString style;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			text = null;
			maxLength = 25;
			style = "TextField";
			changedEvent = null;
		}
	}
}
