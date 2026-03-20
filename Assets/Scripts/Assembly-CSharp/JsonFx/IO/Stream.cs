using System;
using System.Collections.Generic;

namespace JsonFx.IO
{
	public abstract class Stream<T> : IDisposable, IStream<T>
	{
		public static readonly Stream<T> Null = new ListStream<T>(null);

		public abstract bool IsCompleted { get; }

		public abstract bool IsChunking { get; }

		public abstract int ChunkSize { get; }

		public static IStream<T> Create(IEnumerable<T> sequence)
		{
			return Create(sequence, false);
		}

		public static IStream<T> Create(IEnumerable<T> sequence, bool buffered)
		{
			IList<T> list = sequence as IList<T>;
			if (list != null)
			{
				return new ListStream<T>(list);
			}
			if (buffered)
			{
				list = new SequenceBuffer<T>(sequence);
				return new ListStream<T>(list);
			}
			return new EnumerableStream<T>(sequence);
		}

		public abstract T Peek();

		public abstract T Pop();

		public abstract void BeginChunk();

		public abstract IEnumerable<T> EndChunk();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);
	}
}
