using System;
using System.Collections;
using com.google.zxing.common;

namespace com.google.zxing.qrcode.detector
{
	public class FinderPatternFinder
	{
		private class CenterComparator : Comparator
		{
			public virtual int compare(object center1, object center2)
			{
				return ((FinderPattern)center2).Count - ((FinderPattern)center1).Count;
			}
		}

		private const int CENTER_QUORUM = 2;

		protected internal const int MIN_SKIP = 3;

		protected internal const int MAX_MODULES = 57;

		private const int INTEGER_MATH_SHIFT = 8;

		private BitMatrix image;

		private ArrayList possibleCenters;

		private bool hasSkipped;

		private int[] crossCheckStateCount;

		private ResultPointCallback resultPointCallback;

		protected internal virtual BitMatrix Image
		{
			get
			{
				return image;
			}
		}

		protected internal virtual ArrayList PossibleCenters
		{
			get
			{
				return possibleCenters;
			}
		}

		private int[] CrossCheckStateCount
		{
			get
			{
				crossCheckStateCount[0] = 0;
				crossCheckStateCount[1] = 0;
				crossCheckStateCount[2] = 0;
				crossCheckStateCount[3] = 0;
				crossCheckStateCount[4] = 0;
				return crossCheckStateCount;
			}
		}

		public FinderPatternFinder(BitMatrix image)
			: this(image, null)
		{
		}

		public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
		{
			this.image = image;
			possibleCenters = ArrayList.Synchronized(new ArrayList(10));
			crossCheckStateCount = new int[5];
			this.resultPointCallback = resultPointCallback;
		}

		internal virtual FinderPatternInfo find(Hashtable hints)
		{
			bool flag = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
			int height = image.Height;
			int width = image.Width;
			int num = 3 * height / 228;
			if (num < 3 || flag)
			{
				num = 3;
			}
			bool flag2 = false;
			int[] array = new int[5];
			for (int i = num - 1; i < height; i += num)
			{
				if (flag2)
				{
					break;
				}
				array[0] = 0;
				array[1] = 0;
				array[2] = 0;
				array[3] = 0;
				array[4] = 0;
				int num2 = 0;
				for (int j = 0; j < width; j++)
				{
					if (image.get_Renamed(j, i))
					{
						if ((num2 & 1) == 1)
						{
							num2++;
						}
						array[num2]++;
					}
					else if ((num2 & 1) == 0)
					{
						if (num2 == 4)
						{
							if (foundPatternCross(array))
							{
								if (handlePossibleCenter(array, i, j))
								{
									num = 2;
									if (hasSkipped)
									{
										flag2 = haveMultiplyConfirmedCenters();
									}
									else
									{
										int num3 = findRowSkip();
										if (num3 > array[2])
										{
											i += num3 - array[2] - num;
											j = width - 1;
										}
									}
								}
								else
								{
									do
									{
										j++;
									}
									while (j < width && !image.get_Renamed(j, i));
									j--;
								}
								num2 = 0;
								array[0] = 0;
								array[1] = 0;
								array[2] = 0;
								array[3] = 0;
								array[4] = 0;
							}
							else
							{
								array[0] = array[2];
								array[1] = array[3];
								array[2] = array[4];
								array[3] = 1;
								array[4] = 0;
								num2 = 3;
							}
						}
						else
						{
							array[++num2]++;
						}
					}
					else
					{
						array[num2]++;
					}
				}
				if (foundPatternCross(array) && handlePossibleCenter(array, i, width))
				{
					num = array[0];
					if (hasSkipped)
					{
						flag2 = haveMultiplyConfirmedCenters();
					}
				}
			}
			FinderPattern[] array2 = selectBestPatterns();
			if (array2 == null)
			{
				return null;
			}
			ResultPoint.orderBestPatterns(array2);
			return new FinderPatternInfo(array2);
		}

		private static float centerFromEnd(int[] stateCount, int end)
		{
			return (float)(end - stateCount[4] - stateCount[3]) - (float)stateCount[2] / 2f;
		}

