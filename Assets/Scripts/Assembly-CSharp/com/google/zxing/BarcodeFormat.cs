using System;
using System.Collections;

namespace com.google.zxing
{
	public sealed class BarcodeFormat
	{
		private static readonly Hashtable VALUES = Hashtable.Synchronized(new Hashtable());

		public static readonly BarcodeFormat QR_CODE = new BarcodeFormat("QR_CODE");

		public static readonly BarcodeFormat DATAMATRIX = new BarcodeFormat("DATAMATRIX");

		public static readonly BarcodeFormat UPC_E = new BarcodeFormat("UPC_E");

		public static readonly BarcodeFormat UPC_A = new BarcodeFormat("UPC_A");

		public static readonly BarcodeFormat EAN_8 = new BarcodeFormat("EAN_8");

		public static readonly BarcodeFormat EAN_13 = new BarcodeFormat("EAN_13");

		public static readonly BarcodeFormat CODE_128 = new BarcodeFormat("CODE_128");

		public static readonly BarcodeFormat CODE_39 = new BarcodeFormat("CODE_39");

		public static readonly BarcodeFormat ITF = new BarcodeFormat("ITF");

		public static readonly BarcodeFormat PDF417 = new BarcodeFormat("PDF417");

		private string name;

		public string Name
		{
			get
			{
				return name;
			}
		}

		private BarcodeFormat(string name)
		{
			this.name = name;
			VALUES[name] = this;
		}

		public override string ToString()
		{
			return name;
		}

		public static BarcodeFormat valueOf(string name)
		{
			BarcodeFormat barcodeFormat = (BarcodeFormat)VALUES[name];
			if (barcodeFormat == null)
			{
				throw new ArgumentException();
			}
			return barcodeFormat;
		}
	}
}
