using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Destroy the object across the network.\n\nThe object is destroyed locally and remotely.\n\nOptionally remove any RPCs accociated with the object.")]
	[ActionCategory(ActionCategory.Network)]
	public class NetworkDestroy : FsmStateAction
	{
		[Tooltip("The Game Object to destroy.\nNOTE: The Game Object must have a NetworkView attached.")]
		[CheckForComponent(typeof(NetworkView))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("Remove all RPC calls associated with the Game Object.")]
		public FsmBool removeRPCs;

		public override void Reset()
		{
			gameObject = null;
			removeRPCs = true;
		}

		public override void OnEnter()
		{
			DoDestroy();
			Finish();
		}

		private void DoDestroy()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.networkView == null))
			{
				if (removeRPCs.Value)
				{
					Network.RemoveRPCs(ownerDefaultTarget.networkView.owner);
				}
				Network.DestroyPlayerObjects(ownerDefaultTarget.networkView.owner);
			}
		}
	}
}
