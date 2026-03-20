using System.Collections.Generic;

namespace JsonFx.Serialization
{
	public interface IDataTransformer<TIn, TOut>
	{
		IEnumerable<Token<TOut>> Transform(IEnumerable<Token<TIn>> input);
	}
}
