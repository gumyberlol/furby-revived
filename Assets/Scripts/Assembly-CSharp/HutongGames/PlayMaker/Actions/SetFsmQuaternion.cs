using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Set the value of a Quaternion Variable in another FSM.")]
	public class SetFsmQuaternion : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.FsmQuaternion)]
		public FsmString variableName;

		[RequiredField]
		public FsmQuaternion setValue;

		public bool everyFrame;

		private GameObject goLastFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			fsmName = string.Empty;
			variableName = string.Empty;
			setValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetFsmQuaternion();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetFsmQuaternion()
		{
			if (setValue == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != goLastFrame)
			{
				goLastFrame = ownerDefaultTarget;
				fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
			}
			if (!(fsm == null))
			{
				FsmQuaternion fsmQuaternion = fsm.FsmVariables.GetFsmQuaternion(variableName.Value);
				if (fsmQuaternion != null)
				{
					fsmQuaternion.Value = setValue.Value;
				}
			}
		}

		public override void OnUpdate()
		{
			DoSetFsmQuaternion();
		}
	}
}
