namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Repeat Button. Sends an Event while pressed. Optionally store the button state in a Bool Variable.")]
	public class GUILayoutRepeatButton : GUILayoutAction
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
