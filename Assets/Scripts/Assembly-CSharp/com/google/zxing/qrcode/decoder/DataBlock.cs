using System;

namespace com.google.zxing.qrcode.decoder
{
	internal sealed class DataBlock
	{
		private int numDataCodewords;

		private sbyte[] codewords;

		internal int NumDataCodewords
		{
			get
			{
				return numDataCodewords;
			}
		}

		internal sbyte[] Codewords
		{
			get
			{
				return codewords;
			}
		}

		private DataBlock(int numDataCodewords, sbyte[] codewords)
		{
			this.numDataCodewords = numDataCodewords;
			this.codewords = codewords;
		}

		internal static DataBlock[] getDataBlocks(sbyte[] rawCodewords, Version version, ErrorCorrectionLevel ecLevel)
		{
			if (rawCodewords.Length != version.TotalCodewords)
			{
				throw new ArgumentException();
			}
			Version.ECBlocks eCBlocksForLevel = version.getECBlocksForLevel(ecLevel);
			int num = 0;
			Version.ECB[] eCBlocks = eCBlocksForLevel.getECBlocks();
			for (int i = 0; i < eCBlocks.Length; i++)
			{
				num += eCBlocks[i].Count;
			}
			DataBlock[] array = new DataBlock[num];
			int num2 = 0;
			foreach (Version.ECB eCB in eCBlocks)
			{
				for (int k = 0; k < eCB.Count; k++)
				{
					int dataCodewords = eCB.DataCodewords;
					int num3 = eCBlocksForLevel.ECCodewordsPerBlock + dataCodewords;
					array[num2++] = new DataBlock(dataCodewords, new sbyte[num3]);
				}
			}
			int num4 = array[0].codewords.Length;
			int num5;
			for (num5 = array.Length - 1; num5 >= 0; num5--)
			{
				int num6 = array[num5].codewords.Length;
				if (num6 == num4)
				{
					break;
				}
			}
			num5++;
			int num7 = num4 - eCBlocksForLevel.ECCodewordsPerBlock;
			int num8 = 0;
			for (int l = 0; l < num7; l++)
			{
				for (int m = 0; m < num2; m++)
				{
					array[m].codewords[l] = rawCodewords[num8++];
				}
			}
			for (int n = num5; n < num2; n++)
			{
				array[n].codewords[num7] = rawCodewords[num8++];
			}
			int num9 = array[0].codewords.Length;
			for (int num10 = num7; num10 < num9; num10++)
			{
				for (int num11 = 0; num11 < num2; num11++)
				{
					int num12 = ((num11 >= num5) ? (num10 + 1) : num10);
					array[num11].codewords[num12] = rawCodewords[num8++];
				}
			}
			return array;
		}
	}
}
