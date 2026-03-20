using System;
using System.Text;

namespace com.google.zxing.qrcode.encoder
{
	public sealed class BitVector
	{
		private const int DEFAULT_SIZE_IN_BYTES = 32;

		private int sizeInBits;

		private sbyte[] array;

		public sbyte[] Array
		{
			get
			{
				return array;
			}
		}

		public BitVector()
		{
			sizeInBits = 0;
			array = new sbyte[32];
		}

		public int at(int index)
		{
			if (index < 0 || index >= sizeInBits)
			{
				throw new ArgumentException("Bad index: " + index);
			}
			int num = array[index >> 3] & 0xFF;
			return (num >> 7 - (index & 7)) & 1;
		}

		public int size()
		{
			return sizeInBits;
		}

		public int sizeInBytes()
		{
			return sizeInBits + 7 >> 3;
		}

		public void appendBit(int bit)
		{
			if (bit != 0 && bit != 1)
			{
				throw new ArgumentException("Bad bit");
			}
			int num = sizeInBits & 7;
			if (num == 0)
			{
				appendByte(0);
				sizeInBits -= 8;
			}
			array[sizeInBits >> 3] = (sbyte)((byte)array[sizeInBits >> 3] | (byte)(bit << 7 - num));
			sizeInBits++;
		}

		public void appendBits(int value_Renamed, int numBits)
		{
			if (numBits < 0 || numBits > 32)
			{
				throw new ArgumentException("Num bits must be between 0 and 32");
			}
			int num = numBits;
			while (num > 0)
			{
				if ((sizeInBits & 7) == 0 && num >= 8)
				{
					int value_Renamed2 = (value_Renamed >> num - 8) & 0xFF;
					appendByte(value_Renamed2);
					num -= 8;
				}
				else
				{
					int bit = (value_Renamed >> num - 1) & 1;
					appendBit(bit);
					num--;
				}
			}
		}

		public void appendBitVector(BitVector bits)
		{
			int num = bits.size();
			for (int i = 0; i < num; i++)
			{
				appendBit(bits.at(i));
			}
		}

		public void xor(BitVector other)
		{
			if (sizeInBits != other.size())
			{
				throw new ArgumentException("BitVector sizes don't match");
			}
			int num = sizeInBits + 7 >> 3;
			for (int i = 0; i < num; i++)
			{
				sbyte[] obj = array;
				int num2 = i;
				obj[num2] ^= other.array[i];
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(sizeInBits);
			for (int i = 0; i < sizeInBits; i++)
			{
				if (at(i) == 0)
				{
					stringBuilder.Append('0');
					continue;
				}
				if (at(i) == 1)
				{
					stringBuilder.Append('1');
					continue;
				}
				throw new ArgumentException("Byte isn't 0 or 1");
			}
			return stringBuilder.ToString();
		}

		private void appendByte(int value_Renamed)
		{
			if (sizeInBits >> 3 == array.Length)
			{
				sbyte[] destinationArray = new sbyte[array.Length << 1];
				System.Array.Copy(array, 0, destinationArray, 0, array.Length);
				array = destinationArray;
			}
			array[sizeInBits >> 3] = (sbyte)value_Renamed;
			sizeInBits += 8;
		}
	}
}
