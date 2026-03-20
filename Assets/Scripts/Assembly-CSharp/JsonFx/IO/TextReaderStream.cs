using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFx.IO
{
	public class TextReaderStream : IDisposable, IStream<char>, ITextStream
	{
		private const int DefaultBufferSize = 32;

		public static readonly TextReaderStream Null = new TextReaderStream(TextReader.Null);

		private readonly TextReader Reader;

		private char current;

		private char prev;

		private bool isCompleted;

		private bool isReady;

		private int column;

		private int line;

		private long index;

		private CharBuffer chunk;

		public int Column
		{
			get
			{
				return column;
			}
		}

		public int Line
		{
			get
			{
				return line;
			}
		}

		public long Index
		{
			get
			{
				return index;
			}
		}

		public int ChunkSize
		{
			get
			{
				if (chunk == null)
				{
					throw new InvalidOperationException("Not currently chunking.");
				}
				return chunk.Length;
			}
		}

		public bool IsChunking
		{
			get
			{
				return chunk != null;
			}
		}

		public virtual bool IsCompleted
		{
			get
			{
				EnsureReady();
				return isCompleted;
			}
		}

		public TextReaderStream(TextReader reader)
		{
			Reader = reader;
			index = -1L;
		}

		IEnumerable<char> IStream<char>.EndChunk()
		{
			return EndChunk();
		}

		public void BeginChunk()
		{
			if (chunk == null)
			{
				chunk = new CharBuffer(32);
			}
			else
			{
				chunk.Clear();
			}
		}

		public string EndChunk()
		{
			if (chunk == null)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			string result = chunk.ToString();
			chunk = null;
			return result;
		}

		public void EndChunk(StringBuilder buffer)
		{
			if (chunk == null)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			chunk.CopyTo(buffer);
			chunk = null;
		}

		public virtual char Peek()
		{
			EnsureReady();
			return current;
		}

		public virtual char Pop()
		{
			EnsureReady();
			if (isCompleted)
			{
				return current;
			}
			if (chunk != null)
			{
				chunk.Append(current);
			}
			isReady = false;
			UpdateStats(prev, current);
			return prev = current;
		}

		private void EnsureReady()
		{
			if (!isReady && !isCompleted)
			{
				isReady = true;
				int num = Reader.Read();
				isCompleted = num < 0;
				if (isCompleted)
				{
					current = '\0';
				}
				else
				{
					current = (char)num;
				}
			}
		}

		private void UpdateStats(char prev, char value)
		{
			if (index < 0)
			{
				line = (column = 1);
				index = 0L;
				return;
			}
			switch (value)
			{
			case '\n':
				if (prev == '\r')
				{
					break;
				}
				goto case '\r';
			case '\r':
				line++;
				column = 0;
				break;
			default:
				column++;
				break;
			}
			index++;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Reader.Dispose();
			}
		}
	}
}
