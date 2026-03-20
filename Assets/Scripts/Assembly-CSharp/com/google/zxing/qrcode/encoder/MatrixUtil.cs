using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;

namespace com.google.zxing.qrcode.encoder
{
	public sealed class MatrixUtil
	{
		private const int VERSION_INFO_POLY = 7973;

		private const int TYPE_INFO_POLY = 1335;

		private const int TYPE_INFO_MASK_PATTERN = 21522;

		private static readonly int[][] POSITION_DETECTION_PATTERN = new int[7][]
		{
			new int[7] { 1, 1, 1, 1, 1, 1, 1 },
			new int[7] { 1, 0, 0, 0, 0, 0, 1 },
			new int[7] { 1, 0, 1, 1, 1, 0, 1 },
			new int[7] { 1, 0, 1, 1, 1, 0, 1 },
			new int[7] { 1, 0, 1, 1, 1, 0, 1 },
			new int[7] { 1, 0, 0, 0, 0, 0, 1 },
			new int[7] { 1, 1, 1, 1, 1, 1, 1 }
		};

		private static readonly int[][] HORIZONTAL_SEPARATION_PATTERN = new int[1][] { new int[8] };

		private static readonly int[][] VERTICAL_SEPARATION_PATTERN = new int[7][]
		{
			new int[1],
			new int[1],
			new int[1],
			new int[1],
			new int[1],
			new int[1],
			new int[1]
		};

		private static readonly int[][] POSITION_ADJUSTMENT_PATTERN = new int[5][]
		{
			new int[5] { 1, 1, 1, 1, 1 },
			new int[5] { 1, 0, 0, 0, 1 },
			new int[5] { 1, 0, 1, 0, 1 },
			new int[5] { 1, 0, 0, 0, 1 },
			new int[5] { 1, 1, 1, 1, 1 }
		};

		private static readonly int[][] POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE = new int[40][]
		{
			new int[7] { -1, -1, -1, -1, -1, -1, -1 },
			new int[7] { 6, 18, -1, -1, -1, -1, -1 },
			new int[7] { 6, 22, -1, -1, -1, -1, -1 },
			new int[7] { 6, 26, -1, -1, -1, -1, -1 },
			new int[7] { 6, 30, -1, -1, -1, -1, -1 },
			new int[7] { 6, 34, -1, -1, -1, -1, -1 },
			new int[7] { 6, 22, 38, -1, -1, -1, -1 },
			new int[7] { 6, 24, 42, -1, -1, -1, -1 },
			new int[7] { 6, 26, 46, -1, -1, -1, -1 },
			new int[7] { 6, 28, 50, -1, -1, -1, -1 },
			new int[7] { 6, 30, 54, -1, -1, -1, -1 },
			new int[7] { 6, 32, 58, -1, -1, -1, -1 },
			new int[7] { 6, 34, 62, -1, -1, -1, -1 },
			new int[7] { 6, 26, 46, 66, -1, -1, -1 },
			new int[7] { 6, 26, 48, 70, -1, -1, -1 },
			new int[7] { 6, 26, 50, 74, -1, -1, -1 },
			new int[7] { 6, 30, 54, 78, -1, -1, -1 },
			new int[7] { 6, 30, 56, 82, -1, -1, -1 },
			new int[7] { 6, 30, 58, 86, -1, -1, -1 },
			new int[7] { 6, 34, 62, 90, -1, -1, -1 },
			new int[7] { 6, 28, 50, 72, 94, -1, -1 },
			new int[7] { 6, 26, 50, 74, 98, -1, -1 },
			new int[7] { 6, 30, 54, 78, 102, -1, -1 },
			new int[7] { 6, 28, 54, 80, 106, -1, -1 },
			new int[7] { 6, 32, 58, 84, 110, -1, -1 },
			new int[7] { 6, 30, 58, 86, 114, -1, -1 },
			new int[7] { 6, 34, 62, 90, 118, -1, -1 },
			new int[7] { 6, 26, 50, 74, 98, 122, -1 },
			new int[7] { 6, 30, 54, 78, 102, 126, -1 },
			new int[7] { 6, 26, 52, 78, 104, 130, -1 },
			new int[7] { 6, 30, 56, 82, 108, 134, -1 },
			new int[7] { 6, 34, 60, 86, 112, 138, -1 },
			new int[7] { 6, 30, 58, 86, 114, 142, -1 },
			new int[7] { 6, 34, 62, 90, 118, 146, -1 },
			new int[7] { 6, 30, 54, 78, 102, 126, 150 },
			new int[7] { 6, 24, 50, 76, 102, 128, 154 },
			new int[7] { 6, 28, 54, 80, 106, 132, 158 },
			new int[7] { 6, 32, 58, 84, 110, 136, 162 },
			new int[7] { 6, 26, 54, 82, 110, 138, 166 },
			new int[7] { 6, 30, 58, 86, 114, 142, 170 }
		};

