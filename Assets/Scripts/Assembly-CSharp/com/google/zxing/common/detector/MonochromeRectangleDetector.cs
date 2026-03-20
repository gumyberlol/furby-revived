using System;

namespace com.google.zxing.common.detector
{
	public sealed class MonochromeRectangleDetector
	{
		private const int MAX_MODULES = 32;

		private BitMatrix image;

		public MonochromeRectangleDetector(BitMatrix image)
		{
			this.image = image;
		}

		public ResultPoint[] detect()
		{
			int height = image.Height;
			int width = image.Width;
			int num = height >> 1;
			int num2 = width >> 1;
			int num3 = Math.Max(1, height / 256);
			int num4 = Math.Max(1, width / 256);
			int top = 0;
			int bottom = height;
			int left = 0;
			int right = width;
			ResultPoint resultPoint = findCornerFromCenter(num2, 0, left, right, num, -num3, top, bottom, num2 >> 1);
			top = (int)resultPoint.Y - 1;
			ResultPoint resultPoint2 = findCornerFromCenter(num2, -num4, left, right, num, 0, top, bottom, num >> 1);
			left = (int)resultPoint2.X - 1;
			ResultPoint resultPoint3 = findCornerFromCenter(num2, num4, left, right, num, 0, top, bottom, num >> 1);
			right = (int)resultPoint3.X + 1;
			ResultPoint resultPoint4 = findCornerFromCenter(num2, 0, left, right, num, num3, top, bottom, num2 >> 1);
			bottom = (int)resultPoint4.Y + 1;
			resultPoint = findCornerFromCenter(num2, 0, left, right, num, -num3, top, bottom, num2 >> 2);
			return new ResultPoint[4] { resultPoint, resultPoint2, resultPoint3, resultPoint4 };
		}

		private ResultPoint findCornerFromCenter(int centerX, int deltaX, int left, int right, int centerY, int deltaY, int top, int bottom, int maxWhiteRun)
		{
			int[] array = null;
			int num = centerY;
			int num2 = centerX;
			while (num < bottom && num >= top && num2 < right && num2 >= left)
			{
				int[] array2 = ((deltaX != 0) ? blackWhiteRange(num2, maxWhiteRun, top, bottom, false) : blackWhiteRange(num, maxWhiteRun, left, right, true));
				if (array2 == null)
				{
					if (array == null)
					{
						throw ReaderException.Instance;
					}
					if (deltaX == 0)
					{
						int num3 = num - deltaY;
						if (array[0] < centerX)
						{
							if (array[1] > centerX)
							{
								return new ResultPoint((deltaY <= 0) ? array[1] : array[0], num3);
							}
							return new ResultPoint(array[0], num3);
						}
						return new ResultPoint(array[1], num3);
					}
					int num4 = num2 - deltaX;
					if (array[0] < centerY)
					{
						if (array[1] > centerY)
						{
							return new ResultPoint(num4, (deltaX >= 0) ? array[1] : array[0]);
						}
						return new ResultPoint(num4, array[0]);
					}
					return new ResultPoint(num4, array[1]);
				}
				array = array2;
				num += deltaY;
				num2 += deltaX;
			}
			throw ReaderException.Instance;
		}

		private int[] blackWhiteRange(int fixedDimension, int maxWhiteRun, int minDim, int maxDim, bool horizontal)
		{
			int num = minDim + maxDim >> 1;
			int num2 = num;
			while (num2 >= minDim)
			{
				if ((!horizontal) ? image.get_Renamed(fixedDimension, num2) : image.get_Renamed(num2, fixedDimension))
				{
					num2--;
					continue;
				}
				int num3 = num2;
				do
				{
					num2--;
				}
				while (num2 >= minDim && !((!horizontal) ? image.get_Renamed(fixedDimension, num2) : image.get_Renamed(num2, fixedDimension)));
				int num4 = num3 - num2;
				if (num2 >= minDim && num4 <= maxWhiteRun)
				{
					continue;
				}
				num2 = num3;
				break;
			}
			num2++;
			int num5 = num;
			while (num5 < maxDim)
			{
				if ((!horizontal) ? image.get_Renamed(fixedDimension, num5) : image.get_Renamed(num5, fixedDimension))
				{
					num5++;
					continue;
				}
				int num6 = num5;
				do
				{
					num5++;
				}
				while (num5 < maxDim && !((!horizontal) ? image.get_Renamed(fixedDimension, num5) : image.get_Renamed(num5, fixedDimension)));
				int num7 = num5 - num6;
				if (num5 < maxDim && num7 <= maxWhiteRun)
				{
					continue;
				}
				num5 = num6;
				break;
			}
			num5--;
			return (num5 <= num2) ? null : new int[2] { num2, num5 };
		}
	}
}
