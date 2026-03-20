namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets the Right n characters from a String.")]
	public class GetStringRight : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmString stringVariable;

		public FsmInt charCount;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			stringVariable = null;
			charCount = 0;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetStringRight();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetStringRight();
		}

		private void DoGetStringRight()
		{
			if (stringVariable != null && storeResult != null)
			{
				string value = stringVariable.Value;
				storeResult.Value = value.Substring(value.Length - charCount.Value, charCount.Value);
			}
		}
	}
}
