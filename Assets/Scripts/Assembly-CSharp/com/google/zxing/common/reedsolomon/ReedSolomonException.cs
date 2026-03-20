using System;

namespace com.google.zxing.common.reedsolomon
{
	[Serializable]
	public sealed class ReedSolomonException : Exception
	{
		public ReedSolomonException(string message)
			: base(message)
		{
		}
	}
}
