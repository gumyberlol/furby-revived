namespace com.google.zxing.common
{
	public sealed class HybridBinarizer : GlobalHistogramBinarizer
	{
		private const int MINIMUM_DIMENSION = 40;

		private BitMatrix matrix;

		public override BitMatrix BlackMatrix
		{
			get
			{
				binarizeEntireImage();
				return matrix;
			}
		}

		public HybridBinarizer(LuminanceSource source)
			: base(source)
		{
		}

		public override Binarizer createBinarizer(LuminanceSource source)
		{
			return new HybridBinarizer(source);
		}

		private void binarizeEntireImage()
		{
			if (matrix == null)
			{
				LuminanceSource luminanceSource = LuminanceSource;
				if (luminanceSource.Width >= 40 && luminanceSource.Height >= 40)
				{
					sbyte[] array = luminanceSource.Matrix;
					int width = luminanceSource.Width;
					int height = luminanceSource.Height;
					int subWidth = width >> 3;
					int subHeight = height >> 3;
					int[][] blackPoints = calculateBlackPoints(array, subWidth, subHeight, width);
					matrix = new BitMatrix(width, height);
					calculateThresholdForBlock(array, subWidth, subHeight, width, blackPoints, matrix);
				}
				else
				{
					matrix = base.BlackMatrix;
				}
			}
		}

		public void Reset()
		{
			matrix = null;
		}

		private static void calculateThresholdForBlock(sbyte[] luminances, int subWidth, int subHeight, int stride, int[][] blackPoints, BitMatrix matrix)
		{
			for (int i = 0; i < subHeight; i++)
			{
				for (int j = 0; j < subWidth; j++)
				{
					int num = ((j <= 1) ? 2 : j);
					num = ((num >= subWidth - 2) ? (subWidth - 3) : num);
					int num2 = ((i <= 1) ? 2 : i);
					num2 = ((num2 >= subHeight - 2) ? (subHeight - 3) : num2);
					int num3 = 0;
					for (int k = -2; k <= 2; k++)
					{
						int[] array = blackPoints[num2 + k];
						num3 += array[num - 2];
						num3 += array[num - 1];
						num3 += array[num];
						num3 += array[num + 1];
						num3 += array[num + 2];
					}
					int threshold = num3 / 25;
					threshold8x8Block(luminances, j << 3, i << 3, threshold, stride, matrix);
				}
			}
		}

		private static void threshold8x8Block(sbyte[] luminances, int xoffset, int yoffset, int threshold, int stride, BitMatrix matrix)
		{
			for (int i = 0; i < 8; i++)
			{
				int num = (yoffset + i) * stride + xoffset;
				for (int j = 0; j < 8; j++)
				{
					int num2 = luminances[num + j] & 0xFF;
					if (num2 < threshold)
					{
						matrix.set_Renamed(xoffset + j, yoffset + i);
					}
				}
			}
		}

		private static int[][] calculateBlackPoints(sbyte[] luminances, int subWidth, int subHeight, int stride)
		{
			int[][] array = new int[subHeight][];
			for (int i = 0; i < subHeight; i++)
			{
				array[i] = new int[subWidth];
			}
			for (int j = 0; j < subHeight; j++)
			{
				for (int k = 0; k < subWidth; k++)
				{
					int num = 0;
					int num2 = 255;
					int num3 = 0;
					for (int l = 0; l < 8; l++)
					{
						int num4 = ((j << 3) + l) * stride + (k << 3);
						for (int m = 0; m < 8; m++)
						{
							int num5 = luminances[num4 + m] & 0xFF;
							num += num5;
							if (num5 < num2)
							{
								num2 = num5;
							}
							if (num5 > num3)
							{
								num3 = num5;
							}
						}
					}
					int num6 = ((num3 - num2 <= 24) ? (num2 >> 1) : (num >> 6));
					array[j][k] = num6;
				}
			}
			return array;
		}
	}
}
