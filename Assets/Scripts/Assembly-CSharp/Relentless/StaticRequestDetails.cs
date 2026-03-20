using System;
using System.Security.Authentication;
using Relentless.Network.Security;

namespace Relentless
{
	[Serializable]
	public class StaticRequestDetails
	{
		public Servers Server = Servers.None;

		public Protocol Protocol = Protocol.https;

		public string ServerName = "bt01mobile-iap.cloudapp.net";

		public string SandboxServerName = "bt01mobile-iap.cloudapp.net";

		public ApiVersion ApiVersion;

		public int GameVersion = 3;

		public SslProtocols ServerProtocol = SslProtocols.Ssl3;
	}
}
