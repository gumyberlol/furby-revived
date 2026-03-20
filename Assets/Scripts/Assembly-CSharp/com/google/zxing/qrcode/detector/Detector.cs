using System;
using System.Collections;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;

namespace com.google.zxing.qrcode.detector
{
	public class Detector
	{
		private BitMatrix image;

		private ResultPointCallback resultPointCallback;

		protected internal virtual BitMatrix Image
		{
			get
			{
				return image;
			}
		}

		protected internal virtual ResultPointCallback ResultPointCallback
		{
			get
			{
				return resultPointCallback;
			}
		}

		public Detector(BitMatrix image)
		{
			this.image = image;
		}

		public Detector()
		{
		}

		public void SetImage(BitMatrix image)
		{
			this.image = image;
		}

		public virtual DetectorResult detect()
		{
			return detect(null);
		}

		public virtual DetectorResult detect(Hashtable hints)
		{
			object obj;
			if (hints == null)
			{
				ResultPointCallback resultPointCallback = null;
				obj = resultPointCallback;
			}
			else
			{
				obj = (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
			}
			this.resultPointCallback = (ResultPointCallback)obj;
			FinderPatternFinder finderPatternFinder = new FinderPatternFinder(image, this.resultPointCallback);
			FinderPatternInfo finderPatternInfo = finderPatternFinder.find(hints);
			if (finderPatternInfo == null)
			{
				return null;
			}
			return processFinderPatternInfo(finderPatternInfo);
		}

		protected internal virtual DetectorResult processFinderPatternInfo(FinderPatternInfo info)
		{
			FinderPattern topLeft = info.TopLeft;
			FinderPattern topRight = info.TopRight;
			FinderPattern bottomLeft = info.BottomLeft;
			float num = calculateModuleSize(topLeft, topRight, bottomLeft);
			if (num < 1f)
			{
				throw ReaderException.Instance;
			}
			int dimension = computeDimension(topLeft, topRight, bottomLeft, num);
			com.google.zxing.qrcode.decoder.Version provisionalVersionForDimension = com.google.zxing.qrcode.decoder.Version.getProvisionalVersionForDimension(dimension);
			int num2 = provisionalVersionForDimension.DimensionForVersion - 7;
			AlignmentPattern alignmentPattern = null;
			if (provisionalVersionForDimension.AlignmentPatternCenters.Length > 0)
			{
				float num3 = topRight.X - topLeft.X + bottomLeft.X;
				float num4 = topRight.Y - topLeft.Y + bottomLeft.Y;
				float num5 = 1f - 3f / (float)num2;
				int estAlignmentX = (int)(topLeft.X + num5 * (num3 - topLeft.X));
				int estAlignmentY = (int)(topLeft.Y + num5 * (num4 - topLeft.Y));
				for (int num6 = 4; num6 <= 16; num6 <<= 1)
				{
					try
					{
						alignmentPattern = findAlignmentInRegion(num, estAlignmentX, estAlignmentY, num6);
					}
					catch (ReaderException)
					{
						continue;
					}
					break;
				}
			}
			PerspectiveTransform transform = createTransform(topLeft, topRight, bottomLeft, alignmentPattern, dimension);
			BitMatrix bits = sampleGrid(image, transform, dimension);
			ResultPoint[] points = ((alignmentPattern != null) ? new ResultPoint[4] { bottomLeft, topLeft, topRight, alignmentPattern } : new ResultPoint[3] { bottomLeft, topLeft, topRight });
			return new DetectorResult(bits, points);
		}

		public virtual PerspectiveTransform createTransform(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, ResultPoint alignmentPattern, int dimension)
		{
			float num = (float)dimension - 3.5f;
			float x2p;
			float y2p;
			float x;
			float y;
			if (alignmentPattern != null)
			{
				x2p = alignmentPattern.X;
				y2p = alignmentPattern.Y;
				x = (y = num - 3f);
			}
			else
			{
				x2p = topRight.X - topLeft.X + bottomLeft.X;
				y2p = topRight.Y - topLeft.Y + bottomLeft.Y;
				x = (y = num);
			}
			return PerspectiveTransform.quadrilateralToQuadrilateral(3.5f, 3.5f, num, 3.5f, x, y, 3.5f, num, topLeft.X, topLeft.Y, topRight.X, topRight.Y, x2p, y2p, bottomLeft.X, bottomLeft.Y);
		}

		private static BitMatrix sampleGrid(BitMatrix image, PerspectiveTransform transform, int dimension)
		{
			GridSampler instance = GridSampler.Instance;
			return instance.sampleGrid(image, dimension, transform);
		}

		protected internal static int computeDimension(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, float moduleSize)
		{
			int num = round(ResultPoint.distance(topLeft, topRight) / moduleSize);
			int num2 = round(ResultPoint.distance(topLeft, bottomLeft) / moduleSize);
			int num3 = (num + num2 >> 1) + 7;
			switch (num3 & 3)
			{
			case 0:
				num3++;
				break;
			case 2:
				num3--;
				break;
			case 3:
				throw ReaderException.Instance;
			}
			return num3;
		}

		protected internal virtual float calculateModuleSize(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft)
		{
			return (calculateModuleSizeOneWay(topLeft, topRight) + calculateModuleSizeOneWay(topLeft, bottomLeft)) / 2f;
		}

		private float calculateModuleSizeOneWay(ResultPoint pattern, ResultPoint otherPattern)
		{
			float num = sizeOfBlackWhiteBlackRunBothWays((int)pattern.X, (int)pattern.Y, (int)otherPattern.X, (int)otherPattern.Y);
			float num2 = sizeOfBlackWhiteBlackRunBothWays((int)otherPattern.X, (int)otherPattern.Y, (int)pattern.X, (int)pattern.Y);
			if (float.IsNaN(num))
			{
				return num2 / 7f;
			}
			if (float.IsNaN(num2))
			{
				return num / 7f;
			}
			return (num + num2) / 14f;
		}

		private float sizeOfBlackWhiteBlackRunBothWays(int fromX, int fromY, int toX, int toY)
		{
			float num = sizeOfBlackWhiteBlackRun(fromX, fromY, toX, toY);
			float num2 = 1f;
			int num3 = fromX - (toX - fromX);
			if (num3 < 0)
			{
				num2 = (float)fromX / (float)(fromX - num3);
				num3 = 0;
			}
			else if (num3 >= image.Width)
			{
				num2 = (float)(image.Width - 1 - fromX) / (float)(num3 - fromX);
				num3 = image.Width - 1;
			}
			int num4 = (int)((float)fromY - (float)(toY - fromY) * num2);
			num2 = 1f;
			if (num4 < 0)
			{
				num2 = (float)fromY / (float)(fromY - num4);
				num4 = 0;
			}
			else if (num4 >= image.Height)
			{
				num2 = (float)(image.Height - 1 - fromY) / (float)(num4 - fromY);
				num4 = image.Height - 1;
			}
			num3 = (int)((float)fromX + (float)(num3 - fromX) * num2);
			num += sizeOfBlackWhiteBlackRun(fromX, fromY, num3, num4);
			return num - 1f;
		}

		private float sizeOfBlackWhiteBlackRun(int fromX, int fromY, int toX, int toY)
		{
			bool flag = Math.Abs(toY - fromY) > Math.Abs(toX - fromX);
			if (flag)
			{
				int num = fromX;
				fromX = fromY;
				fromY = num;
				num = toX;
				toX = toY;
				toY = num;
			}
			int num2 = Math.Abs(toX - fromX);
			int num3 = Math.Abs(toY - fromY);
			int num4 = -num2 >> 1;
			int num5 = ((fromY < toY) ? 1 : (-1));
			int num6 = ((fromX < toX) ? 1 : (-1));
			int num7 = 0;
			int i = fromX;
			int num8 = fromY;
			for (; i != toX; i += num6)
			{
				int x = ((!flag) ? i : num8);
				int y = ((!flag) ? num8 : i);
				if (num7 == 1)
				{
					if (image.get_Renamed(x, y))
					{
						num7++;
					}
				}
				else if (!image.get_Renamed(x, y))
				{
					num7++;
				}
				if (num7 == 3)
				{
					int num9 = i - fromX;
					int num10 = num8 - fromY;
					return (float)Math.Sqrt(num9 * num9 + num10 * num10);
				}
				num4 += num3;
				if (num4 > 0)
				{
					if (num8 == toY)
					{
						break;
					}
					num8 += num5;
					num4 -= num2;
				}
			}
			int num11 = toX - fromX;
			int num12 = toY - fromY;
			return (float)Math.Sqrt(num11 * num11 + num12 * num12);
		}

		protected internal virtual AlignmentPattern findAlignmentInRegion(float overallEstModuleSize, int estAlignmentX, int estAlignmentY, float allowanceFactor)
		{
			int num = (int)(allowanceFactor * overallEstModuleSize);
			int num2 = Math.Max(0, estAlignmentX - num);
			int num3 = Math.Min(image.Width - 1, estAlignmentX + num);
			if ((float)(num3 - num2) < overallEstModuleSize * 3f)
			{
				throw ReaderException.Instance;
			}
			int num4 = Math.Max(0, estAlignmentY - num);
			int num5 = Math.Min(image.Height - 1, estAlignmentY + num);
			AlignmentPatternFinder alignmentPatternFinder = new AlignmentPatternFinder(image, num2, num4, num3 - num2, num5 - num4, overallEstModuleSize, resultPointCallback);
			return alignmentPatternFinder.find();
		}

		private static int round(float d)
		{
			return (int)(d + 0.5f);
		}
	}
}
