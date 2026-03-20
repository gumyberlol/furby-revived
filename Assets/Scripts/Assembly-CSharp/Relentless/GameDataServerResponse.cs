using System;

namespace Relentless
{
	[Serializable]
	public class GameDataServerResponse
	{
		public string RequestId = string.Empty;

		public string FileUrl = string.Empty;

		public string FileUrlBase64 = string.Empty;

		public string Data;

		public string DataType;

		public AbTests AbTests;
	}
}
