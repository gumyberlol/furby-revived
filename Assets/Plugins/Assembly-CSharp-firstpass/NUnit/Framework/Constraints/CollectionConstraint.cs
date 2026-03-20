using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class CollectionConstraint : Constraint
	{
		public CollectionConstraint()
		{
		}

		public CollectionConstraint(object arg)
			: base(arg)
		{
		}

		protected static bool IsEmpty(IEnumerable enumerable)
		{
			ICollection collection = enumerable as ICollection;
			if (collection != null)
			{
				return collection.Count == 0;
			}
			return !enumerable.GetEnumerator().MoveNext();
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			IEnumerable enumerable = actual as IEnumerable;
			if (enumerable == null)
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			return doMatch(enumerable);
		}

		protected abstract bool doMatch(IEnumerable collection);
	}
}
