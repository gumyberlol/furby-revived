using System;

namespace Relentless
{
	[Serializable]
	public abstract class IAPRequestBuilder
	{
		public StaticRequestDetails StaticRequestDetails;

		public StoreNames StoreName = StoreNames.DummyStore;

		public Platforms OS;

		public string OSVer = "6";
	}
}
