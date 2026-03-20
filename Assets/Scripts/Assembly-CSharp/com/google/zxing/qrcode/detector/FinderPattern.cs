using System;

namespace com.google.zxing.qrcode.detector
{
	public sealed class FinderPattern : ResultPoint
	{
		private float estimatedModuleSize;

		private int count;

		public float EstimatedModuleSize
		{
			get
			{
				return estimatedModuleSize;
			}
		}

		internal int Count
		{
			get
			{
				return count;
			}
		}

		internal FinderPattern(float posX, float posY, float estimatedModuleSize)
			: base(posX, posY)
		{
			this.estimatedModuleSize = estimatedModuleSize;
			count = 1;
		}

		internal void incrementCount()
		{
			count++;
		}

		internal bool aboutEquals(float moduleSize, float i, float j)
		{
			if (Math.Abs(i - Y) <= moduleSize && Math.Abs(j - X) <= moduleSize)
			{
				float num = Math.Abs(moduleSize - estimatedModuleSize);
				return num <= 1f || num / estimatedModuleSize <= 1f;
			}
			return false;
		}
	}
}
