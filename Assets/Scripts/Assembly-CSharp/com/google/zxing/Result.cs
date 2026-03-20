using System;
using System.Collections;

namespace com.google.zxing
{
	public sealed class Result
	{
		private string text;

		private sbyte[] rawBytes;

		private ResultPoint[] resultPoints;

		private BarcodeFormat format;

		private Hashtable resultMetadata;

		public string Text
		{
			get
			{
				return text;
			}
		}

		public sbyte[] RawBytes
		{
			get
			{
				return rawBytes;
			}
		}

		public ResultPoint[] ResultPoints
		{
			get
			{
				return resultPoints;
			}
		}

		public BarcodeFormat BarcodeFormat
		{
			get
			{
				return format;
			}
		}

		public Hashtable ResultMetadata
		{
			get
			{
				return resultMetadata;
			}
		}

		public Result(string text, sbyte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format)
		{
			if (text == null && rawBytes == null)
			{
				throw new ArgumentException("Text and bytes are null");
			}
			this.text = text;
			this.rawBytes = rawBytes;
			this.resultPoints = resultPoints;
			this.format = format;
			resultMetadata = null;
		}

		public void putMetadata(ResultMetadataType type, object value_Renamed)
		{
			if (resultMetadata == null)
			{
				resultMetadata = Hashtable.Synchronized(new Hashtable(3));
			}
			resultMetadata[type] = value_Renamed;
		}

		public override string ToString()
		{
			if (text == null)
			{
				return "[" + rawBytes.Length + " bytes]";
			}
			return text;
		}
	}
}
