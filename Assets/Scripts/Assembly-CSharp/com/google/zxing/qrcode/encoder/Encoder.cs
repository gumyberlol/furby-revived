using System;
using System.Collections;
using System.IO;
using System.Text;
using com.google.zxing.common;
using com.google.zxing.common.reedsolomon;
using com.google.zxing.qrcode.decoder;

namespace com.google.zxing.qrcode.encoder
{
	public sealed class Encoder
	{
		internal const string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";

		private static readonly int[] ALPHANUMERIC_TABLE = new int[96]
		{
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, 36, -1, -1, -1, 37, 38, -1, -1,
			-1, -1, 39, 40, -1, 41, 42, 43, 0, 1,
			2, 3, 4, 5, 6, 7, 8, 9, 44, -1,
			-1, -1, -1, -1, -1, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, 26, 27, 28, 29, 30, 31, 32, 33, 34,
			35, -1, -1, -1, -1, -1
		};

		private Encoder()
		{
		}

		private static int calculateMaskPenalty(ByteMatrix matrix)
		{
			int num = 0;
			num += MaskUtil.applyMaskPenaltyRule1(matrix);
			num += MaskUtil.applyMaskPenaltyRule2(matrix);
			num += MaskUtil.applyMaskPenaltyRule3(matrix);
			return num + MaskUtil.applyMaskPenaltyRule4(matrix);
		}

		public static void encode(string content, ErrorCorrectionLevel ecLevel, QRCode qrCode)
		{
			encode(content, ecLevel, null, qrCode);
		}

		public static void encode(string content, ErrorCorrectionLevel ecLevel, Hashtable hints, QRCode qrCode)
		{
			string text = ((hints != null) ? ((string)hints[EncodeHintType.CHARACTER_SET]) : null);
			if (text == null)
			{
				text = "ISO-8859-1";
			}
			Mode mode = chooseMode(content, text);
			BitVector bitVector = new BitVector();
			appendBytes(content, mode, bitVector, text);
			int numInputBytes = bitVector.sizeInBytes();
			initQRCode(numInputBytes, ecLevel, mode, qrCode);
			BitVector bitVector2 = new BitVector();
			if (mode == Mode.BYTE && !"ISO-8859-1".Equals(text))
			{
				CharacterSetECI characterSetECIByName = CharacterSetECI.getCharacterSetECIByName(text);
				if (characterSetECIByName != null)
				{
					appendECI(characterSetECIByName, bitVector2);
				}
			}
			appendModeInfo(mode, bitVector2);
			int numLetters = ((!mode.Equals(Mode.BYTE)) ? content.Length : bitVector.sizeInBytes());
			appendLengthInfo(numLetters, qrCode.Version, mode, bitVector2);
			bitVector2.appendBitVector(bitVector);
			terminateBits(qrCode.NumDataBytes, bitVector2);
			BitVector bitVector3 = new BitVector();
			interleaveWithECBytes(bitVector2, qrCode.NumTotalBytes, qrCode.NumDataBytes, qrCode.NumRSBlocks, bitVector3);
			ByteMatrix matrix = new ByteMatrix(qrCode.MatrixWidth, qrCode.MatrixWidth);
			qrCode.MaskPattern = chooseMaskPattern(bitVector3, qrCode.ECLevel, qrCode.Version, matrix);
			MatrixUtil.buildMatrix(bitVector3, qrCode.ECLevel, qrCode.Version, qrCode.MaskPattern, matrix);
			qrCode.Matrix = matrix;
			if (!qrCode.Valid)
			{
				throw new WriterException("Invalid QR code: " + qrCode.ToString());
			}
		}

		internal static int getAlphanumericCode(int code)
		{
			if (code < ALPHANUMERIC_TABLE.Length)
			{
				return ALPHANUMERIC_TABLE[code];
			}
			return -1;
		}

		public static Mode chooseMode(string content)
		{
			return chooseMode(content, null);
		}

