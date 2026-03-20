using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Forces a Game Object's Rigid Body to wake up.")]
	[ActionCategory(ActionCategory.Physics)]
	public class WakeUp : FsmStateAction
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
			DoWakeUp();
			Finish();
		}

		private void DoWakeUp()
		{
			GameObject gameObject = ((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
			if (!(gameObject == null) && !(gameObject.rigidbody == null))
			{
				gameObject.rigidbody.WakeUp();
			}
		}
	}
}
