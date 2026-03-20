using System;
using System.Runtime.Serialization;

namespace JsonFx.Serialization
{
	public class SerializationException : InvalidOperationException
	{
		public SerializationException()
		{
		}

		public SerializationException(string message)
			: base(message)
		{
		}

		public SerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public SerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