		protected internal static bool foundPatternCross(int[] stateCount)
		{
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				int num2 = stateCount[i];
				if (num2 == 0)
				{
					return false;
				}
				num += num2;
			}
			if (num < 7)
			{
				return false;
			}
			int num3 = (num << 8) / 7;
			int num4 = num3 / 2;
			return Math.Abs(num3 - (stateCount[0] << 8)) < num4 && Math.Abs(num3 - (stateCount[1] << 8)) < num4 && Math.Abs(3 * num3 - (stateCount[2] << 8)) < 3 * num4 && Math.Abs(num3 - (stateCount[3] << 8)) < num4 && Math.Abs(num3 - (stateCount[4] << 8)) < num4;
		}

		private float crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
		{
			BitMatrix bitMatrix = image;
			int height = bitMatrix.Height;
			int[] array = CrossCheckStateCount;
			int num = startI;
			while (num >= 0 && bitMatrix.get_Renamed(centerJ, num))
			{
				array[2]++;
				num--;
			}
			if (num < 0)
			{
				return float.NaN;
			}
			while (num >= 0 && !bitMatrix.get_Renamed(centerJ, num) && array[1] <= maxCount)
			{
				array[1]++;
				num--;
			}
			if (num < 0 || array[1] > maxCount)
			{
				return float.NaN;
			}
			while (num >= 0 && bitMatrix.get_Renamed(centerJ, num) && array[0] <= maxCount)
			{
				array[0]++;
				num--;
			}
			if (array[0] > maxCount)
			{
				return float.NaN;
			}
			for (num = startI + 1; num < height && bitMatrix.get_Renamed(centerJ, num); num++)
			{
				array[2]++;
			}
			if (num == height)
			{
				return float.NaN;
			}
			for (; num < height && !bitMatrix.get_Renamed(centerJ, num); num++)
			{
				if (array[3] >= maxCount)
				{
					break;
				}
				array[3]++;
			}
			if (num == height || array[3] >= maxCount)
			{
				return float.NaN;
			}
			for (; num < height && bitMatrix.get_Renamed(centerJ, num); num++)
			{
				if (array[4] >= maxCount)
				{
					break;
				}
				array[4]++;
			}
			if (array[4] >= maxCount)
			{
				return float.NaN;
			}
			int num2 = array[0] + array[1] + array[2] + array[3] + array[4];
			if (5 * Math.Abs(num2 - originalStateCountTotal) >= 2 * originalStateCountTotal)
			{
				return float.NaN;
			}
			return (!foundPatternCross(array)) ? float.NaN : centerFromEnd(array, num);
		}

		private float crossCheckHorizontal(int startJ, int centerI, int maxCount, int originalStateCountTotal)
		{
			BitMatrix bitMatrix = image;
			int width = bitMatrix.Width;
			int[] array = CrossCheckStateCount;
			int num = startJ;
			while (num >= 0 && bitMatrix.get_Renamed(num, centerI))
			{
				array[2]++;
				num--;
			}
			if (num < 0)
			{
				return float.NaN;
			}
			while (num >= 0 && !bitMatrix.get_Renamed(num, centerI) && array[1] <= maxCount)
			{
				array[1]++;
				num--;
			}
			if (num < 0 || array[1] > maxCount)
			{
				return float.NaN;
			}
			while (num >= 0 && bitMatrix.get_Renamed(num, centerI) && array[0] <= maxCount)
			{
				array[0]++;
				num--;
			}
			if (array[0] > maxCount)
			{
				return float.NaN;
			}
			for (num = startJ + 1; num < width && bitMatrix.get_Renamed(num, centerI); num++)
			{
				array[2]++;
			}
			if (num == width)
			{
				return float.NaN;
			}
			for (; num < width && !bitMatrix.get_Renamed(num, centerI); num++)
			{
				if (array[3] >= maxCount)
				{
					break;
				}
				array[3]++;
			}
			if (num == width || array[3] >= maxCount)
			{
				return float.NaN;
			}
			for (; num < width && bitMatrix.get_Renamed(num, centerI); num++)
			{
				if (array[4] >= maxCount)
				{
					break;
				}
				array[4]++;
			}
			if (array[4] >= maxCount)
			{
				return float.NaN;
			}
			int num2 = array[0] + array[1] + array[2] + array[3] + array[4];
			if (5 * Math.Abs(num2 - originalStateCountTotal) >= originalStateCountTotal)
			{
				return float.NaN;
			}
			return (!foundPatternCross(array)) ? float.NaN : centerFromEnd(array, num);
		}

