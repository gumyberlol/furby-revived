using System;
using System.Text;

namespace com.google.zxing.common
{
	public sealed class BitArray
	{
		public int[] bits;

		public int size;

		public int Size
		{
			get
			{
				return size;
			}
		}

		public BitArray(int size)
		{
			if (size < 1)
			{
				throw new ArgumentException("size must be at least 1");
			}
			this.size = size;
			bits = makeArray(size);
		}

		public bool get_Renamed(int i)
		{
			return (bits[i >> 5] & (1 << (i & 0x1F))) != 0;
		}

		public void set_Renamed(int i)
		{
			bits[i >> 5] |= 1 << (i & 0x1F);
		}

		public void flip(int i)
		{
			bits[i >> 5] ^= 1 << (i & 0x1F);
		}

		public void setBulk(int i, int newBits)
		{
			bits[i >> 5] = newBits;
		}

		public void clear()
		{
			int num = bits.Length;
			for (int i = 0; i < num; i++)
			{
				bits[i] = 0;
			}
		}

		public bool isRange(int start, int end, bool value_Renamed)
		{
			if (end < start)
			{
				throw new ArgumentException();
			}
			if (end == start)
			{
				return true;
			}
			end--;
			int num = start >> 5;
			int num2 = end >> 5;
			for (int i = num; i <= num2; i++)
			{
				int num3 = ((i <= num) ? (start & 0x1F) : 0);
				int num4 = ((i >= num2) ? (end & 0x1F) : 31);
				int num5;
				if (num3 == 0 && num4 == 31)
				{
					num5 = -1;
				}
				else
				{
					num5 = 0;
					for (int j = num3; j <= num4; j++)
					{
						num5 |= 1 << j;
					}
				}
				if ((bits[i] & num5) != (value_Renamed ? num5 : 0))
				{
					return false;
				}
			}
			return true;
		}

		public int[] getBitArray()
		{
			return bits;
		}

		public void reverse()
		{
			int[] array = new int[bits.Length];
			int num = size;
			for (int i = 0; i < num; i++)
			{
				if (get_Renamed(num - i - 1))
				{
					array[i >> 5] |= 1 << (i & 0x1F);
				}
			}
			bits = array;
		}

		private static int[] makeArray(int size)
		{
			int num = size >> 5;
			if ((size & 0x1F) != 0)
			{
				num++;
			}
			return new int[num];
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(size);
			for (int i = 0; i < size; i++)
			{
				if ((i & 7) == 0)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append((!get_Renamed(i)) ? '.' : 'X');
			}
			return stringBuilder.ToString();
		}
	}
}
