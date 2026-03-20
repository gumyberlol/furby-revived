using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonAdapter
	{
		private class DefaultComparisonAdapter : ComparerAdapter
		{
			public DefaultComparisonAdapter()
				: base(NUnitComparer.Default)
			{
			}
		}

		private class ComparerAdapter : ComparisonAdapter
		{
			private IComparer comparer;

			public ComparerAdapter(IComparer comparer)
			{
				this.comparer = comparer;
			}

			public override int Compare(object expected, object actual)
			{
				return comparer.Compare(expected, actual);
			}
		}

		public static ComparisonAdapter Default
		{
			get
			{
				return new DefaultComparisonAdapter();
			}
		}

		public static ComparisonAdapter For(IComparer comparer)
		{
			return new ComparerAdapter(comparer);
		}

		public abstract int Compare(object expected, object actual);
	}
}