		public static Mode chooseMode(string content, string encoding)
		{
			if ("Shift_JIS".Equals(encoding))
			{
				return (!isOnlyDoubleByteKanji(content)) ? Mode.BYTE : Mode.KANJI;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (char c in content)
			{
				if (c >= '0' && c <= '9')
				{
					flag = true;
					continue;
				}
				if (getAlphanumericCode(c) != -1)
				{
					flag2 = true;
					continue;
				}
				return Mode.BYTE;
			}
			if (flag2)
			{
				return Mode.ALPHANUMERIC;
			}
			if (flag)
			{
				return Mode.NUMERIC;
			}
			return Mode.BYTE;
		}

		private static bool isOnlyDoubleByteKanji(string content)
		{
			sbyte[] array;
			try
			{
				array = SupportClass.ToSByteArray(Encoding.GetEncoding("Shift_JIS").GetBytes(content));
			}
			catch (IOException)
			{
				return false;
			}
			int num = array.Length;
			if (num % 2 != 0)
			{
				return false;
			}
			for (int i = 0; i < num; i += 2)
			{
				int num2 = array[i] & 0xFF;
				if ((num2 < 129 || num2 > 159) && (num2 < 224 || num2 > 235))
				{
					return false;
				}
			}
			return true;
		}

		private static int chooseMaskPattern(BitVector bits, ErrorCorrectionLevel ecLevel, int version, ByteMatrix matrix)
		{
			int num = int.MaxValue;
			int result = -1;
			for (int i = 0; i < 8; i++)
			{
				MatrixUtil.buildMatrix(bits, ecLevel, version, i, matrix);
				int num2 = calculateMaskPenalty(matrix);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		private static void initQRCode(int numInputBytes, ErrorCorrectionLevel ecLevel, Mode mode, QRCode qrCode)
		{
			qrCode.ECLevel = ecLevel;
			qrCode.Mode = mode;
			for (int i = 1; i <= 40; i++)
			{
				com.google.zxing.qrcode.decoder.Version versionForNumber = com.google.zxing.qrcode.decoder.Version.getVersionForNumber(i);
				int totalCodewords = versionForNumber.TotalCodewords;
				com.google.zxing.qrcode.decoder.Version.ECBlocks eCBlocksForLevel = versionForNumber.getECBlocksForLevel(ecLevel);
				int totalECCodewords = eCBlocksForLevel.TotalECCodewords;
				int numBlocks = eCBlocksForLevel.NumBlocks;
				int num = totalCodewords - totalECCodewords;
				if (num >= numInputBytes + 3)
				{
					qrCode.Version = i;
					qrCode.NumTotalBytes = totalCodewords;
					qrCode.NumDataBytes = num;
					qrCode.NumRSBlocks = numBlocks;
					qrCode.NumECBytes = totalECCodewords;
					qrCode.MatrixWidth = versionForNumber.DimensionForVersion;
					return;
				}
			}
			throw new WriterException("Cannot find proper rs block info (input data too big?)");
		}

		internal static void terminateBits(int numDataBytes, BitVector bits)
		{
			int num = numDataBytes << 3;
			if (bits.size() > num)
			{
				throw new WriterException("data bits cannot fit in the QR Code" + bits.size() + " > " + num);
			}
			for (int i = 0; i < 4; i++)
			{
				if (bits.size() >= num)
				{
					break;
				}
				bits.appendBit(0);
			}
			int num2 = bits.size() % 8;
			if (num2 > 0)
			{
				int num3 = 8 - num2;
				for (int j = 0; j < num3; j++)
				{
					bits.appendBit(0);
				}
			}
			if (bits.size() % 8 != 0)
			{
				throw new WriterException("Number of bits is not a multiple of 8");
			}
			int num4 = numDataBytes - bits.sizeInBytes();
			for (int k = 0; k < num4; k++)
			{
				if (k % 2 == 0)
				{
					bits.appendBits(236, 8);
				}
				else
				{
					bits.appendBits(17, 8);
				}
			}
			if (bits.size() != num)
			{
				throw new WriterException("Bits size does not equal capacity");
			}
		}

		internal static void getNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks, int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock)
		{
			if (blockID >= numRSBlocks)
			{
				throw new WriterException("Block ID too large");
			}
			int num = numTotalBytes % numRSBlocks;
			int num2 = numRSBlocks - num;
			int num3 = numTotalBytes / numRSBlocks;
			int num4 = num3 + 1;
			int num5 = numDataBytes / numRSBlocks;
			int num6 = num5 + 1;
			int num7 = num3 - num5;
			int num8 = num4 - num6;
			if (num7 != num8)
			{
				throw new WriterException("EC bytes mismatch");
			}
			if (numRSBlocks != num2 + num)
			{
				throw new WriterException("RS blocks mismatch");
			}
			if (numTotalBytes != (num5 + num7) * num2 + (num6 + num8) * num)
			{
				throw new WriterException("Total bytes mismatch");
			}
			if (blockID < num2)
			{
				numDataBytesInBlock[0] = num5;
				numECBytesInBlock[0] = num7;
			}
			else
			{
				numDataBytesInBlock[0] = num6;
				numECBytesInBlock[0] = num8;
			}
		}

		internal static void interleaveWithECBytes(BitVector bits, int numTotalBytes, int numDataBytes, int numRSBlocks, BitVector result)
		{
			if (bits.sizeInBytes() != numDataBytes)
			{
				throw new WriterException("Number of bits and data bytes does not match");
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			ArrayList arrayList = ArrayList.Synchronized(new ArrayList(numRSBlocks));
			for (int i = 0; i < numRSBlocks; i++)
			{
				int[] array = new int[1];
				int[] array2 = new int[1];
				getNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, i, array, array2);
				ByteArray byteArray = new ByteArray();
				byteArray.set_Renamed(bits.Array, num, array[0]);
				ByteArray byteArray2 = generateECBytes(byteArray, array2[0]);
				arrayList.Add(new BlockPair(byteArray, byteArray2));
				num2 = Math.Max(num2, byteArray.size());
				num3 = Math.Max(num3, byteArray2.size());
				num += array[0];
			}
			if (numDataBytes != num)
			{
				throw new WriterException("Data bytes does not match offset");
			}
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < arrayList.Count; k++)
				{
					ByteArray dataBytes = ((BlockPair)arrayList[k]).DataBytes;
					if (j < dataBytes.size())
					{
						result.appendBits(dataBytes.at(j), 8);
					}
				}
			}
			for (int l = 0; l < num3; l++)
			{
				for (int m = 0; m < arrayList.Count; m++)
				{
					ByteArray errorCorrectionBytes = ((BlockPair)arrayList[m]).ErrorCorrectionBytes;
					if (l < errorCorrectionBytes.size())
					{
						result.appendBits(errorCorrectionBytes.at(l), 8);
					}
				}
			}
			if (numTotalBytes != result.sizeInBytes())
			{
				throw new WriterException("Interleaving error: " + numTotalBytes + " and " + result.sizeInBytes() + " differ.");
			}
		}

