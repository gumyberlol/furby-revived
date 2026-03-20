using System;

namespace Relentless
{
	[Serializable]
	public class ValidateProductResponse : ProductDetailsResponse
	{
		public bool IsValidReceipt;
	}
}
