using System;

namespace com.google.zxing
{
	[Serializable]
	public sealed class WriterException : Exception
	{
		public WriterException()
		{
		}

		public WriterException(string message)
			: base(message)
		{
		}
	}
}
