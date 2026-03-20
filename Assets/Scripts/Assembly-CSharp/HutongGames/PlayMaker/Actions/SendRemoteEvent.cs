using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Send an Fsm Event on a remote machine. Uses Unity RPC functions.")]
	[ActionCategory(ActionCategory.Network)]
	public class SendRemoteEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NetworkView))]
		[Tooltip("The game object that sends the event.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The event you want to send.")]
		[RequiredField]
		public FsmEvent remoteEvent;

		[Tooltip("Optional string data. Use 'Get Event Info' action to retrieve it.")]
		public FsmString stringData;

		[Tooltip("Option for who will receive an RPC.")]
		public RPCMode mode;

		public override void Reset()
		{
			gameObject = null;
			remoteEvent = null;
			mode = RPCMode.All;
			stringData = null;
			mode = RPCMode.All;
		}

		public override void OnEnter()
		{
			DoRemoteEvent();
			Finish();
		}

		private void DoRemoteEvent()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(ownerDefaultTarget.networkView == null))
			{
				if (!stringData.IsNone && stringData.Value != string.Empty)
				{
					ownerDefaultTarget.networkView.RPC("SendRemoteFsmEvent", mode, remoteEvent.Name, stringData.Value);
				}
				else
				{
					ownerDefaultTarget.networkView.RPC("SendRemoteFsmEvent", mode, remoteEvent.Name);
				}
			}
		}
	}
}
