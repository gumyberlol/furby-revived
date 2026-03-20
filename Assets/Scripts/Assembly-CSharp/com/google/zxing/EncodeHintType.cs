namespace com.google.zxing
{
	public sealed class EncodeHintType
	{
		public static readonly EncodeHintType ERROR_CORRECTION = new EncodeHintType();

		public static readonly EncodeHintType CHARACTER_SET = new EncodeHintType();

		private EncodeHintType()
		{
		}
	}
}
