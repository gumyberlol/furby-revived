using System;
using System.Collections;
using System.IO;
using System.Text;
using com.google.zxing.common;

namespace com.google.zxing.qrcode.decoder
{
	internal sealed class DecodedBitStreamParser
	{
		private const string SHIFT_JIS = "SJIS";

		private const string EUC_JP = "EUC_JP";

		private const string UTF8 = "UTF8";

		private const string ISO88591 = "ISO-8859-1";

		private static readonly char[] ALPHANUMERIC_CHARS;

		private static bool ASSUME_SHIFT_JIS;

		private DecodedBitStreamParser()
		{
		}

		static DecodedBitStreamParser()
		{
			ALPHANUMERIC_CHARS = new char[45]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
				'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
				'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*',
				'+', '-', '.', '/', ':'
			};
			ASSUME_SHIFT_JIS = false;
		}

		internal static DecoderResult decode(sbyte[] bytes, Version version, ErrorCorrectionLevel ecLevel)
		{
			BitSource bitSource = new BitSource(bytes);
			StringBuilder stringBuilder = new StringBuilder(50);
			CharacterSetECI characterSetECI = null;
			bool fc1InEffect = false;
			ArrayList arrayList = ArrayList.Synchronized(new ArrayList(1));
			Mode mode;
			do
			{
				if (bitSource.available() < 4)
				{
					mode = Mode.TERMINATOR;
				}
				else
				{
					try
					{
						mode = Mode.forBits(bitSource.readBits(4));
					}
					catch (ArgumentException)
					{
						throw ReaderException.Instance;
					}
				}
				if (mode.Equals(Mode.TERMINATOR))
				{
					continue;
				}
				if (mode.Equals(Mode.FNC1_FIRST_POSITION) || mode.Equals(Mode.FNC1_SECOND_POSITION))
				{
					fc1InEffect = true;
					continue;
				}
				if (mode.Equals(Mode.STRUCTURED_APPEND))
				{
					bitSource.readBits(16);
					continue;
				}
				if (mode.Equals(Mode.ECI))
				{
					int value_Renamed = parseECIValue(bitSource);
					characterSetECI = CharacterSetECI.getCharacterSetECIByValue(value_Renamed);
					if (characterSetECI == null)
					{
						throw ReaderException.Instance;
					}
					continue;
				}
				int count = bitSource.readBits(mode.getCharacterCountBits(version));
				if (mode.Equals(Mode.NUMERIC))
				{
					decodeNumericSegment(bitSource, stringBuilder, count);
					continue;
				}
				if (mode.Equals(Mode.ALPHANUMERIC))
				{
					decodeAlphanumericSegment(bitSource, stringBuilder, count, fc1InEffect);
					continue;
				}
				if (mode.Equals(Mode.BYTE))
				{
					decodeByteSegment(bitSource, stringBuilder, count, characterSetECI, arrayList);
					continue;
				}
				if (mode.Equals(Mode.KANJI))
				{
					decodeKanjiSegment(bitSource, stringBuilder, count);
					continue;
				}
				throw ReaderException.Instance;
			}
			while (!mode.Equals(Mode.TERMINATOR));
			return new DecoderResult(bytes, stringBuilder.ToString(), (arrayList.Count != 0) ? arrayList : null, ecLevel);
		}

		private static void decodeKanjiSegment(BitSource bits, StringBuilder result, int count)
		{
			sbyte[] array = new sbyte[2 * count];
			int num = 0;
			while (count > 0)
			{
				int num2 = bits.readBits(13);
				int num3 = (num2 / 192 << 8) | (num2 % 192);
				num3 = ((num3 >= 7936) ? (num3 + 49472) : (num3 + 33088));
				array[num] = (sbyte)(num3 >> 8);
				array[num + 1] = (sbyte)num3;
				num += 2;
				count--;
			}
			try
			{
				result.Append(Encoding.GetEncoding("SJIS").GetString(SupportClass.ToByteArray(array)));
			}
			catch (IOException)
			{
				throw ReaderException.Instance;
			}
		}

		private static void decodeByteSegment(BitSource bits, StringBuilder result, int count, CharacterSetECI currentCharacterSetECI, ArrayList byteSegments)
		{
			sbyte[] array = new sbyte[count];
			if (count << 3 > bits.available())
			{
				throw ReaderException.Instance;
			}
			for (int i = 0; i < count; i++)
			{
				array[i] = (sbyte)bits.readBits(8);
			}
			string name = ((currentCharacterSetECI != null) ? currentCharacterSetECI.EncodingName : guessEncoding(array));
			try
			{
				result.Append(Encoding.GetEncoding(name).GetString(SupportClass.ToByteArray(array)));
			}
			catch (IOException)
			{
				throw ReaderException.Instance;
			}
			byteSegments.Add(SupportClass.ToByteArray(array));
		}

