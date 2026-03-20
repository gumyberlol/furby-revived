using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class ProductDetailsResponse
	{
		public string RequestId { get; set; }

		public string Store { get; set; }

		public List<ProductDetails> ProductDetails { get; set; }
	}
}
