using System.Collections.Generic;

namespace Relentless
{
	public interface IPersistedData
	{
		Dictionary<string, string> Data { get; }
	}
}
