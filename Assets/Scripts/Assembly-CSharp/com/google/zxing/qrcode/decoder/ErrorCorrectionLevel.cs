using System;

namespace com.google.zxing.qrcode.decoder
{
	public sealed class ErrorCorrectionLevel
	{
		public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 1, "L");

		public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0, "M");

		public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 3, "Q");

		public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 2, "H");

		private static readonly ErrorCorrectionLevel[] FOR_BITS = new ErrorCorrectionLevel[4] { M, L, H, Q };

		private int ordinal_Renamed_Field;

		private int bits;

		private string name;

		public int Bits
		{
			get
			{
				return bits;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		private ErrorCorrectionLevel(int ordinal, int bits, string name)
		{
			ordinal_Renamed_Field = ordinal;
			this.bits = bits;
			this.name = name;
		}

		public int ordinal()
		{
			return ordinal_Renamed_Field;
		}

		public override string ToString()
		{
			return name;
		}

		public static ErrorCorrectionLevel forBits(int bits)
		{
			if (bits < 0 || bits >= FOR_BITS.Length)
			{
				throw new ArgumentException();
			}
			return FOR_BITS[bits];
		}
	}
}
