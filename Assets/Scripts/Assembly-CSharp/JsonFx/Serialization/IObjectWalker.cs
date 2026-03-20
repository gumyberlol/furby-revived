using System.Collections.Generic;

namespace JsonFx.Serialization
{
	public interface IObjectWalker<T>
	{
		IEnumerable<Token<T>> GetTokens(object value);
	}
}
