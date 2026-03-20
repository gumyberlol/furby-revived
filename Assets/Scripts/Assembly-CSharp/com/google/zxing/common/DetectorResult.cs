namespace com.google.zxing.common
{
	public sealed class DetectorResult
	{
		private BitMatrix bits;

		private ResultPoint[] points;

		public BitMatrix Bits
		{
			get
			{
				return bits;
			}
		}

		public ResultPoint[] Points
		{
			get
			{
				return points;
			}
		}

		public DetectorResult(BitMatrix bits, ResultPoint[] points)
		{
			this.bits = bits;
			this.points = points;
		}
	}
}
