using System;

namespace Relentless
{
	[Serializable]
	public class ProductDetailsRequest : MessageRequestContent
	{
		public string Locale;

		public string CoutryCode;
	}
}
