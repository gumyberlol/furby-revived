using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class MessageRequired : RunException
	{
		public MessageRequired()
		{
		}

		public MessageRequired(string message)
			: base(message)
		{
		}

		public MessageRequired(string message, Exception e)
			: base(message, e)
		{
		}

		protected MessageRequired(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
