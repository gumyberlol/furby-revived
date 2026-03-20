using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class EqualityAdapter
	{
		private class ComparisonAdapterAdapter : EqualityAdapter
		{
			private ComparisonAdapter comparer;

			public ComparisonAdapterAdapter(ComparisonAdapter comparer)
			{
				this.comparer = comparer;
			}

			public override bool ObjectsEqual(object x, object y)
			{
				return comparer.Compare(x, y) == 0;
			}
		}

		public abstract bool ObjectsEqual(object x, object y);

		public static EqualityAdapter For(IComparer comparer)
		{
			return new ComparisonAdapterAdapter(ComparisonAdapter.For(comparer));
		}
	}
}
