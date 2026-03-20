using System;
using System.Text;

namespace JsonFx.IO
{
	internal class CharBuffer
	{
		private const int DefaultCapacity = 32;

		private static readonly char[] EmptyBuffer = new char[0];

		private char[] buffer;

		private int size;

		public int Length
		{
			get
			{
				return size;
			}
		}

		public CharBuffer()
		{
			buffer = EmptyBuffer;
		}

		public CharBuffer(int capacity)
		{
			buffer = new char[capacity];
		}

		public void Clear()
		{
			size = 0;
		}

		public CharBuffer Append(char value)
		{
			if (size <= buffer.Length)
			{
				EnsureCapacity(size + 1);
			}
			buffer[size++] = value;
			return this;
		}

		public CharBuffer Append(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return this;
			}
			int length = value.Length;
			if (size <= buffer.Length)
			{
				EnsureCapacity(size + length);
			}
			for (int i = 0; i < length; i++)
			{
				buffer[size++] = value[i];
			}
			return this;
		}

		public void CopyTo(StringBuilder buffer)
		{
			if (size >= 1)
			{
				buffer.Append(this.buffer, 0, size);
			}
		}

		private void EnsureCapacity(int min)
		{
			int num = buffer.Length;
			if (num < min)
			{
				int num2 = Math.Max(Math.Max(32, num * 2), min);
				char[] destinationArray = new char[num2];
				if (size > 0)
				{
					Array.Copy(buffer, 0, destinationArray, 0, size);
				}
				buffer = destinationArray;
			}
		}

		public override string ToString()
		{
			return new string(buffer, 0, size);
		}
	}
}
