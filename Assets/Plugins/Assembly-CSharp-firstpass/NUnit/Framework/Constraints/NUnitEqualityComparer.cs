using System;
using System.Collections;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class NUnitEqualityComparer
	{
		private bool caseInsensitive;

		private bool compareAsCollection;

		private Tolerance tolerance = Tolerance.Empty;

		private EqualityAdapter externalComparer;

		private ArrayList failurePoints;

		private static readonly int BUFFER_SIZE = 4096;

		public static NUnitEqualityComparer Default
		{
			get
			{
				return new NUnitEqualityComparer();
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return caseInsensitive;
			}
			set
			{
				caseInsensitive = value;
			}
		}

		public bool CompareAsCollection
		{
			get
			{
				return compareAsCollection;
			}
			set
			{
				compareAsCollection = value;
			}
		}

		public EqualityAdapter ExternalComparer
		{
			get
			{
				return externalComparer;
			}
			set
			{
				externalComparer = value;
			}
		}

		public Tolerance Tolerance
		{
			get
			{
				return tolerance;
			}
			set
			{
				tolerance = value;
			}
		}

		public IList FailurePoints
		{
			get
			{
				return failurePoints;
			}
		}

		public bool ObjectsEqual(object x, object y)
		{
			failurePoints = new ArrayList();
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			Type type = x.GetType();
			Type type2 = y.GetType();
			if (type.IsArray && type2.IsArray && !compareAsCollection)
			{
				return ArraysEqual((Array)x, (Array)y);
			}
			if (x is IDictionary && y is IDictionary)
			{
				return DictionariesEqual((IDictionary)x, (IDictionary)y);
			}
			if (x is ICollection && y is ICollection)
			{
				return CollectionsEqual((ICollection)x, (ICollection)y);
			}
			if (x is IEnumerable && y is IEnumerable && (!(x is string) || !(y is string)))
			{
				return EnumerablesEqual((IEnumerable)x, (IEnumerable)y);
			}
			if (externalComparer != null)
			{
				return externalComparer.ObjectsEqual(x, y);
			}
			if (x is string && y is string)
			{
				return StringsEqual((string)x, (string)y);
			}
			if (x is Stream && y is Stream)
			{
				return StreamsEqual((Stream)x, (Stream)y);
			}
			if (x is DirectoryInfo && y is DirectoryInfo)
			{
				return DirectoriesEqual((DirectoryInfo)x, (DirectoryInfo)y);
			}
			if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
			{
				return Numerics.AreEqual(x, y, ref tolerance);
			}
			if (tolerance != null && tolerance.Value is TimeSpan)
			{
				TimeSpan timeSpan = (TimeSpan)tolerance.Value;
				if (x is DateTime && y is DateTime)
				{
					return ((DateTime)x - (DateTime)y).Duration() <= timeSpan;
				}
				if (x is TimeSpan && y is TimeSpan)
				{
					return ((TimeSpan)x - (TimeSpan)y).Duration() <= timeSpan;
				}
			}
			return x.Equals(y);
		}

		private bool ArraysEqual(Array x, Array y)
		{
			int rank = x.Rank;
			if (rank != y.Rank)
			{
				return false;
			}
			for (int i = 1; i < rank; i++)
			{
				if (x.GetLength(i) != y.GetLength(i))
				{
					return false;
				}
			}
			return CollectionsEqual(x, y);
		}

		private bool DictionariesEqual(IDictionary x, IDictionary y)
		{
			if (x.Count != y.Count)
			{
				return false;
			}
			CollectionTally collectionTally = new CollectionTally(this, x.Keys);
			if (!collectionTally.TryRemove(y.Keys) || collectionTally.Count > 0)
			{
				return false;
			}
			foreach (object key in x.Keys)
			{
				if (!ObjectsEqual(x[key], y[key]))
				{
					return false;
				}
			}
			return true;
		}

		private bool CollectionsEqual(ICollection x, ICollection y)
		{
			IEnumerator enumerator = x.GetEnumerator();
			IEnumerator enumerator2 = y.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext() && enumerator2.MoveNext() && ObjectsEqual(enumerator.Current, enumerator2.Current))
			{
				num++;
			}
			if (num == x.Count && num == y.Count)
			{
				return true;
			}
			failurePoints.Insert(0, num);
			return false;
		}

		private bool StringsEqual(string x, string y)
		{
			string text = ((!caseInsensitive) ? x : x.ToLower());
			string value = ((!caseInsensitive) ? y : y.ToLower());
			return text.Equals(value);
		}

		private bool EnumerablesEqual(IEnumerable x, IEnumerable y)
		{
			IEnumerator enumerator = x.GetEnumerator();
			IEnumerator enumerator2 = y.GetEnumerator();
			int num = 0;
			bool flag;
			bool flag2;
			do
			{
				flag = enumerator.MoveNext();
				flag2 = enumerator2.MoveNext();
				if (!flag && !flag2)
				{
					return true;
				}
			}
			while (flag == flag2 && ObjectsEqual(enumerator.Current, enumerator2.Current));
			failurePoints.Insert(0, num);
			return false;
		}

		private bool DirectoriesEqual(DirectoryInfo x, DirectoryInfo y)
		{
			if (x.Attributes != y.Attributes || x.CreationTime != y.CreationTime || x.LastAccessTime != y.LastAccessTime)
			{
				return false;
			}
			return new SamePathConstraint(x.FullName).Matches(y.FullName);
		}

		private bool StreamsEqual(Stream x, Stream y)
		{
			if (x == y)
			{
				return true;
			}
			if (!x.CanRead)
			{
				throw new ArgumentException("Stream is not readable", "expected");
			}
			if (!y.CanRead)
			{
				throw new ArgumentException("Stream is not readable", "actual");
			}
			if (!x.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "expected");
			}
			if (!y.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "actual");
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			byte[] array = new byte[BUFFER_SIZE];
			byte[] array2 = new byte[BUFFER_SIZE];
			BinaryReader binaryReader = new BinaryReader(x);
			BinaryReader binaryReader2 = new BinaryReader(y);
			long position = x.Position;
			long position2 = y.Position;
			try
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				binaryReader2.BaseStream.Seek(0L, SeekOrigin.Begin);
				for (long num = 0L; num < x.Length; num += BUFFER_SIZE)
				{
					binaryReader.Read(array, 0, BUFFER_SIZE);
					binaryReader2.Read(array2, 0, BUFFER_SIZE);
					for (int i = 0; i < BUFFER_SIZE; i++)
					{
						if (array[i] != array2[i])
						{
							failurePoints.Insert(0, num + i);
							return false;
						}
					}
				}
			}
			finally
			{
				x.Position = position;
				y.Position = position2;
			}
			return true;
		}
	}
}
