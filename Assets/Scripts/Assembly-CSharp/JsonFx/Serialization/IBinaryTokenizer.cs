using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	public interface IBinaryTokenizer<T> : IDisposable
	{
		long Index { get; }

		IEnumerable<Token<T>> GetTokens(Stream stream);

		IEnumerable<Token<T>> GetTokens(byte[] bytes);
	}
}
