using System;
using System.Collections.Generic;

namespace JsonFx.IO
{
	internal class ListStream<T> : Stream<T>
	{
		private bool isCompleted;

		private bool isReady;

		private T current;

		private readonly IList<T> Buffer;

		private int index = -1;

		private int start = -1;

		public override int ChunkSize
		{
			get
			{
				if (start < 0)
				{
					throw new InvalidOperationException("Not currently chunking.");
				}
				return 1 + index - start;
			}
		}

		public override bool IsChunking
		{
			get
			{
				return start >= 0;
			}
		}

		public override bool IsCompleted
		{
			get
			{
				EnsureReady();
				return isCompleted;
			}
		}

		public ListStream(IList<T> value)
		{
			Buffer = value ?? new T[0];
		}

		public override void BeginChunk()
		{
			start = index + 1;
		}

		public override IEnumerable<T> EndChunk()
		{
			if (start < 0)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			IEnumerable<T> result = new Subsequence<T>(Buffer, start, 1 + index - start);
			start = -1;
			return result;
		}

		public override T Peek()
		{
			EnsureReady();
			return current;
		}

		public override T Pop()
		{
			EnsureReady();
			if (isCompleted)
			{
				return current;
			}
			isReady = false;
			index++;
			return current;
		}

		private void EnsureReady()
		{
			if (isReady || isCompleted)
			{
				return;
			}
			isReady = true;
			int num = index + 1;
			SequenceBuffer<T> sequenceBuffer = Buffer as SequenceBuffer<T>;
			if (sequenceBuffer != null)
			{
				if (sequenceBuffer.TryAdvance(num))
				{
					current = Buffer[num];
					return;
				}
				isCompleted = true;
				current = default(T);
			}
			else if (num < Buffer.Count)
			{
				current = Buffer[num];
			}
			else
			{
				isCompleted = true;
				current = default(T);
			}
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
