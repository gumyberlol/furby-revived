using System;

namespace com.google.zxing.common
{
	public sealed class BitSource
	{
		private sbyte[] bytes;

		private int byteOffset;

		private int bitOffset;

		public BitSource(sbyte[] bytes)
		{
			this.bytes = bytes;
		}

		public int readBits(int numBits)
		{
			if (numBits < 1 || numBits > 32)
			{
				throw new ArgumentException();
			}
			int num = 0;
			if (bitOffset > 0)
			{
				int num2 = 8 - bitOffset;
				int num3 = ((numBits >= num2) ? num2 : numBits);
				int num4 = num2 - num3;
				int num5 = 255 >> 8 - num3 << num4;
				num = (bytes[byteOffset] & num5) >> num4;
				numBits -= num3;
				bitOffset += num3;
				if (bitOffset == 8)
				{
					bitOffset = 0;
					byteOffset++;
				}
			}
			if (numBits > 0)
			{
				while (numBits >= 8)
				{
					num = (num << 8) | (bytes[byteOffset] & 0xFF);
					byteOffset++;
					numBits -= 8;
				}
				if (numBits > 0)
				{
					int num6 = 8 - numBits;
					int num7 = 255 >> num6 << num6;
					num = (num << numBits) | ((bytes[byteOffset] & num7) >> num6);
					bitOffset += numBits;
				}
			}
			return num;
		}

		public int available()
		{
			return 8 * (bytes.Length - byteOffset) - bitOffset;
		}
	}
}
