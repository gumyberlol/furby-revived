using System;
using System.Collections;
using System.Collections.Generic;

namespace Furby
{
	internal struct SlangActionsEnumerable : IEnumerable<string>, IEnumerable
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<string> GetEnumerator()
		{
			string[] names = Enum.GetNames(typeof(FurbyAction));
			foreach (string s in names)
			{
				if (s.StartsWith("Slang_"))
				{
					yield return s;
				}
			}
		}
	}
}
