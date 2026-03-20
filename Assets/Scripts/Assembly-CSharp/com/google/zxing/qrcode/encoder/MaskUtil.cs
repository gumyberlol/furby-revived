using System;
using com.google.zxing.common;

namespace com.google.zxing.qrcode.encoder
{
	public sealed class MaskUtil
	{
		private MaskUtil()
		{
		}

		public static int applyMaskPenaltyRule1(ByteMatrix matrix)
		{
			return applyMaskPenaltyRule1Internal(matrix, true) + applyMaskPenaltyRule1Internal(matrix, false);
		}

		public static int applyMaskPenaltyRule2(ByteMatrix matrix)
		{
			int num = 0;
			sbyte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height - 1; i++)
			{
				for (int j = 0; j < width - 1; j++)
				{
					int num2 = array[i][j];
					if (num2 == array[i][j + 1] && num2 == array[i + 1][j] && num2 == array[i + 1][j + 1])
					{
						num += 3;
					}
				}
			}
			return num;
		}

		public static int applyMaskPenaltyRule3(ByteMatrix matrix)
		{
			int num = 0;
			sbyte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (j + 6 < width && array[i][j] == 1 && array[i][j + 1] == 0 && array[i][j + 2] == 1 && array[i][j + 3] == 1 && array[i][j + 4] == 1 && array[i][j + 5] == 0 && array[i][j + 6] == 1 && ((j + 10 < width && array[i][j + 7] == 0 && array[i][j + 8] == 0 && array[i][j + 9] == 0 && array[i][j + 10] == 0) || (j - 4 >= 0 && array[i][j - 1] == 0 && array[i][j - 2] == 0 && array[i][j - 3] == 0 && array[i][j - 4] == 0)))
					{
						num += 40;
					}
					if (i + 6 < height && array[i][j] == 1 && array[i + 1][j] == 0 && array[i + 2][j] == 1 && array[i + 3][j] == 1 && array[i + 4][j] == 1 && array[i + 5][j] == 0 && array[i + 6][j] == 1 && ((i + 10 < height && array[i + 7][j] == 0 && array[i + 8][j] == 0 && array[i + 9][j] == 0 && array[i + 10][j] == 0) || (i - 4 >= 0 && array[i - 1][j] == 0 && array[i - 2][j] == 0 && array[i - 3][j] == 0 && array[i - 4][j] == 0)))
					{
						num += 40;
					}
				}
			}
			return num;
		}

		public static int applyMaskPenaltyRule4(ByteMatrix matrix)
		{
			int num = 0;
			sbyte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (array[i][j] == 1)
					{
						num++;
					}
				}
			}
			int num2 = matrix.Height * matrix.Width;
			double num3 = (double)num / (double)num2;
			return Math.Abs((int)(num3 * 100.0 - 50.0)) / 5 * 10;
		}

		public static bool getDataMaskBit(int maskPattern, int x, int y)
		{
			if (!QRCode.isValidMaskPattern(maskPattern))
			{
				throw new ArgumentException("Invalid mask pattern");
			}
			int num2;
			switch (maskPattern)
			{
			case 0:
				num2 = (y + x) & 1;
				break;
			case 1:
				num2 = y & 1;
				break;
			case 2:
				num2 = x % 3;
				break;
			case 3:
				num2 = (y + x) % 3;
				break;
			case 4:
				num2 = (SupportClass.URShift(y, 1) + x / 3) & 1;
				break;
			case 5:
			{
				int num = y * x;
				num2 = (num & 1) + num % 3;
				break;
			}
			case 6:
			{
				int num = y * x;
				num2 = ((num & 1) + num % 3) & 1;
				break;
			}
			case 7:
			{
				int num = y * x;
				num2 = (num % 3 + ((y + x) & 1)) & 1;
				break;
			}
			default:
				throw new ArgumentException("Invalid mask pattern: " + maskPattern);
			}
			return num2 == 0;
		}

		private static int applyMaskPenaltyRule1Internal(ByteMatrix matrix, bool isHorizontal)
		{
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			int num4 = ((!isHorizontal) ? matrix.Width : matrix.Height);
			int num5 = ((!isHorizontal) ? matrix.Height : matrix.Width);
			sbyte[][] array = matrix.Array;
			for (int i = 0; i < num4; i++)
			{
				for (int j = 0; j < num5; j++)
				{
					int num6 = ((!isHorizontal) ? array[j][i] : array[i][j]);
					if (num6 == num3)
					{
						num2++;
						if (num2 == 5)
						{
							num += 3;
						}
						else if (num2 > 5)
						{
							num++;
						}
					}
					else
					{
						num2 = 1;
						num3 = num6;
					}
				}
				num2 = 0;
			}
			return num;
		}
	}
}
