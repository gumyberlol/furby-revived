namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
	[ActionCategory(ActionCategory.GUI)]
	public class GUIHorizontalSlider : GUIAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat leftValue;

		[RequiredField]
		public FsmFloat rightValue;

		public FsmString sliderStyle;

		public FsmString thumbStyle;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			leftValue = 0f;
			rightValue = 100f;
			sliderStyle = "horizontalslider";
			thumbStyle = "horizontalsliderthumb";
		}
	}
}
