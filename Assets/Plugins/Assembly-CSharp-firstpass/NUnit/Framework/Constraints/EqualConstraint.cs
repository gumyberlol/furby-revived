using System;
using System.Collections;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EqualConstraint : Constraint
	{
		private readonly object expected;

		private bool clipStrings = true;

		private NUnitEqualityComparer comparer = new NUnitEqualityComparer();

		private static readonly string StringsDiffer_1 = "String lengths are both {0}. Strings differ at index {1}.";

		private static readonly string StringsDiffer_2 = "Expected string length {0} but was {1}. Strings differ at index {2}.";

		private static readonly string StreamsDiffer_1 = "Stream lengths are both {0}. Streams differ at offset {1}.";

		private static readonly string StreamsDiffer_2 = "Expected Stream length {0} but was {1}.";

		private static readonly string CollectionType_1 = "Expected and actual are both {0}";

		private static readonly string CollectionType_2 = "Expected is {0}, actual is {1}";

		private static readonly string ValuesDiffer_1 = "Values differ at index {0}";

		private static readonly string ValuesDiffer_2 = "Values differ at expected index {0}, actual index {1}";

		public EqualConstraint IgnoreCase
		{
			get
			{
				comparer.IgnoreCase = true;
				return this;
			}
		}

		public EqualConstraint NoClip
		{
			get
			{
				clipStrings = false;
				return this;
			}
		}

		public EqualConstraint AsCollection
		{
			get
			{
				comparer.CompareAsCollection = true;
				return this;
			}
		}

		public EqualConstraint Ulps
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Ulps;
				return this;
			}
		}

		public EqualConstraint Percent
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Percent;
				return this;
			}
		}

		public EqualConstraint Days
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Days;
				return this;
			}
		}

		public EqualConstraint Hours
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Hours;
				return this;
			}
		}

		public EqualConstraint Minutes
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Minutes;
				return this;
			}
		}

		public EqualConstraint Seconds
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Seconds;
				return this;
			}
		}

		public EqualConstraint Milliseconds
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Milliseconds;
				return this;
			}
		}

		public EqualConstraint Ticks
		{
			get
			{
				comparer.Tolerance = comparer.Tolerance.Ticks;
				return this;
			}
		}

		public EqualConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public EqualConstraint Within(object amount)
		{
			if (!comparer.Tolerance.IsEmpty)
			{
				throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");
			}
			comparer.Tolerance = new Tolerance(amount);
			return this;
		}

		[Obsolete("Replace with 'Using'")]
		public EqualConstraint Comparer(IComparer comparer)
		{
			return Using(comparer);
		}

		public EqualConstraint Using(IComparer comparer)
		{
			this.comparer.ExternalComparer = EqualityAdapter.For(comparer);
			return this;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return comparer.ObjectsEqual(expected, actual);
		}

		public override void WriteMessageTo(MessageWriter writer)
		{
			DisplayDifferences(writer, expected, actual, 0);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WriteExpectedValue(expected);
			if (comparer.Tolerance != null && !comparer.Tolerance.IsEmpty)
			{
				writer.WriteConnector("+/-");
				writer.WriteExpectedValue(comparer.Tolerance.Value);
			}
			if (comparer.IgnoreCase)
			{
				writer.WriteModifier("ignoring case");
			}
		}

		private void DisplayDifferences(MessageWriter writer, object expected, object actual, int depth)
		{
			if (expected is string && actual is string)
			{
				DisplayStringDifferences(writer, (string)expected, (string)actual);
			}
			else if (expected is ICollection && actual is ICollection)
			{
				DisplayCollectionDifferences(writer, (ICollection)expected, (ICollection)actual, depth);
			}
			else if (expected is Stream && actual is Stream)
			{
				DisplayStreamDifferences(writer, (Stream)expected, (Stream)actual, depth);
			}
			else if (comparer.Tolerance != null)
			{
				writer.DisplayDifferences(expected, actual, comparer.Tolerance);
			}
			else
			{
				writer.DisplayDifferences(expected, actual);
			}
		}

		private void DisplayStringDifferences(MessageWriter writer, string expected, string actual)
		{
			int num = MsgUtils.FindMismatchPosition(expected, actual, 0, comparer.IgnoreCase);
			if (expected.Length == actual.Length)
			{
				writer.WriteMessageLine(StringsDiffer_1, expected.Length, num);
			}
			else
			{
				writer.WriteMessageLine(StringsDiffer_2, expected.Length, actual.Length, num);
			}
			writer.DisplayStringDifferences(expected, actual, num, comparer.IgnoreCase, clipStrings);
		}

		private void DisplayStreamDifferences(MessageWriter writer, Stream expected, Stream actual, int depth)
		{
			if (expected.Length == actual.Length)
			{
				long num = (long)comparer.FailurePoints[depth];
				writer.WriteMessageLine(StreamsDiffer_1, expected.Length, num);
			}
			else
			{
				writer.WriteMessageLine(StreamsDiffer_2, expected.Length, actual.Length);
			}
		}

		private void DisplayCollectionDifferences(MessageWriter writer, ICollection expected, ICollection actual, int depth)
		{
			int num = ((comparer.FailurePoints.Count <= depth) ? (-1) : ((int)comparer.FailurePoints[depth]));
			DisplayCollectionTypesAndSizes(writer, expected, actual, depth);
			if (num >= 0)
			{
				DisplayFailurePoint(writer, expected, actual, num, depth);
				if (num < expected.Count && num < actual.Count)
				{
					DisplayDifferences(writer, GetValueFromCollection(expected, num), GetValueFromCollection(actual, num), ++depth);
				}
				else if (expected.Count < actual.Count)
				{
					writer.Write("  Extra:    ");
					writer.WriteCollectionElements(actual, num, 3);
				}
				else
				{
					writer.Write("  Missing:  ");
					writer.WriteCollectionElements(expected, num, 3);
				}
			}
		}

		private void DisplayCollectionTypesAndSizes(MessageWriter writer, ICollection expected, ICollection actual, int indent)
		{
			string text = MsgUtils.GetTypeRepresentation(expected);
			if (!(expected is Array))
			{
				text += string.Format(" with {0} elements", expected.Count);
			}
			string text2 = MsgUtils.GetTypeRepresentation(actual);
			if (!(actual is Array))
			{
				text2 += string.Format(" with {0} elements", actual.Count);
			}
			if (text == text2)
			{
				writer.WriteMessageLine(indent, CollectionType_1, text);
			}
			else
			{
				writer.WriteMessageLine(indent, CollectionType_2, text, text2);
			}
		}

		private void DisplayFailurePoint(MessageWriter writer, ICollection expected, ICollection actual, int failurePoint, int indent)
		{
			Array array = expected as Array;
			Array array2 = actual as Array;
			int num = ((array == null) ? 1 : array.Rank);
			int num2 = ((array2 == null) ? 1 : array2.Rank);
			bool flag = num == num2;
			if (array != null && array2 != null)
			{
				for (int i = 1; i < num; i++)
				{
					if (!flag)
					{
						break;
					}
					if (array.GetLength(i) != array2.GetLength(i))
					{
						flag = false;
					}
				}
			}
			int[] arrayIndicesFromCollectionIndex = MsgUtils.GetArrayIndicesFromCollectionIndex(expected, failurePoint);
			if (flag)
			{
				writer.WriteMessageLine(indent, ValuesDiffer_1, MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex));
				return;
			}
			int[] arrayIndicesFromCollectionIndex2 = MsgUtils.GetArrayIndicesFromCollectionIndex(actual, failurePoint);
			writer.WriteMessageLine(indent, ValuesDiffer_2, MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex), MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex2));
		}

		private static object GetValueFromCollection(ICollection collection, int index)
		{
			Array array = collection as Array;
			if (array != null && array.Rank > 1)
			{
				return array.GetValue(MsgUtils.GetArrayIndicesFromCollectionIndex(array, index));
			}
			if (collection is IList)
			{
				return ((IList)collection)[index];
			}
			foreach (object item in collection)
			{
				if (--index < 0)
				{
					return item;
				}
			}
			return null;
		}
	}
}
