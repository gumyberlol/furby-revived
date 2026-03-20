namespace com.google.zxing
{
	public sealed class DecodeHintType
	{
		public static readonly DecodeHintType OTHER = new DecodeHintType();

		public static readonly DecodeHintType PURE_BARCODE = new DecodeHintType();

		public static readonly DecodeHintType POSSIBLE_FORMATS = new DecodeHintType();

		public static readonly DecodeHintType TRY_HARDER = new DecodeHintType();

		public static readonly DecodeHintType ALLOWED_LENGTHS = new DecodeHintType();

		public static readonly DecodeHintType ASSUME_CODE_39_CHECK_DIGIT = new DecodeHintType();

		public static readonly DecodeHintType NEED_RESULT_POINT_CALLBACK = new DecodeHintType();

		private DecodeHintType()
		{
		}
	}
}
