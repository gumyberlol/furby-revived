using System;
using System.Runtime.Serialization;

namespace JsonFx.Serialization
{
	public class TypeCoercionException : ArgumentException
	{
		public TypeCoercionException()
		{
		}

		public TypeCoercionException(string message)
			: base(message)
		{
		}

		public TypeCoercionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public TypeCoercionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
