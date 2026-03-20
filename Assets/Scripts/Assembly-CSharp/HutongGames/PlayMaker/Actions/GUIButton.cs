namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
	[ActionCategory(ActionCategory.GUI)]
	public class GUIButton : GUIContentAction
	{
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;

		public override void Reset()
		{
			base.Reset();
			sendEvent = null;
			storeButtonState = null;
			style = "Button";
		}
	}
}
