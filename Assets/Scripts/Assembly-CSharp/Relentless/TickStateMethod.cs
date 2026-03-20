using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[ActionCategory("Relentless")]
	[HutongGames.PlayMaker.Tooltip("Invoke a method each frame")]
	public class TickStateMethod : FsmStateAction
	{
		private delegate void StateUpdateDelegate(bool b);

		[RequiredField]
		public GameObject TargetObject;

		[RequiredField]
		[UIHint(UIHint.Script)]
		public string TargetComponent;

		[UIHint(UIHint.Method)]
		[RequiredField]
		public string TargetMethod;

		public bool TransitionOnly;

		private StateUpdateDelegate StateUpdate;

		public override void OnEnter()
		{
			Type typeFromHandle = typeof(StateUpdateDelegate);
			Component component = TargetObject.GetComponent(TargetComponent);
			Delegate obj = Delegate.CreateDelegate(typeFromHandle, component, TargetMethod);
			StateUpdate = (StateUpdateDelegate)obj;
			StateUpdate(true);
			if (TransitionOnly)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			StateUpdate(false);
		}
	}
}
