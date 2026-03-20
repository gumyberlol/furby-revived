using System;
using System.Collections.Generic;

namespace JsonFx.IO
{
	internal class EnumerableStream<T> : Stream<T>
	{
		private const int InitialChunkCapacity = 16;

		private readonly IEnumerator<T> Enumerator;

		private bool isReady;

		private bool isCompleted;

		private T current;

		private List<T> chunk;

		public override bool IsCompleted
		{
			get
			{
				EnsureReady();
				return isCompleted;
			}
		}

		public override bool IsChunking
		{
			get
			{
				return chunk != null;
			}
		}

		public override int ChunkSize
		{
			get
			{
				if (chunk == null)
				{
					throw new InvalidOperationException("Not currently chunking.");
				}
				return chunk.Count;
			}
		}

		public EnumerableStream(IEnumerable<T> sequence)
		{
			if (sequence == null)
			{
				sequence = new T[0];
			}
			Enumerator = sequence.GetEnumerator();
		}

		public override T Peek()
		{
			EnsureReady();
			return current;
		}

		public override T Pop()
		{
			EnsureReady();
			isReady = isCompleted;
			return current;
		}

		public override void BeginChunk()
		{
			if (chunk == null)
			{
				chunk = new List<T>(16);
			}
			else
			{
				chunk.Clear();
			}
		}

		public override IEnumerable<T> EndChunk()
		{
			if (chunk == null)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			IEnumerable<T> result = chunk.AsReadOnly();
			chunk = null;
			return result;
		}

		private void EnsureReady()
		{
			if (isReady)
			{
				return;
			}
			isReady = true;
			isCompleted = !Enumerator.MoveNext();
			if (isCompleted)
			{
				current = default(T);
				return;
			}
			current = Enumerator.Current;
			if (chunk != null)
			{
				chunk.Add(current);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Enumerator.Dispose();
			}
		}
	}
}
