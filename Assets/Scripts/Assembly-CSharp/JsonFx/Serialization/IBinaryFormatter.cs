using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	public interface IBinaryFormatter<T>
	{
		void Format(IEnumerable<Token<T>> tokens, Stream stream);

		byte[] Format(IEnumerable<Token<T>> tokens);
	}
}
