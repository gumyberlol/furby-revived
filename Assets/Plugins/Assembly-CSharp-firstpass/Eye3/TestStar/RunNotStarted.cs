using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class RunNotStarted : RunException
	{
		public RunNotStarted()
		{
		}

		public RunNotStarted(string message)
			: base(message)
		{
		}

		public RunNotStarted(string message, Exception e)
			: base(message, e)
		{
		}

		protected RunNotStarted(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
