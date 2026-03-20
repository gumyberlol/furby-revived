namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutButton : GUILayoutAction
	{
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;

		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			sendEvent = null;
			storeButtonState = null;
			text = string.Empty;
			image = null;
			tooltip = string.Empty;
			style = string.Empty;
		}
	}
}
