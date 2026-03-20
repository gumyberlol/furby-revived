using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Forces a Game Object's Rigid Body to Sleep at least one frame.")]
	[ActionCategory(ActionCategory.Physics)]
	public class Sleep : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			DoSleep();
			Finish();
		}

		private void DoSleep()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.rigidbody == null))
			{
				ownerDefaultTarget.rigidbody.Sleep();
			}
		}
	}
}
