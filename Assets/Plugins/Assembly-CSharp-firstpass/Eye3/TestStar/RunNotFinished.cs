using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class RunNotFinished : RunException
	{
		public RunNotFinished()
		{
		}

		public RunNotFinished(string message)
			: base(message)
		{
		}

		public RunNotFinished(string message, Exception e)
			: base(message, e)
		{
		}

		protected RunNotFinished(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
