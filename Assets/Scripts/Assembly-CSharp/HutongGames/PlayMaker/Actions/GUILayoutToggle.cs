namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutToggle : GUILayoutAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmBool storeButtonState;

		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

		public FsmString style;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			storeButtonState = null;
			text = string.Empty;
			image = null;
			tooltip = string.Empty;
			style = "Toggle";
			changedEvent = null;
		}
	}
}
