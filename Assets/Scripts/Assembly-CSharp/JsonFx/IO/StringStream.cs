using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFx.IO
{
	public class StringStream : IDisposable, IStream<char>, ITextStream
	{
		public static readonly StringStream Null = new StringStream(null);

		private bool isCompleted;

		private bool isReady;

		private char current;

		private int column;

		private int line;

		private int index = -1;

		private readonly string Buffer;

		private int start = -1;

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
				if (start < 0)
				{
					throw new InvalidOperationException("Not currently chunking.");
				}
				return 1 + index - start;
			}
		}

		public bool IsChunking
		{
			get
			{
				return start >= 0;
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

		public StringStream(string value)
		{
			Buffer = value ?? string.Empty;
		}

		IEnumerable<char> IStream<char>.EndChunk()
		{
			return EndChunk();
		}

		public void BeginChunk()
		{
			start = index + 1;
		}

		public string EndChunk()
		{
			if (start < 0)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			string result = Buffer.Substring(start, 1 + index - start);
			start = -1;
			return result;
		}

		public void EndChunk(StringBuilder buffer)
		{
			if (start < 0)
			{
				throw new InvalidOperationException("Not currently chunking.");
			}
			buffer.Append(Buffer, start, 1 + index - start);
			start = -1;
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
			isReady = false;
			UpdateStats();
			return current;
		}

		private void EnsureReady()
		{
			if (!isReady)
			{
				isReady = true;
				int num = index + 1;
				if (num < Buffer.Length)
				{
					isCompleted = false;
					current = Buffer[num];
				}
				else
				{
					isCompleted = true;
					current = '\0';
				}
			}
		}

		private void UpdateStats()
		{
			if (index < 0)
			{
				line = (column = 1);
				index = 0;
				return;
			}
			switch (current)
			{
			case '\n':
				if (Buffer[index] == '\r')
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
		}
	}
}