		internal static ByteArray generateECBytes(ByteArray dataBytes, int numEcBytesInBlock)
		{
			int num = dataBytes.size();
			int[] array = new int[num + numEcBytesInBlock];
			for (int i = 0; i < num; i++)
			{
				array[i] = dataBytes.at(i);
			}
			new ReedSolomonEncoder(GF256.QR_CODE_FIELD).encode(array, numEcBytesInBlock);
			ByteArray byteArray = new ByteArray(numEcBytesInBlock);
			for (int j = 0; j < numEcBytesInBlock; j++)
			{
				byteArray.set_Renamed(j, array[num + j]);
			}
			return byteArray;
		}

		internal static void appendModeInfo(Mode mode, BitVector bits)
		{
			bits.appendBits(mode.Bits, 4);
		}

		internal static void appendLengthInfo(int numLetters, int version, Mode mode, BitVector bits)
		{
			int characterCountBits = mode.getCharacterCountBits(com.google.zxing.qrcode.decoder.Version.getVersionForNumber(version));
			if (numLetters > (1 << characterCountBits) - 1)
			{
				throw new WriterException(numLetters + "is bigger than" + ((1 << characterCountBits) - 1));
			}
			bits.appendBits(numLetters, characterCountBits);
		}

		internal static void appendBytes(string content, Mode mode, BitVector bits, string encoding)
		{
			if (mode.Equals(Mode.NUMERIC))
			{
				appendNumericBytes(content, bits);
				return;
			}
			if (mode.Equals(Mode.ALPHANUMERIC))
			{
				appendAlphanumericBytes(content, bits);
				return;
			}
			if (mode.Equals(Mode.BYTE))
			{
				append8BitBytes(content, bits, encoding);
				return;
			}
			if (mode.Equals(Mode.KANJI))
			{
				appendKanjiBytes(content, bits);
				return;
			}
			throw new WriterException("Invalid mode: " + mode);
		}

