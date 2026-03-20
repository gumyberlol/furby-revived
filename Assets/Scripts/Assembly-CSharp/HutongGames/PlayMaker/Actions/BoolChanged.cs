namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Tests if the value of a Bool Variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	[ActionCategory(ActionCategory.Logic)]
	public class BoolChanged : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmBool boolVariable;

		public FsmEvent changedEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private bool previousValue;

		public override void Reset()
		{
			boolVariable = null;
			changedEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			if (boolVariable.IsNone)
			{
				Finish();
			}
			else
			{
				previousValue = boolVariable.Value;
			}
		}

		public override void OnUpdate()
		{
			storeResult.Value = false;
			if (boolVariable.Value != previousValue)
			{
				storeResult.Value = true;
				base.Fsm.Event(changedEvent);
			}
		}
	}
}
