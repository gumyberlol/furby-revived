using System;
using System.Text;

namespace JsonFx.IO
{
	public interface ITextStream : IDisposable, IStream<char>
	{
		int Column { get; }

		int Line { get; }

		long Index { get; }

		new string EndChunk();

		void EndChunk(StringBuilder buffer);
	}
}
