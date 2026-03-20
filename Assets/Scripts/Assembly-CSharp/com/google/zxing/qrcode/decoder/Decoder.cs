using com.google.zxing.common;
using com.google.zxing.common.reedsolomon;

namespace com.google.zxing.qrcode.decoder
{
	public sealed class Decoder
	{
		private ReedSolomonDecoder rsDecoder;

		public Decoder()
		{
			rsDecoder = new ReedSolomonDecoder(GF256.QR_CODE_FIELD);
		}

		public DecoderResult decode(bool[][] image)
		{
			int num = image.Length;
			BitMatrix bitMatrix = new BitMatrix(num);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (image[i][j])
					{
						bitMatrix.set_Renamed(j, i);
					}
				}
			}
			return decode(bitMatrix);
		}

		public DecoderResult decode(BitMatrix bits)
		{
			BitMatrixParser bitMatrixParser = new BitMatrixParser(bits);
			Version version = bitMatrixParser.readVersion();
			ErrorCorrectionLevel errorCorrectionLevel = bitMatrixParser.readFormatInformation().ErrorCorrectionLevel;
			sbyte[] rawCodewords = bitMatrixParser.readCodewords();
			DataBlock[] dataBlocks = DataBlock.getDataBlocks(rawCodewords, version, errorCorrectionLevel);
			int num = 0;
			for (int i = 0; i < dataBlocks.Length; i++)
			{
				num += dataBlocks[i].NumDataCodewords;
			}
			sbyte[] array = new sbyte[num];
			int num2 = 0;
			foreach (DataBlock dataBlock in dataBlocks)
			{
				sbyte[] codewords = dataBlock.Codewords;
				int numDataCodewords = dataBlock.NumDataCodewords;
				correctErrors(codewords, numDataCodewords);
				for (int k = 0; k < numDataCodewords; k++)
				{
					array[num2++] = codewords[k];
				}
			}
			return DecodedBitStreamParser.decode(array, version, errorCorrectionLevel);
		}

		private void correctErrors(sbyte[] codewordBytes, int numDataCodewords)
		{
			int num = codewordBytes.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = codewordBytes[i] & 0xFF;
			}
			int twoS = codewordBytes.Length - numDataCodewords;
			try
			{
				rsDecoder.decode(array, twoS);
			}
			catch (ReedSolomonException)
			{
				throw ReaderException.Instance;
			}
			for (int j = 0; j < numDataCodewords; j++)
			{
				codewordBytes[j] = (sbyte)array[j];
			}
		}
	}
}
