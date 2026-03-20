using System;

namespace com.google.zxing.common
{
	public abstract class GridSampler
	{
		private static GridSampler gridSampler = new DefaultGridSampler();

		public static GridSampler Instance
		{
			get
			{
				return gridSampler;
			}
		}

		public static void setGridSampler(GridSampler newGridSampler)
		{
			if (newGridSampler == null)
			{
				throw new ArgumentException();
			}
			gridSampler = newGridSampler;
		}

		public abstract BitMatrix sampleGrid(BitMatrix image, int dimension, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);

		public virtual BitMatrix sampleGrid(BitMatrix image, int dimension, PerspectiveTransform transform)
		{
			throw new NotSupportedException();
		}

		protected internal static void checkAndNudgePoints(BitMatrix image, float[] points)
		{
			int width = image.Width;
			int height = image.Height;
			bool flag = true;
			for (int i = 0; i < points.Length; i += 2)
			{
				if (!flag)
				{
					break;
				}
				int num = (int)points[i];
				int num2 = (int)points[i + 1];
				if (num < -1 || num > width || num2 < -1 || num2 > height)
				{
					throw ReaderException.Instance;
				}
				flag = false;
				if (num == -1)
				{
					points[i] = 0f;
					flag = true;
				}
				else if (num == width)
				{
					points[i] = width - 1;
					flag = true;
				}
				if (num2 == -1)
				{
					points[i + 1] = 0f;
					flag = true;
				}
				else if (num2 == height)
				{
					points[i + 1] = height - 1;
					flag = true;
				}
			}
			flag = true;
			int num3 = points.Length - 2;
			while (num3 >= 0 && flag)
			{
				int num4 = (int)points[num3];
				int num5 = (int)points[num3 + 1];
				if (num4 < -1 || num4 > width || num5 < -1 || num5 > height)
				{
					throw ReaderException.Instance;
				}
				flag = false;
				if (num4 == -1)
				{
					points[num3] = 0f;
					flag = true;
				}
				else if (num4 == width)
				{
					points[num3] = width - 1;
					flag = true;
				}
				if (num5 == -1)
				{
					points[num3 + 1] = 0f;
					flag = true;
				}
				else if (num5 == height)
				{
					points[num3 + 1] = height - 1;
					flag = true;
				}
				num3 -= 2;
			}
		}
	}
}