		private static readonly int[][] TYPE_INFO_COORDINATES = new int[15][]
		{
			new int[2] { 8, 0 },
			new int[2] { 8, 1 },
			new int[2] { 8, 2 },
			new int[2] { 8, 3 },
			new int[2] { 8, 4 },
			new int[2] { 8, 5 },
			new int[2] { 8, 7 },
			new int[2] { 8, 8 },
			new int[2] { 7, 8 },
			new int[2] { 5, 8 },
			new int[2] { 4, 8 },
			new int[2] { 3, 8 },
			new int[2] { 2, 8 },
			new int[2] { 1, 8 },
			new int[2] { 0, 8 }
		};

		private MatrixUtil()
		{
		}

		public static void clearMatrix(ByteMatrix matrix)
		{
			matrix.clear(-1);
		}

		public static void buildMatrix(BitVector dataBits, ErrorCorrectionLevel ecLevel, int version, int maskPattern, ByteMatrix matrix)
		{
			clearMatrix(matrix);
			embedBasicPatterns(version, matrix);
			embedTypeInfo(ecLevel, maskPattern, matrix);
			maybeEmbedVersionInfo(version, matrix);
			embedDataBits(dataBits, maskPattern, matrix);
		}

		public static void embedBasicPatterns(int version, ByteMatrix matrix)
		{
			embedPositionDetectionPatternsAndSeparators(matrix);
			embedDarkDotAtLeftBottomCorner(matrix);
			maybeEmbedPositionAdjustmentPatterns(version, matrix);
			embedTimingPatterns(matrix);
		}

		public static void embedTypeInfo(ErrorCorrectionLevel ecLevel, int maskPattern, ByteMatrix matrix)
		{
			BitVector bitVector = new BitVector();
			makeTypeInfoBits(ecLevel, maskPattern, bitVector);
			for (int i = 0; i < bitVector.size(); i++)
			{
				int value_Renamed = bitVector.at(bitVector.size() - 1 - i);
				int x = TYPE_INFO_COORDINATES[i][0];
				int y = TYPE_INFO_COORDINATES[i][1];
				matrix.set_Renamed(x, y, value_Renamed);
				if (i < 8)
				{
					int x2 = matrix.Width - i - 1;
					int y2 = 8;
					matrix.set_Renamed(x2, y2, value_Renamed);
				}
				else
				{
					int x3 = 8;
					int y3 = matrix.Height - 7 + (i - 8);
					matrix.set_Renamed(x3, y3, value_Renamed);
				}
			}
		}

