using System;
using System.Collections.Generic;

namespace JsonFx.IO
{
	public interface IStream<T> : IDisposable
	{
		bool IsCompleted { get; }

		bool IsChunking { get; }

		int ChunkSize { get; }

		T Peek();

		T Pop();

		void BeginChunk();

		IEnumerable<T> EndChunk();
	}
}
