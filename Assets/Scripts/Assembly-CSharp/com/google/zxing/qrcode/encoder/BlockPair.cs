using com.google.zxing.common;

namespace com.google.zxing.qrcode.encoder
{
	internal sealed class BlockPair
	{
		private ByteArray dataBytes;

		private ByteArray errorCorrectionBytes;

		public ByteArray DataBytes
		{
			get
			{
				return dataBytes;
			}
		}

		public ByteArray ErrorCorrectionBytes
		{
			get
			{
				return errorCorrectionBytes;
			}
		}

		internal BlockPair(ByteArray data, ByteArray errorCorrection)
		{
			dataBytes = data;
			errorCorrectionBytes = errorCorrection;
		}
	}
}
