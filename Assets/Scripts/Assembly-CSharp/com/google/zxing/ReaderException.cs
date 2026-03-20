using System;

namespace com.google.zxing
{
	[Serializable]
	public sealed class ReaderException : Exception
	{
		private static readonly ReaderException instance = new ReaderException();

		public static ReaderException Instance
		{
			get
			{
				return instance;
			}
		}

		private ReaderException()
		{
		}

		public Exception fillInStackTrace()
		{
			return null;
		}
	}
}
