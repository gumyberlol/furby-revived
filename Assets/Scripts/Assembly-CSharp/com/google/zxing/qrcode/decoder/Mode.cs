using System;

namespace com.google.zxing.qrcode.decoder
{
	public sealed class Mode
	{
		public static readonly Mode TERMINATOR = new Mode(new int[3], 0, "TERMINATOR");

		public static readonly Mode NUMERIC = new Mode(new int[3] { 10, 12, 14 }, 1, "NUMERIC");

		public static readonly Mode ALPHANUMERIC = new Mode(new int[3] { 9, 11, 13 }, 2, "ALPHANUMERIC");

		public static readonly Mode STRUCTURED_APPEND = new Mode(new int[3], 3, "STRUCTURED_APPEND");

		public static readonly Mode BYTE = new Mode(new int[3] { 8, 16, 16 }, 4, "BYTE");

		public static readonly Mode ECI = new Mode(null, 7, "ECI");

		public static readonly Mode KANJI = new Mode(new int[3] { 8, 10, 12 }, 8, "KANJI");

		public static readonly Mode FNC1_FIRST_POSITION = new Mode(null, 5, "FNC1_FIRST_POSITION");

		public static readonly Mode FNC1_SECOND_POSITION = new Mode(null, 9, "FNC1_SECOND_POSITION");

		private int[] characterCountBitsForVersions;

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

		private Mode(int[] characterCountBitsForVersions, int bits, string name)
		{
			this.characterCountBitsForVersions = characterCountBitsForVersions;
			this.bits = bits;
			this.name = name;
		}

		public static Mode forBits(int bits)
		{
			switch (bits)
			{
			case 0:
				return TERMINATOR;
			case 1:
				return NUMERIC;
			case 2:
				return ALPHANUMERIC;
			case 3:
				return STRUCTURED_APPEND;
			case 4:
				return BYTE;
			case 5:
				return FNC1_FIRST_POSITION;
			case 7:
				return ECI;
			case 8:
				return KANJI;
			case 9:
				return FNC1_SECOND_POSITION;
			default:
				throw new ArgumentException();
			}
		}

		public int getCharacterCountBits(Version version)
		{
			if (characterCountBitsForVersions == null)
			{
				throw new ArgumentException("Character count doesn't apply to this mode");
			}
			int versionNumber = version.VersionNumber;
			int num = ((versionNumber > 9) ? ((versionNumber <= 26) ? 1 : 2) : 0);
			return characterCountBitsForVersions[num];
		}

		public override string ToString()
		{
			return name;
		}
	}
}
