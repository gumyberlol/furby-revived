namespace com.google.zxing.common
{
	public class GlobalHistogramBinarizer : Binarizer
	{
		private const int LUMINANCE_BITS = 5;

		private static readonly int LUMINANCE_SHIFT = 3;

		private static readonly int LUMINANCE_BUCKETS = 32;

		private sbyte[] luminances;

		private int[] buckets;

		public override BitMatrix BlackMatrix
		{
			get
			{
				LuminanceSource luminanceSource = LuminanceSource;
				int width = luminanceSource.Width;
				int height = luminanceSource.Height;
				BitMatrix bitMatrix = new BitMatrix(width, height);
				initArrays(width);
				int[] array = buckets;
				sbyte[] row;
				for (int i = 1; i < 5; i++)
				{
					int y = height * i / 5;
					row = luminanceSource.getRow(y, luminances);
					int num = (width << 2) / 5;
					for (int j = width / 5; j < num; j++)
					{
						int num2 = row[j] & 0xFF;
						array[num2 >> LUMINANCE_SHIFT]++;
					}
				}
				int num3 = estimateBlackPoint(array);
				row = luminanceSource.Matrix;
				for (int k = 0; k < height; k++)
				{
					int num4 = k * width;
					for (int l = 0; l < width; l++)
					{
						int num5 = row[num4 + l] & 0xFF;
						if (num5 < num3)
						{
							bitMatrix.set_Renamed(l, k);
						}
					}
				}
				return bitMatrix;
			}
		}

		public GlobalHistogramBinarizer(LuminanceSource source)
			: base(source)
		{
		}

		public override BitArray getBlackRow(int y, BitArray row)
		{
			LuminanceSource luminanceSource = LuminanceSource;
			int width = luminanceSource.Width;
			if (row == null || row.Size < width)
			{
				row = new BitArray(width);
			}
			else
			{
				row.clear();
			}
			initArrays(width);
			sbyte[] row2 = luminanceSource.getRow(y, luminances);
			int[] array = buckets;
			for (int i = 0; i < width; i++)
			{
				int num = row2[i] & 0xFF;
				array[num >> LUMINANCE_SHIFT]++;
			}
			int num2 = estimateBlackPoint(array);
			int num3 = row2[0] & 0xFF;
			int num4 = row2[1] & 0xFF;
			for (int j = 1; j < width - 1; j++)
			{
				int num5 = row2[j + 1] & 0xFF;
				int num6 = (num4 << 2) - num3 - num5 >> 1;
				if (num6 < num2)
				{
					row.set_Renamed(j);
				}
				num3 = num4;
				num4 = num5;
			}
			return row;
		}

		public override Binarizer createBinarizer(LuminanceSource source)
		{
			return new GlobalHistogramBinarizer(source);
		}

		private void initArrays(int luminanceSize)
		{
			if (luminances == null || luminances.Length < luminanceSize)
			{
				luminances = new sbyte[luminanceSize];
			}
			if (buckets == null)
			{
				buckets = new int[LUMINANCE_BUCKETS];
				return;
			}
			for (int i = 0; i < LUMINANCE_BUCKETS; i++)
			{
				buckets[i] = 0;
			}
		}

		private static int estimateBlackPoint(int[] buckets)
		{
			int num = buckets.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				if (buckets[i] > num4)
				{
					num3 = i;
					num4 = buckets[i];
				}
				if (buckets[i] > num2)
				{
					num2 = buckets[i];
				}
			}
			int num5 = 0;
			int num6 = 0;
			for (int j = 0; j < num; j++)
			{
				int num7 = j - num3;
				int num8 = buckets[j] * num7 * num7;
				if (num8 > num6)
				{
					num5 = j;
					num6 = num8;
				}
			}
			if (num3 > num5)
			{
				int num9 = num3;
				num3 = num5;
				num5 = num9;
			}
			if (num5 - num3 <= num >> 4)
			{
				throw ReaderException.Instance;
			}
			int num10 = num5 - 1;
			int num11 = -1;
			for (int num12 = num5 - 1; num12 > num3; num12--)
			{
				int num13 = num12 - num3;
				int num14 = num13 * num13 * (num5 - num12) * (num2 - buckets[num12]);
				if (num14 > num11)
				{
					num10 = num12;
					num11 = num14;
				}
			}
			return num10 << LUMINANCE_SHIFT;
		}
	}
}