		public static void maybeEmbedVersionInfo(int version, ByteMatrix matrix)
		{
			if (version < 7)
			{
				return;
			}
			BitVector bitVector = new BitVector();
			makeVersionInfoBits(version, bitVector);
			int num = 17;
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int value_Renamed = bitVector.at(num);
					num--;
					matrix.set_Renamed(i, matrix.Height - 11 + j, value_Renamed);
					matrix.set_Renamed(matrix.Height - 11 + j, i, value_Renamed);
				}
			}
		}

		public static void embedDataBits(BitVector dataBits, int maskPattern, ByteMatrix matrix)
		{
			int num = 0;
			int num2 = -1;
			int num3 = matrix.Width - 1;
			int i = matrix.Height - 1;
			while (num3 > 0)
			{
				if (num3 == 6)
				{
					num3--;
				}
				for (; i >= 0 && i < matrix.Height; i += num2)
				{
					for (int j = 0; j < 2; j++)
					{
						int x = num3 - j;
						if (isEmpty(matrix.get_Renamed(x, i)))
						{
							int num4;
							if (num < dataBits.size())
							{
								num4 = dataBits.at(num);
								num++;
							}
							else
							{
								num4 = 0;
							}
							if (maskPattern != -1 && MaskUtil.getDataMaskBit(maskPattern, x, i))
							{
								num4 ^= 1;
							}
							matrix.set_Renamed(x, i, num4);
						}
					}
				}
				num2 = -num2;
				i += num2;
				num3 -= 2;
			}
			if (num != dataBits.size())
			{
				throw new WriterException("Not all bits consumed: " + num + '/' + dataBits.size());
			}
		}

		public static int findMSBSet(int value_Renamed)
		{
			int num = 0;
			while (value_Renamed != 0)
			{
				value_Renamed = SupportClass.URShift(value_Renamed, 1);
				num++;
			}
			return num;
		}

		public static int calculateBCHCode(int value_Renamed, int poly)
		{
			int num = findMSBSet(poly);
			value_Renamed <<= num - 1;
			while (findMSBSet(value_Renamed) >= num)
			{
				value_Renamed ^= poly << findMSBSet(value_Renamed) - num;
			}
			return value_Renamed;
		}

		public static void makeTypeInfoBits(ErrorCorrectionLevel ecLevel, int maskPattern, BitVector bits)
		{
			if (!QRCode.isValidMaskPattern(maskPattern))
			{
				throw new WriterException("Invalid mask pattern");
			}
			int value_Renamed = (ecLevel.Bits << 3) | maskPattern;
			bits.appendBits(value_Renamed, 5);
			int value_Renamed2 = calculateBCHCode(value_Renamed, 1335);
			bits.appendBits(value_Renamed2, 10);
			BitVector bitVector = new BitVector();
			bitVector.appendBits(21522, 15);
			bits.xor(bitVector);
			if (bits.size() != 15)
			{
				throw new WriterException("should not happen but we got: " + bits.size());
			}
		}

		public static void makeVersionInfoBits(int version, BitVector bits)
		{
			bits.appendBits(version, 6);
			int value_Renamed = calculateBCHCode(version, 7973);
			bits.appendBits(value_Renamed, 12);
			if (bits.size() != 18)
			{
				throw new WriterException("should not happen but we got: " + bits.size());
			}
		}

		private static bool isEmpty(int value_Renamed)
		{
			return value_Renamed == -1;
		}

		private static bool isValidValue(int value_Renamed)
		{
			return value_Renamed == -1 || value_Renamed == 0 || value_Renamed == 1;
		}

		private static void embedTimingPatterns(ByteMatrix matrix)
		{
			for (int i = 8; i < matrix.Width - 8; i++)
			{
				int value_Renamed = (i + 1) % 2;
				if (!isValidValue(matrix.get_Renamed(i, 6)))
				{
					throw new WriterException();
				}
				if (isEmpty(matrix.get_Renamed(i, 6)))
				{
					matrix.set_Renamed(i, 6, value_Renamed);
				}
				if (!isValidValue(matrix.get_Renamed(6, i)))
				{
					throw new WriterException();
				}
				if (isEmpty(matrix.get_Renamed(6, i)))
				{
					matrix.set_Renamed(6, i, value_Renamed);
				}
			}
		}

		private static void embedDarkDotAtLeftBottomCorner(ByteMatrix matrix)
		{
			if (matrix.get_Renamed(8, matrix.Height - 8) == 0)
			{
				throw new WriterException();
			}
			matrix.set_Renamed(8, matrix.Height - 8, 1);
		}

		private static void embedHorizontalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			if (HORIZONTAL_SEPARATION_PATTERN[0].Length != 8 || HORIZONTAL_SEPARATION_PATTERN.Length != 1)
			{
				throw new WriterException("Bad horizontal separation pattern");
			}
			for (int i = 0; i < 8; i++)
			{
				if (!isEmpty(matrix.get_Renamed(xStart + i, yStart)))
				{
					throw new WriterException();
				}
				matrix.set_Renamed(xStart + i, yStart, HORIZONTAL_SEPARATION_PATTERN[0][i]);
			}
		}

		private static void embedVerticalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			if (VERTICAL_SEPARATION_PATTERN[0].Length != 1 || VERTICAL_SEPARATION_PATTERN.Length != 7)
			{
				throw new WriterException("Bad vertical separation pattern");
			}
			for (int i = 0; i < 7; i++)
			{
				if (!isEmpty(matrix.get_Renamed(xStart, yStart + i)))
				{
					throw new WriterException();
				}
				matrix.set_Renamed(xStart, yStart + i, VERTICAL_SEPARATION_PATTERN[i][0]);
			}
		}

		private static void embedPositionAdjustmentPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			if (POSITION_ADJUSTMENT_PATTERN[0].Length != 5 || POSITION_ADJUSTMENT_PATTERN.Length != 5)
			{
				throw new WriterException("Bad position adjustment");
			}
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					if (!isEmpty(matrix.get_Renamed(xStart + j, yStart + i)))
					{
						throw new WriterException();
					}
					matrix.set_Renamed(xStart + j, yStart + i, POSITION_ADJUSTMENT_PATTERN[i][j]);
				}
			}
		}

		private static void embedPositionDetectionPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			if (POSITION_DETECTION_PATTERN[0].Length != 7 || POSITION_DETECTION_PATTERN.Length != 7)
			{
				throw new WriterException("Bad position detection pattern");
			}
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					if (!isEmpty(matrix.get_Renamed(xStart + j, yStart + i)))
					{
						throw new WriterException();
					}
					matrix.set_Renamed(xStart + j, yStart + i, POSITION_DETECTION_PATTERN[i][j]);
				}
			}
		}

		private static void embedPositionDetectionPatternsAndSeparators(ByteMatrix matrix)
		{
			int num = POSITION_DETECTION_PATTERN[0].Length;
			embedPositionDetectionPattern(0, 0, matrix);
			embedPositionDetectionPattern(matrix.Width - num, 0, matrix);
			embedPositionDetectionPattern(0, matrix.Width - num, matrix);
			int num2 = HORIZONTAL_SEPARATION_PATTERN[0].Length;
			embedHorizontalSeparationPattern(0, num2 - 1, matrix);
			embedHorizontalSeparationPattern(matrix.Width - num2, num2 - 1, matrix);
			embedHorizontalSeparationPattern(0, matrix.Width - num2, matrix);
			int num3 = VERTICAL_SEPARATION_PATTERN.Length;
			embedVerticalSeparationPattern(num3, 0, matrix);
			embedVerticalSeparationPattern(matrix.Height - num3 - 1, 0, matrix);
			embedVerticalSeparationPattern(num3, matrix.Height - num3, matrix);
		}

		private static void maybeEmbedPositionAdjustmentPatterns(int version, ByteMatrix matrix)
		{
			if (version < 2)
			{
				return;
			}
			int num = version - 1;
			int[] array = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[num];
			int num2 = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[num].Length;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					int num3 = array[i];
					int num4 = array[j];
					if (num4 != -1 && num3 != -1 && isEmpty(matrix.get_Renamed(num4, num3)))
					{
						embedPositionAdjustmentPattern(num4 - 2, num3 - 2, matrix);
					}
				}
			}
		}
	}
}
