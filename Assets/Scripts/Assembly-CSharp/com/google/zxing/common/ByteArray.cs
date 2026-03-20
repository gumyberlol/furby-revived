using System;

namespace com.google.zxing.common
{
	public sealed class ByteArray
	{
		private const int INITIAL_SIZE = 32;

		private sbyte[] bytes;

		private int size_Renamed_Field;

		public bool Empty
		{
			get
			{
				return size_Renamed_Field == 0;
			}
		}

		public ByteArray()
		{
			bytes = null;
			size_Renamed_Field = 0;
		}

		public ByteArray(int size)
		{
			bytes = new sbyte[size];
			size_Renamed_Field = size;
		}

		public ByteArray(sbyte[] byteArray)
		{
			bytes = byteArray;
			size_Renamed_Field = bytes.Length;
		}

		public int at(int index)
		{
			return bytes[index] & 0xFF;
		}

		public void set_Renamed(int index, int value_Renamed)
		{
			bytes[index] = (sbyte)value_Renamed;
		}

		public int size()
		{
			return size_Renamed_Field;
		}

		public void appendByte(int value_Renamed)
		{
			if (size_Renamed_Field == 0 || size_Renamed_Field >= bytes.Length)
			{
				int capacity = Math.Max(32, size_Renamed_Field << 1);
				reserve(capacity);
			}
			bytes[size_Renamed_Field] = (sbyte)value_Renamed;
			size_Renamed_Field++;
		}

		public void reserve(int capacity)
		{
			if (bytes == null || bytes.Length < capacity)
			{
				sbyte[] destinationArray = new sbyte[capacity];
				if (bytes != null)
				{
					Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
				}
				bytes = destinationArray;
			}
		}

		public void set_Renamed(sbyte[] source, int offset, int count)
		{
			bytes = new sbyte[count];
			size_Renamed_Field = count;
			for (int i = 0; i < count; i++)
			{
				bytes[i] = source[offset + i];
			}
		}
	}
}
