using System.Collections.Generic;
using JsonFx.IO;

namespace JsonFx.Serialization.Filters
{
	public abstract class DataFilter<TTokenType, TResult> : IDataFilter<TTokenType, TResult>, IDataFilter<TTokenType>
	{
		bool IDataFilter<TTokenType>.TryRead(DataReaderSettings settings, IStream<Token<TTokenType>> tokens, out object value)
		{
			TResult value2;
			bool result = TryRead(settings, tokens, out value2);
			value = value2;
			return result;
		}

		bool IDataFilter<TTokenType>.TryWrite(DataWriterSettings settings, object value, out IEnumerable<Token<TTokenType>> tokens)
		{
			tokens = null;
			return value is TResult && TryWrite(settings, (TResult)value, out tokens);
		}

		public abstract bool TryRead(DataReaderSettings settings, IStream<Token<TTokenType>> tokens, out TResult value);

		public abstract bool TryWrite(DataWriterSettings settings, TResult value, out IEnumerable<Token<TTokenType>> tokens);
	}
}
