using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class RunException : TestStarException
	{
		public RunException()
		{
		}

		public RunException(string message)
			: base(message)
		{
		}

		public RunException(string message, Exception e)
			: base(message, e)
		{
		}

		protected RunException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
