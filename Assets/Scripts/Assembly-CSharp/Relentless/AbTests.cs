using System;
using System.Collections.Generic;
using System.Text;

namespace Relentless
{
	[Serializable]
	public class AbTests
	{
		public List<AbTest> Tests;

		public override string ToString()
		{
			if (Tests == null)
			{
				return "(null)";
			}
			if (Tests.Count == 0)
			{
				return "(none)";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AbTest test in Tests)
			{
				stringBuilder.AppendFormat("{0},", test.GroupName);
			}
			return stringBuilder.ToString();
		}
	}
}