		private static void decodeAlphanumericSegment(BitSource bits, StringBuilder result, int count, bool fc1InEffect)
		{
			int length = result.Length;
			while (count > 1)
			{
				int num = bits.readBits(11);
				result.Append(ALPHANUMERIC_CHARS[num / 45]);
				result.Append(ALPHANUMERIC_CHARS[num % 45]);
				count -= 2;
			}
			if (count == 1)
			{
				result.Append(ALPHANUMERIC_CHARS[bits.readBits(6)]);
			}
			if (!fc1InEffect)
			{
				return;
			}
			for (int i = length; i < result.Length; i++)
			{
				if (result[i] == '%')
				{
					if (i < result.Length - 1 && result[i + 1] == '%')
					{
						result.Remove(i + 1, 1);
					}
					else
					{
						result[i] = '\u001d';
					}
				}
			}
		}

		private static void decodeNumericSegment(BitSource bits, StringBuilder result, int count)
		{
			while (count >= 3)
			{
				int num = bits.readBits(10);
				if (num >= 1000)
				{
					throw ReaderException.Instance;
				}
				result.Append(ALPHANUMERIC_CHARS[num / 100]);
				result.Append(ALPHANUMERIC_CHARS[num / 10 % 10]);
				result.Append(ALPHANUMERIC_CHARS[num % 10]);
				count -= 3;
			}
			switch (count)
			{
			case 2:
			{
				int num3 = bits.readBits(7);
				if (num3 >= 100)
				{
					throw ReaderException.Instance;
				}
				result.Append(ALPHANUMERIC_CHARS[num3 / 10]);
				result.Append(ALPHANUMERIC_CHARS[num3 % 10]);
				break;
			}
			case 1:
			{
				int num2 = bits.readBits(4);
				if (num2 >= 10)
				{
					throw ReaderException.Instance;
				}
				result.Append(ALPHANUMERIC_CHARS[num2]);
				break;
			}
			}
		}

		private static string guessEncoding(sbyte[] bytes)
		{
			if (ASSUME_SHIFT_JIS)
			{
				return "SJIS";
			}
			if (bytes.Length > 3 && bytes[0] == (sbyte)SupportClass.Identity(239L) && bytes[1] == (sbyte)SupportClass.Identity(187L) && bytes[2] == (sbyte)SupportClass.Identity(191L))
			{
				return "UTF8";
			}
			int num = bytes.Length;
			bool flag = true;
			bool flag2 = true;
			int num2 = 0;
			int num3 = 0;
			bool flag3 = false;
			bool flag4 = false;
			for (int i = 0; i < num; i++)
			{
				if (!flag && !flag2)
				{
					break;
				}
				int num4 = bytes[i] & 0xFF;
				if ((num4 == 194 || num4 == 195) && i < num - 1)
				{
					int num5 = bytes[i + 1] & 0xFF;
					if (num5 <= 191 && ((num4 == 194 && num5 >= 160) || (num4 == 195 && num5 >= 128)))
					{
						flag3 = true;
					}
				}
				if (num4 >= 127 && num4 <= 159)
				{
					flag = false;
				}
				if (num4 >= 161 && num4 <= 223 && !flag4)
				{
					num3++;
				}
				if (!flag4 && ((num4 >= 240 && num4 <= 255) || num4 == 128 || num4 == 160))
				{
					flag2 = false;
				}
				if ((num4 >= 129 && num4 <= 159) || (num4 >= 224 && num4 <= 239))
				{
					if (flag4)
					{
						flag4 = false;
						continue;
					}
					flag4 = true;
					if (i >= bytes.Length - 1)
					{
						flag2 = false;
						continue;
					}
					int num6 = bytes[i + 1] & 0xFF;
					if (num6 < 64 || num6 > 252)
					{
						flag2 = false;
					}
					else
					{
						num2++;
					}
				}
				else
				{
					flag4 = false;
				}
			}
			if (flag2 && (num2 >= 3 || 20 * num3 > num))
			{
				return "SJIS";
			}
			if (!flag3 && flag)
			{
				return "ISO-8859-1";
			}
			return "UTF8";
		}

		private static int parseECIValue(BitSource bits)
		{
			int num = bits.readBits(8);
			if ((num & 0x80) == 0)
			{
				return num & 0x7F;
			}
			if ((num & 0xC0) == 128)
			{
				int num2 = bits.readBits(8);
				return ((num & 0x3F) << 8) | num2;
			}
			if ((num & 0xE0) == 192)
			{
				int num3 = bits.readBits(16);
				return ((num & 0x1F) << 16) | num3;
			}
			throw new ArgumentException("Bad ECI bits starting with byte " + num);
		}
	}
}
