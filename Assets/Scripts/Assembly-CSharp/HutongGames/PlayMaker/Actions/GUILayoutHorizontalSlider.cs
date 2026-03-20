namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("A Horizontal Slider linked to a Float Variable.")]
	public class GUILayoutHorizontalSlider : GUILayoutAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat leftValue;

		[RequiredField]
		public FsmFloat rightValue;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			leftValue = 0f;
			rightValue = 100f;
			changedEvent = null;
		}
	}
}