		internal static void appendNumericBytes(string content, BitVector bits)
		{
			int length = content.Length;
			int num = 0;
			while (num < length)
			{
				int num2 = content[num] - 48;
				if (num + 2 < length)
				{
					int num3 = content[num + 1] - 48;
					int num4 = content[num + 2] - 48;
					bits.appendBits(num2 * 100 + num3 * 10 + num4, 10);
					num += 3;
				}
				else if (num + 1 < length)
				{
					int num5 = content[num + 1] - 48;
					bits.appendBits(num2 * 10 + num5, 7);
					num += 2;
				}
				else
				{
					bits.appendBits(num2, 4);
					num++;
				}
			}
		}

		internal static void appendAlphanumericBytes(string content, BitVector bits)
		{
			int length = content.Length;
			int num = 0;
			while (num < length)
			{
				int alphanumericCode = getAlphanumericCode(content[num]);
				if (alphanumericCode == -1)
				{
					throw new WriterException();
				}
				if (num + 1 < length)
				{
					int alphanumericCode2 = getAlphanumericCode(content[num + 1]);
					if (alphanumericCode2 == -1)
					{
						throw new WriterException();
					}
					bits.appendBits(alphanumericCode * 45 + alphanumericCode2, 11);
					num += 2;
				}
				else
				{
					bits.appendBits(alphanumericCode, 6);
					num++;
				}
			}
		}

		internal static void append8BitBytes(string content, BitVector bits, string encoding)
		{
			sbyte[] array;
			try
			{
				array = SupportClass.ToSByteArray(Encoding.GetEncoding(encoding).GetBytes(content));
			}
			catch (IOException ex)
			{
				throw new WriterException(ex.ToString());
			}
			for (int i = 0; i < array.Length; i++)
			{
				bits.appendBits(array[i], 8);
			}
		}

		internal static void appendKanjiBytes(string content, BitVector bits)
		{
			sbyte[] array;
			try
			{
				array = SupportClass.ToSByteArray(Encoding.GetEncoding("Shift_JIS").GetBytes(content));
			}
			catch (IOException ex)
			{
				throw new WriterException(ex.ToString());
			}
			int num = array.Length;
			for (int i = 0; i < num; i += 2)
			{
				int num2 = array[i] & 0xFF;
				int num3 = array[i + 1] & 0xFF;
				int num4 = (num2 << 8) | num3;
				int num5 = -1;
				if (num4 >= 33088 && num4 <= 40956)
				{
					num5 = num4 - 33088;
				}
				else if (num4 >= 57408 && num4 <= 60351)
				{
					num5 = num4 - 49472;
				}
				if (num5 == -1)
				{
					throw new WriterException("Invalid byte sequence");
				}
				int value_Renamed = (num5 >> 8) * 192 + (num5 & 0xFF);
				bits.appendBits(value_Renamed, 13);
			}
		}

		private static void appendECI(CharacterSetECI eci, BitVector bits)
		{
			bits.appendBits(Mode.ECI.Bits, 4);
			bits.appendBits(eci.Value, 8);
		}
	}
}
