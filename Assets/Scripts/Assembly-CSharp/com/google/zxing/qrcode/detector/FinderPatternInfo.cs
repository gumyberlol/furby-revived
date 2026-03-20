namespace com.google.zxing.qrcode.detector
{
	public sealed class FinderPatternInfo
	{
		private FinderPattern bottomLeft;

		private FinderPattern topLeft;

		private FinderPattern topRight;

		public FinderPattern BottomLeft
		{
			get
			{
				return bottomLeft;
			}
		}

		public FinderPattern TopLeft
		{
			get
			{
				return topLeft;
			}
		}

		public FinderPattern TopRight
		{
			get
			{
				return topRight;
			}
		}

		public FinderPatternInfo(FinderPattern[] patternCenters)
		{
			bottomLeft = patternCenters[0];
			topLeft = patternCenters[1];
			topRight = patternCenters[2];
		}
	}
}
