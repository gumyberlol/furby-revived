using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
	[ActionCategory(ActionCategory.Physics)]
	public class UseGravity : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool useGravity;

		public override void Reset()
		{
			gameObject = null;
			useGravity = true;
		}

		public override void OnEnter()
		{
			DoUseGravity();
			Finish();
		}

		private void DoUseGravity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.rigidbody == null))
			{
				ownerDefaultTarget.rigidbody.useGravity = useGravity.Value;
			}
		}
	}
}
