using System.Collections.Generic;
using JsonFx.IO;

namespace JsonFx.Serialization.Filters
{
	public interface IDataFilterRS<TTokenType>
	{
		bool TryRead(DataReaderSettingsRS settings, IStream<Token<TTokenType>> tokens, out object value);

		bool TryWrite(DataWriterSettingsRS settings, object value, out IEnumerable<Token<TTokenType>> tokens);
	}
	public interface IDataFilterRS<TTokenType, TResult> : IDataFilterRS<TTokenType>
	{
		bool TryRead(DataReaderSettingsRS settings, IStream<Token<TTokenType>> tokens, out TResult value);

		bool TryWrite(DataWriterSettingsRS settings, TResult value, out IEnumerable<Token<TTokenType>> tokens);
	}
}
