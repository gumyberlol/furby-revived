using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network)]
	[Tooltip("Connect to a server.")]
	public class NetworkConnect : FsmStateAction
	{
		[Tooltip("IP address of the host. Either a dotted IP address or a domain name.")]
		[RequiredField]
		public FsmString remoteIP;

		[Tooltip("The port on the remote machine to connect to.")]
		[RequiredField]
		public FsmInt remotePort;

		[Tooltip("Optional password for the server.")]
		public FsmString password;

		[ActionSection("Errors")]
		[Tooltip("Event to send in case of an error connecting to the server.")]
		public FsmEvent errorEvent;

		[Tooltip("Store the error string in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmString errorString;

		public override void Reset()
		{
			remoteIP = "127.0.0.1";
			remotePort = 25001;
			password = string.Empty;
			errorEvent = null;
			errorString = null;
		}

		public override void OnEnter()
		{
			NetworkConnectionError networkConnectionError = Network.Connect(remoteIP.Value, remotePort.Value, password.Value);
			if (networkConnectionError != NetworkConnectionError.NoError)
			{
				errorString.Value = networkConnectionError.ToString();
				LogError(errorString.Value);
				base.Fsm.Event(errorEvent);
			}
			Finish();
		}
	}
}
