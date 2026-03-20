using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	public interface ITextFormatter<T>
	{
		void Format(IEnumerable<Token<T>> tokens, TextWriter writer);

		string Format(IEnumerable<Token<T>> tokens);
	}
}
