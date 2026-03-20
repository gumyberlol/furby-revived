using System;

namespace Relentless
{
	[Serializable]
	public class ValidateProductRequest : MessageRequestContent
	{
		public string Locale;

		public string StoreReceipt;

		public string ProductId;

		public string StoreSpecificProductId;
	}
}
