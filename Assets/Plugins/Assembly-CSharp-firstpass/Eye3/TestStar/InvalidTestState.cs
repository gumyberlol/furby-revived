using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class InvalidTestState : RunException
	{
		public InvalidTestState()
		{
		}

		public InvalidTestState(string message)
			: base(message)
		{
		}

		public InvalidTestState(string message, Exception e)
			: base(message, e)
		{
		}

		protected InvalidTestState(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
