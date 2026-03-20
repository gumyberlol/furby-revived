using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class CollectionItemsEqualConstraint : CollectionConstraint
	{
		internal NUnitEqualityComparer comparer = NUnitEqualityComparer.Default;

		public CollectionItemsEqualConstraint IgnoreCase
		{
			get
			{
				comparer.IgnoreCase = true;
				return this;
			}
		}

		public CollectionItemsEqualConstraint()
		{
		}

		public CollectionItemsEqualConstraint(object arg)
			: base(arg)
		{
		}

		public CollectionItemsEqualConstraint Using(IComparer comparer)
		{
			this.comparer.ExternalComparer = EqualityAdapter.For(comparer);
			return this;
		}

		protected bool ItemsEqual(object x, object y)
		{
			return comparer.ObjectsEqual(x, y);
		}

		protected CollectionTally Tally(IEnumerable c)
		{
			return new CollectionTally(comparer, c);
		}
	}
}
