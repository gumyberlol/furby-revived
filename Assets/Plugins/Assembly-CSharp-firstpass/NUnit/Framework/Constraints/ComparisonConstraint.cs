using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonConstraint : Constraint
	{
		protected object expected;

		protected bool ltOK;

		protected bool eqOK;

		protected bool gtOK;

		private string predicate;

		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		public ComparisonConstraint(object value, bool ltOK, bool eqOK, bool gtOK, string predicate)
			: base(value)
		{
			expected = value;
			this.ltOK = ltOK;
			this.eqOK = eqOK;
			this.gtOK = gtOK;
			this.predicate = predicate;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (expected == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "expected");
			}
			if (actual == null)
			{
				throw new ArgumentException("Cannot compare to null reference", "actual");
			}
			int num = comparer.Compare(expected, actual);
			return (num < 0 && gtOK) || (num == 0 && eqOK) || (num > 0 && ltOK);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate(predicate);
			writer.WriteExpectedValue(expected);
		}

		public ComparisonConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
