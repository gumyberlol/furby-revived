using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float Variable to its absolute value.")]
	public class FloatAbs : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoFloatAbs();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFloatAbs();
		}

		private void DoFloatAbs()
		{
			floatVariable.Value = Mathf.Abs(floatVariable.Value);
		}
	}
}