		protected internal virtual bool handlePossibleCenter(int[] stateCount, int i, int j)
		{
			int num = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
			float num2 = centerFromEnd(stateCount, j);
			float num3 = crossCheckVertical(i, (int)num2, stateCount[2], num);
			if (!float.IsNaN(num3))
			{
				num2 = crossCheckHorizontal((int)num2, (int)num3, stateCount[2], num);
				if (!float.IsNaN(num2))
				{
					float num4 = (float)num / 7f;
					bool flag = false;
					int count = possibleCenters.Count;
					for (int k = 0; k < count; k++)
					{
						FinderPattern finderPattern = (FinderPattern)possibleCenters[k];
						if (finderPattern.aboutEquals(num4, num3, num2))
						{
							finderPattern.incrementCount();
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						ResultPoint resultPoint = new FinderPattern(num2, num3, num4);
						possibleCenters.Add(resultPoint);
						if (resultPointCallback != null)
						{
							resultPointCallback.foundPossibleResultPoint(resultPoint);
						}
					}
					return true;
				}
			}
			return false;
		}

		private int findRowSkip()
		{
			int count = possibleCenters.Count;
			if (count <= 1)
			{
				return 0;
			}
			FinderPattern finderPattern = null;
			for (int i = 0; i < count; i++)
			{
				FinderPattern finderPattern2 = (FinderPattern)possibleCenters[i];
				if (finderPattern2.Count >= 2)
				{
					if (finderPattern != null)
					{
						hasSkipped = true;
						return (int)(Math.Abs(finderPattern.X - finderPattern2.X) - Math.Abs(finderPattern.Y - finderPattern2.Y)) / 2;
					}
					finderPattern = finderPattern2;
				}
			}
			return 0;
		}

		private bool haveMultiplyConfirmedCenters()
		{
			int num = 0;
			float num2 = 0f;
			int count = possibleCenters.Count;
			for (int i = 0; i < count; i++)
			{
				FinderPattern finderPattern = (FinderPattern)possibleCenters[i];
				if (finderPattern.Count >= 2)
				{
					num++;
					num2 += finderPattern.EstimatedModuleSize;
				}
			}
			if (num < 3)
			{
				return false;
			}
			float num3 = num2 / (float)count;
			float num4 = 0f;
			for (int j = 0; j < count; j++)
			{
				FinderPattern finderPattern2 = (FinderPattern)possibleCenters[j];
				num4 += Math.Abs(finderPattern2.EstimatedModuleSize - num3);
			}
			return num4 <= 0.05f * num2;
		}

		private FinderPattern[] selectBestPatterns()
		{
			int count = possibleCenters.Count;
			if (count < 3)
			{
				return null;
			}
			if (count > 3)
			{
				float num = 0f;
				for (int i = 0; i < count; i++)
				{
					num += ((FinderPattern)possibleCenters[i]).EstimatedModuleSize;
				}
				float num2 = num / (float)count;
				for (int j = 0; j < possibleCenters.Count; j++)
				{
					if (possibleCenters.Count <= 3)
					{
						break;
					}
					FinderPattern finderPattern = (FinderPattern)possibleCenters[j];
					if (Math.Abs(finderPattern.EstimatedModuleSize - num2) > 0.2f * num2)
					{
						possibleCenters.RemoveAt(j);
						j--;
					}
				}
			}
			if (possibleCenters.Count > 3)
			{
				Collections.insertionSort(possibleCenters, new CenterComparator());
				SupportClass.SetCapacity(possibleCenters, 3);
			}
			return new FinderPattern[3]
			{
				(FinderPattern)possibleCenters[0],
				(FinderPattern)possibleCenters[1],
				(FinderPattern)possibleCenters[2]
			};
		}
	}
}
