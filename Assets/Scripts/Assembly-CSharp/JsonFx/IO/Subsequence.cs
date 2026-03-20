using System;
using System.Collections;
using System.Collections.Generic;

namespace JsonFx.IO
{
	internal class Subsequence<T> : ICollection<T>, IList<T>, IEnumerable, IEnumerable<T>
	{
		private sealed class Enumerator : IDisposable, IEnumerator, IEnumerator<T>
		{
			private readonly IList<T> Items;

			private readonly int Start;

			private readonly int End;

			private int Index;

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public T Current
			{
				get
				{
					if (Index < Start)
					{
						throw new InvalidOperationException("Enumerator not started");
					}
					if (Index >= End)
					{
						throw new InvalidOperationException("Enumerator has ended");
					}
					return Items[Index];
				}
			}

			public Enumerator(IList<T> sequence, int start, int length)
			{
				Items = sequence;
				Index = start - 1;
				Start = start;
				End = start + length;
			}

			void IDisposable.Dispose()
			{
			}

			public bool MoveNext()
			{
				if (Index < End)
				{
					Index++;
					return Index < End;
				}
				return false;
			}

			public void Reset()
			{
				Index = Start - 1;
			}
		}

		private readonly IList<T> Items;

		private readonly int Start;

		private readonly int Size;

		T IList<T>.this[int index]
		{
			get
			{
				return Items[Start + index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public T this[int index]
		{
			get
			{
				return Items[Start + index];
			}
		}

		public int Count
		{
			get
			{
				return Size;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public Subsequence(IList<T> sequence, int start, int length)
		{
			if (sequence == null)
			{
				throw new ArgumentNullException("sequence");
			}
			if (start < 0 || start >= sequence.Count)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (length < 0 || length > start + sequence.Count)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			Subsequence<T> subsequence = sequence as Subsequence<T>;
			if (subsequence != null)
			{
				Items = subsequence.Items;
				Start = subsequence.Start + start;
				Size = length;
			}
			else
			{
				Items = sequence;
				Start = start;
				Size = length;
			}
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(Items, Start, Size);
		}

		public int IndexOf(T item)
		{
			T[] array = Items as T[];
			if (array != null)
			{
				return Array.FindIndex(array, Start, Size, (T n) => object.Equals(n, item));
			}
			List<T> list = Items as List<T>;
			if (list != null)
			{
				return list.FindIndex(Start, Size, (T n) => object.Equals(n, item));
			}
			int num = Start + Size;
			for (int num2 = Start; num2 < num; num2++)
			{
				if (object.Equals(Items[num2], item))
				{
					return num2;
				}
			}
			return -1;
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] dest, int arrayIndex)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (Size + arrayIndex > dest.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			T[] array = Items as T[];
			if (array != null)
			{
				Array.Copy(array, Start, dest, arrayIndex, Size);
				return;
			}
			List<T> list = Items as List<T>;
			if (list != null)
			{
				list.CopyTo(Start, dest, arrayIndex, Size);
				return;
			}
			int num = arrayIndex;
			int num2 = Start;
			int num3 = arrayIndex + Size;
			while (num < num3)
			{
				dest[num] = Items[num2];
				num++;
				num2++;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(Items, Start, Size);
		}
	}
}
