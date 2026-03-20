using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class NoTestFixtureToRun : TestStarException
	{
		public NoTestFixtureToRun()
		{
		}

		public NoTestFixtureToRun(string message)
			: base(message)
		{
		}

		public NoTestFixtureToRun(string message, Exception e)
			: base(message, e)
		{
		}

		protected NoTestFixtureToRun(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
