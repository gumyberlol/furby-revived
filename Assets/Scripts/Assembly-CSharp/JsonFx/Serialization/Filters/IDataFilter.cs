using System.Collections.Generic;
using JsonFx.IO;

namespace JsonFx.Serialization.Filters
{
	public interface IDataFilter<TTokenType>
	{
		bool TryRead(DataReaderSettings settings, IStream<Token<TTokenType>> tokens, out object value);

		bool TryWrite(DataWriterSettings settings, object value, out IEnumerable<Token<TTokenType>> tokens);
	}
	public interface IDataFilter<TTokenType, TResult> : IDataFilter<TTokenType>
	{
		bool TryRead(DataReaderSettings settings, IStream<Token<TTokenType>> tokens, out TResult value);

		bool TryWrite(DataWriterSettings settings, TResult value, out IEnumerable<Token<TTokenType>> tokens);
	}
}
