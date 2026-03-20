using System;
using System.Collections;
using System.Collections.Generic;

namespace JsonFx.IO
{
	internal class SequenceBuffer<T> : IEnumerable<T>, IEnumerable, IList<T>, ICollection<T>
	{
		private sealed class Enumerator : IDisposable, IEnumerator<T>, IEnumerator
		{
			private readonly SequenceBuffer<T> Sequence;

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
					if (Index < 0)
					{
						throw new InvalidOperationException("Enumerator not started");
					}
					if (Index >= Sequence.Cache.Count)
					{
						throw new InvalidOperationException("Enumerator has ended");
					}
					return Sequence[Index];
				}
			}

			public Enumerator(SequenceBuffer<T> sequence)
			{
				Sequence = sequence;
				Index = -1;
			}

			void IDisposable.Dispose()
			{
			}

			public bool MoveNext()
			{
				int index = Index + 1;
				if (Sequence.TryAdvance(index))
				{
					Index = index;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				Index = -1;
			}
		}

		private readonly List<T> Cache;

		private readonly IEnumerator<T> Iterator;

		private bool isComplete;

		T IList<T>.this[int index]
		{
			get
			{
				TryAdvance(index);
				return Cache[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		int ICollection<T>.Count
		{
			get
			{
				SeekToEnd();
				return Cache.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				TryAdvance(index);
				return Cache[index];
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public SequenceBuffer(IEnumerable<T> sequence)
		{
			if (sequence == null)
			{
				sequence = new T[0];
			}
			Cache = new List<T>();
			Iterator = sequence.GetEnumerator();
		}

		int IList<T>.IndexOf(T item)
		{
			int num = 0;
			foreach (T item2 in (IEnumerable<T>)this)
			{
				if (object.Equals(item2, item))
				{
					return num;
				}
				num++;
			}
			return -1;
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

		bool ICollection<T>.Contains(T item)
		{
			foreach (T item2 in (IEnumerable<T>)this)
			{
				if (object.Equals(item2, item))
				{
					return true;
				}
			}
			return false;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			SeekToEnd();
			Cache.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public bool TryAdvance(int index)
		{
			if (index < Cache.Count)
			{
				return true;
			}
			if (isComplete)
			{
				return false;
			}
			while (index >= Cache.Count && Iterator.MoveNext())
			{
				Cache.Add(Iterator.Current);
			}
			isComplete = index >= Cache.Count;
			return !isComplete;
		}

		private void SeekToEnd()
		{
			while (Iterator.MoveNext())
			{
				Cache.Add(Iterator.Current);
			}
			isComplete = true;
		}
	}
}
