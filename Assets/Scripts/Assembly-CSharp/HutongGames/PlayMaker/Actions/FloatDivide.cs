namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Divides a one Float by another.")]
	[ActionCategory(ActionCategory.Math)]
	public class FloatDivide : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat divideBy;

		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			divideBy = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			floatVariable.Value /= divideBy.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			floatVariable.Value /= divideBy.Value;
		}
	}
}
