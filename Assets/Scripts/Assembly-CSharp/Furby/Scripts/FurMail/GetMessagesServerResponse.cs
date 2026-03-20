using System;
using System.Collections.Generic;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class GetMessagesServerResponse
	{
		public string RequestId = string.Empty;

		public List<FurMailServerMessage> Messages;
	}
}
