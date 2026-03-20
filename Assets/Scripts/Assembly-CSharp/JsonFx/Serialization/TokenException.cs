using System;
using System.Runtime.Serialization;

namespace JsonFx.Serialization
{
	public class TokenException<T> : SerializationException
	{
		private readonly Token<T> token;

		public Token<T> Token
		{
			get
			{
				return token;
			}
		}

		public TokenException(Token<T> token)
		{
			this.token = token;
		}

		public TokenException(Token<T> token, string message)
			: base(message)
		{
			this.token = token;
		}

		public TokenException(Token<T> token, string message, Exception innerException)
			: base(message, innerException)
		{
			this.token = token;
		}

		public TokenException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
