using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Remove the RPC function calls accociated with a Game Object.\n\nNOTE: The Game Object must have a NetworkView component attached.")]
	[ActionCategory(ActionCategory.Network)]
	public class NetworkViewRemoveRPCs : FsmStateAction
	{
		[Tooltip("Remove the RPC function calls accociated with this Game Object.\n\nNOTE: The GameObject must have a NetworkView component attached.")]
		[CheckForComponent(typeof(NetworkView))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			DoRemoveRPCsFromViewID();
			Finish();
		}

		private void DoRemoveRPCsFromViewID()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.networkView == null))
			{
				Network.RemoveRPCs(ownerDefaultTarget.networkView.viewID);
			}
		}
	}
}
