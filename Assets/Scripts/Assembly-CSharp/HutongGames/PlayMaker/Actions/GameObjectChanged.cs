using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Tests if the value of a game object variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	[ActionCategory(ActionCategory.Logic)]
	public class GameObjectChanged : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObjectVariable;

		[RequiredField]
		public FsmEvent changedEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private GameObject previousValue;

		public override void Reset()
		{
			gameObjectVariable = null;
			changedEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			if (gameObjectVariable.IsNone)
			{
				Finish();
			}
			else
			{
				previousValue = gameObjectVariable.Value;
			}
		}

		public override void OnUpdate()
		{
			storeResult.Value = false;
			if (gameObjectVariable.Value != previousValue)
			{
				storeResult.Value = true;
				base.Fsm.Event(changedEvent);
			}
		}
	}
}
