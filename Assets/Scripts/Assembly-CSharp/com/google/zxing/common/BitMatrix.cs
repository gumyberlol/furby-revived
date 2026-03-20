using System;
using System.Text;

namespace com.google.zxing.common
{
	public sealed class BitMatrix
	{
		public int width;

		public int height;

		public int rowSize;

		public int[] bits;

		public int Width
		{
			get
			{
				return width;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
		}

		public int Dimension
		{
			get
			{
				if (width != height)
				{
					throw new SystemException("Can't call getDimension() on a non-square matrix");
				}
				return width;
			}
		}

		public BitMatrix(int dimension)
			: this(dimension, dimension)
		{
		}

		public BitMatrix(int width, int height)
		{
			if (width < 1 || height < 1)
			{
				throw new ArgumentException("Both dimensions must be greater than 0");
			}
			this.width = width;
			this.height = height;
			int num = width >> 5;
			if ((width & 0x1F) != 0)
			{
				num++;
			}
			rowSize = num;
			bits = new int[num * height];
		}

		public bool get_Renamed(int x, int y)
		{
			int num = y * rowSize + (x >> 5);
			return (SupportClass.URShift(bits[num], x & 0x1F) & 1) != 0;
		}

		public void set_Renamed(int x, int y)
		{
			int num = y * rowSize + (x >> 5);
			bits[num] |= 1 << (x & 0x1F);
		}

		public void flip(int x, int y)
		{
			int num = y * rowSize + (x >> 5);
			bits[num] ^= 1 << (x & 0x1F);
		}

		public void clear()
		{
			int num = bits.Length;
			for (int i = 0; i < num; i++)
			{
				bits[i] = 0;
			}
		}

		public void setRegion(int left, int top, int width, int height)
		{
			if (top < 0 || left < 0)
			{
				throw new ArgumentException("Left and top must be nonnegative");
			}
			if (height < 1 || width < 1)
			{
				throw new ArgumentException("Height and width must be at least 1");
			}
			int num = left + width;
			int num2 = top + height;
			if (num2 > this.height || num > this.width)
			{
				throw new ArgumentException("The region must fit inside the matrix");
			}
			for (int i = top; i < num2; i++)
			{
				int num3 = i * rowSize;
				for (int j = left; j < num; j++)
				{
					bits[num3 + (j >> 5)] |= 1 << j;
				}
			}
		}

		public BitArray getRow(int y, BitArray row)
		{
			if (row == null || row.Size < width)
			{
				row = new BitArray(width);
			}
			int num = y * rowSize;
			for (int i = 0; i < rowSize; i++)
			{
				row.setBulk(i << 5, bits[num + i]);
			}
			return row;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(height * (width + 1));
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					stringBuilder.Append((!get_Renamed(j, i)) ? "  " : "X ");
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}
	}
}
