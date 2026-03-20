namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the global Alpha for the GUI. Useful for fading GUI up/down. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIAlpha : FsmStateAction
	{
		[RequiredField]
		public FsmFloat alpha;

		public FsmBool applyGlobally;

		public override void Reset()
		{
			alpha = 1f;
		}
	}
}
