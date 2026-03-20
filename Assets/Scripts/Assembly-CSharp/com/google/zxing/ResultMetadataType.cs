namespace com.google.zxing
{
	public sealed class ResultMetadataType
	{
		public static readonly ResultMetadataType OTHER = new ResultMetadataType();

		public static readonly ResultMetadataType ORIENTATION = new ResultMetadataType();

		public static readonly ResultMetadataType BYTE_SEGMENTS = new ResultMetadataType();

		public static readonly ResultMetadataType ERROR_CORRECTION_LEVEL = new ResultMetadataType();

		private ResultMetadataType()
		{
		}
	}
}
