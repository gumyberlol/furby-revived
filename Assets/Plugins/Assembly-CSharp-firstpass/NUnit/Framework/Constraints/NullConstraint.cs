using UnityEngine;

namespace NUnit.Framework.Constraints
{
	public class NullConstraint : BasicConstraint
	{
		public NullConstraint()
			: base(null, "null")
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (base.actual is Object)
			{
				return actual.Equals(expected);
			}
			if (expected is Object)
			{
				return expected.Equals(actual);
			}
			return base.Matches(actual);
		}
	}
}
