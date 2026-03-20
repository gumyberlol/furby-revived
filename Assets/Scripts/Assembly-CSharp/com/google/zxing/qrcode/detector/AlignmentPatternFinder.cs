using System;
using System.Collections;
using com.google.zxing.common;

namespace com.google.zxing.qrcode.detector
{
	internal sealed class AlignmentPatternFinder
	{
		private BitMatrix image;

		private ArrayList possibleCenters;

		private int startX;

		private int startY;

		private int width;

		private int height;

		private float moduleSize;

		private int[] crossCheckStateCount;

		private ResultPointCallback resultPointCallback;

		internal AlignmentPatternFinder(BitMatrix image, int startX, int startY, int width, int height, float moduleSize, ResultPointCallback resultPointCallback)
		{
			this.image = image;
			possibleCenters = ArrayList.Synchronized(new ArrayList(5));
			this.startX = startX;
			this.startY = startY;
			this.width = width;
			this.height = height;
			this.moduleSize = moduleSize;
			crossCheckStateCount = new int[3];
			this.resultPointCallback = resultPointCallback;
		}

		internal AlignmentPattern find()
		{
			int num = startX;
			int num2 = height;
			int num3 = num + width;
			int num4 = startY + (num2 >> 1);
			int[] array = new int[3];
			for (int i = 0; i < num2; i++)
			{
				int num5 = num4 + (((i & 1) != 0) ? (-(i + 1 >> 1)) : (i + 1 >> 1));
				array[0] = 0;
				array[1] = 0;
				array[2] = 0;
				int j;
				for (j = num; j < num3 && !image.get_Renamed(j, num5); j++)
				{
				}
				int num6 = 0;
				for (; j < num3; j++)
				{
					if (image.get_Renamed(j, num5))
					{
						switch (num6)
						{
						case 1:
							array[num6]++;
							break;
						case 2:
							if (foundPatternCross(array))
							{
								AlignmentPattern alignmentPattern = handlePossibleCenter(array, num5, j);
								if (alignmentPattern != null)
								{
									return alignmentPattern;
								}
							}
							array[0] = array[2];
							array[1] = 1;
							array[2] = 0;
							num6 = 1;
							break;
						default:
							array[++num6]++;
							break;
						}
					}
					else
					{
						if (num6 == 1)
						{
							num6++;
						}
						array[num6]++;
					}
				}
				if (foundPatternCross(array))
				{
					AlignmentPattern alignmentPattern2 = handlePossibleCenter(array, num5, num3);
					if (alignmentPattern2 != null)
					{
						return alignmentPattern2;
					}
				}
			}
			if (possibleCenters.Count != 0)
			{
				return (AlignmentPattern)possibleCenters[0];
			}
			throw ReaderException.Instance;
		}

		private static float centerFromEnd(int[] stateCount, int end)
		{
			return (float)(end - stateCount[2]) - (float)stateCount[1] / 2f;
		}

		private bool foundPatternCross(int[] stateCount)
		{
			float num = moduleSize;
			float num2 = num / 2f;
			for (int i = 0; i < 3; i++)
			{
				if (Math.Abs(num - (float)stateCount[i]) >= num2)
				{
					return false;
				}
			}
			return true;
		}

		private float crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
		{
			BitMatrix bitMatrix = image;
			int num = bitMatrix.Height;
			int[] array = crossCheckStateCount;
			array[0] = 0;
			array[1] = 0;
			array[2] = 0;
			int num2 = startI;
			while (num2 >= 0 && bitMatrix.get_Renamed(centerJ, num2) && array[1] <= maxCount)
			{
				array[1]++;
				num2--;
			}
			if (num2 < 0 || array[1] > maxCount)
			{
				return float.NaN;
			}
			while (num2 >= 0 && !bitMatrix.get_Renamed(centerJ, num2) && array[0] <= maxCount)
			{
				array[0]++;
				num2--;
			}
			if (array[0] > maxCount)
			{
				return float.NaN;
			}
			for (num2 = startI + 1; num2 < num && bitMatrix.get_Renamed(centerJ, num2); num2++)
			{
				if (array[1] > maxCount)
				{
					break;
				}
				array[1]++;
			}
			if (num2 == num || array[1] > maxCount)
			{
				return float.NaN;
			}
			for (; num2 < num && !bitMatrix.get_Renamed(centerJ, num2); num2++)
			{
				if (array[2] > maxCount)
				{
					break;
				}
				array[2]++;
			}
			if (array[2] > maxCount)
			{
				return float.NaN;
			}
			int num3 = array[0] + array[1] + array[2];
			if (5 * Math.Abs(num3 - originalStateCountTotal) >= 2 * originalStateCountTotal)
			{
				return float.NaN;
			}
			return (!foundPatternCross(array)) ? float.NaN : centerFromEnd(array, num2);
		}

		private AlignmentPattern handlePossibleCenter(int[] stateCount, int i, int j)
		{
			int originalStateCountTotal = stateCount[0] + stateCount[1] + stateCount[2];
			float num = centerFromEnd(stateCount, j);
			float num2 = crossCheckVertical(i, (int)num, 2 * stateCount[1], originalStateCountTotal);
			if (!float.IsNaN(num2))
			{
				float estimatedModuleSize = (float)(stateCount[0] + stateCount[1] + stateCount[2]) / 3f;
				int count = possibleCenters.Count;
				for (int k = 0; k < count; k++)
				{
					AlignmentPattern alignmentPattern = (AlignmentPattern)possibleCenters[k];
					if (alignmentPattern.aboutEquals(estimatedModuleSize, num2, num))
					{
						return new AlignmentPattern(num, num2, estimatedModuleSize);
					}
				}
				ResultPoint resultPoint = new AlignmentPattern(num, num2, estimatedModuleSize);
				possibleCenters.Add(resultPoint);
				if (resultPointCallback != null)
				{
					resultPointCallback.foundPossibleResultPoint(resultPoint);
				}
			}
			return null;
		}
	}
}
