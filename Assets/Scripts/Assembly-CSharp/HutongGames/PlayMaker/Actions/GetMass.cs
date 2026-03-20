using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets the Mass of a Game Object's Rigid Body.")]
	public class GetMass : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			DoGetMass();
			Finish();
		}

		private void DoGetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.rigidbody == null))
			{
				storeResult.Value = ownerDefaultTarget.rigidbody.mass;
			}
		}
	}
}
