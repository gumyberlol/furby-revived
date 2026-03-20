using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class SimpleNameValueStore
	{
		public Dictionary<string, object> DataDictionary = new Dictionary<string, object>();
	}
}
