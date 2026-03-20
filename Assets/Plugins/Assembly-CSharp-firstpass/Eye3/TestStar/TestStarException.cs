using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class TestStarException : Exception
	{
		public TestStarException()
		{
		}

		public TestStarException(string message)
			: base(message)
		{
		}

		public TestStarException(string message, Exception e)
			: base(message, e)
		{
		}

		protected TestStarException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
