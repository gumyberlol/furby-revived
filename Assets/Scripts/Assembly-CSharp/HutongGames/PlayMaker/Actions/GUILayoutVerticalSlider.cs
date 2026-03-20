namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("A Vertical Slider linked to a Float Variable.")]
	public class GUILayoutVerticalSlider : GUILayoutAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat topValue;

		[RequiredField]
		public FsmFloat bottomValue;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			topValue = 100f;
			bottomValue = 0f;
			changedEvent = null;
		}
	}
}
