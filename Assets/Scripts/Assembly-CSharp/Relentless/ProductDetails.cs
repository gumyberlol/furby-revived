using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class ProductDetails
	{
		public string Name;

		public Guid BundleId;

		public Guid SkuId;

		public string StoreSpecificProductId;

		public bool IsComingSoon;

		public bool IsStoreHosted;

		public bool IsAvailable;

		public PriceDetails PriceDetails;

		public string NamedText_Title;

		public string NamedText_Description;

		public string PreferredCurrencyCode;

		public string PurchaseType;

		public List<string> ListOfBundledProducts;
	}
}
