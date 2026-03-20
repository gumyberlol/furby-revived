using System;

namespace Relentless
{
	[Serializable]
	public class ProductDetailsRequestBuilder : IAPRequestBuilder
	{
		public override string ToString()
		{
			return string.Format("{0}://{1}/api/{2}/products/{3}/{4}/{5}/{6}", StaticRequestDetails.Protocol, StaticRequestDetails.ServerName, StaticRequestDetails.ApiVersion, StaticRequestDetails.GameVersion, StoreName, OS, OSVer);
		}
	}
}
