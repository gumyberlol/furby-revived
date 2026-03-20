using System;
using System.Collections;
using com.google.zxing.qrcode.decoder;

namespace com.google.zxing.common
{
	public sealed class DecoderResult
	{
		private sbyte[] rawBytes;

		private string text;

		private ArrayList byteSegments;

		private ErrorCorrectionLevel ecLevel;

		public sbyte[] RawBytes
		{
			get
			{
				return rawBytes;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
		}

		public ArrayList ByteSegments
		{
			get
			{
				return byteSegments;
			}
		}

		public ErrorCorrectionLevel ECLevel
		{
			get
			{
				return ecLevel;
			}
		}

		public DecoderResult(sbyte[] rawBytes, string text, ArrayList byteSegments, ErrorCorrectionLevel ecLevel)
		{
			if (rawBytes == null && text == null)
			{
				throw new ArgumentException();
			}
			this.rawBytes = rawBytes;
			this.text = text;
			this.byteSegments = byteSegments;
			this.ecLevel = ecLevel;
		}
	}
}
