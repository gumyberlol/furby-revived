using System;
using System.Runtime.Serialization;

namespace JsonFx.Serialization
{
	public class DeserializationException : SerializationException
	{
		private readonly int column = -1;

		private readonly int line = -1;

		private readonly long index = -1L;

		public int Column
		{
			get
			{
				return column;
			}
		}

		public long Index
		{
			get
			{
				return index;
			}
		}

		public int Line
		{
			get
			{
				return line;
			}
		}

		public DeserializationException(string message, long index)
			: this(message, index, -1, -1)
		{
		}

		public DeserializationException(string message, long index, int line, int column)
			: base(message)
		{
			this.column = column;
			this.line = line;
			this.index = index;
		}

		public DeserializationException(string message, long index, Exception innerException)
			: this(message, index, -1, -1, innerException)
		{
		}

		public DeserializationException(string message, long index, int line, int column, Exception innerException)
			: base(message, innerException)
		{
			this.column = column;
			this.line = line;
			this.index = index;
		}

		public DeserializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public void GetLineAndColumn(string source, out int line, out int col)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}
			col = 1;
			line = 1;
			bool flag = false;
			for (int num = Math.Min((int)index, source.Length); num > 0; num--)
			{
				if (!flag)
				{
					col++;
				}
				if (source[num - 1] == '\n')
				{
					line++;
					flag = true;
				}
			}
		}
	}
}
