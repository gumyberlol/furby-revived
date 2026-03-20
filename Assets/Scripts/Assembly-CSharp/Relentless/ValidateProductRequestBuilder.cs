using System;

namespace Relentless
{
	[Serializable]
	public class ValidateProductRequestBuilder : IAPRequestBuilder
	{
		public bool IsSandbox;

		public override string ToString()
		{
			if (IsSandbox)
			{
				return string.Format("{0}://{1}/api/{2}/validateproduct/{3}/{4}/{5}/{6}/true", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, StoreName, OS, OSVer);
			}
			return string.Format("{0}://{1}/api/{2}/validateproduct/{3}/{4}/{5}/{6}/false", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, StoreName, OS, OSVer);
		}
	}
}
