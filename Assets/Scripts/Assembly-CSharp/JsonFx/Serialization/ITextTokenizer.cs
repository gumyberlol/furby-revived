using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	public interface ITextTokenizer<T> : IDisposable
	{
		int Column { get; }

		int Line { get; }

		long Index { get; }

		IEnumerable<Token<T>> GetTokens(TextReader reader);

		IEnumerable<Token<T>> GetTokens(string text);
	}
}
