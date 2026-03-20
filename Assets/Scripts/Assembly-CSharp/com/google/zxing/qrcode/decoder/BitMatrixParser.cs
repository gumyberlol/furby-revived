using com.google.zxing.common;

namespace com.google.zxing.qrcode.decoder
{
	internal sealed class BitMatrixParser
	{
		private BitMatrix bitMatrix;

		private Version parsedVersion;

		private FormatInformation parsedFormatInfo;

		internal BitMatrixParser(BitMatrix bitMatrix)
		{
			int dimension = bitMatrix.Dimension;
			if (dimension < 21 || (dimension & 3) != 1)
			{
				throw ReaderException.Instance;
			}
			this.bitMatrix = bitMatrix;
		}

		internal FormatInformation readFormatInformation()
		{
			if (parsedFormatInfo != null)
			{
				return parsedFormatInfo;
			}
			int versionBits = 0;
			for (int i = 0; i < 6; i++)
			{
				versionBits = copyBit(i, 8, versionBits);
			}
			versionBits = copyBit(7, 8, versionBits);
			versionBits = copyBit(8, 8, versionBits);
			versionBits = copyBit(8, 7, versionBits);
			for (int num = 5; num >= 0; num--)
			{
				versionBits = copyBit(8, num, versionBits);
			}
			parsedFormatInfo = FormatInformation.decodeFormatInformation(versionBits);
			if (parsedFormatInfo != null)
			{
				return parsedFormatInfo;
			}
			int dimension = bitMatrix.Dimension;
			versionBits = 0;
			int num2 = dimension - 8;
			for (int num3 = dimension - 1; num3 >= num2; num3--)
			{
				versionBits = copyBit(num3, 8, versionBits);
			}
			for (int j = dimension - 7; j < dimension; j++)
			{
				versionBits = copyBit(8, j, versionBits);
			}
			parsedFormatInfo = FormatInformation.decodeFormatInformation(versionBits);
			if (parsedFormatInfo != null)
			{
				return parsedFormatInfo;
			}
			throw ReaderException.Instance;
		}

		internal Version readVersion()
		{
			if (parsedVersion != null)
			{
				return parsedVersion;
			}
			int dimension = bitMatrix.Dimension;
			int num = dimension - 17 >> 2;
			if (num <= 6)
			{
				return Version.getVersionForNumber(num);
			}
			int versionBits = 0;
			int num2 = dimension - 11;
			for (int num3 = 5; num3 >= 0; num3--)
			{
				for (int num4 = dimension - 9; num4 >= num2; num4--)
				{
					versionBits = copyBit(num4, num3, versionBits);
				}
			}
			parsedVersion = Version.decodeVersionInformation(versionBits);
			if (parsedVersion != null && parsedVersion.DimensionForVersion == dimension)
			{
				return parsedVersion;
			}
			versionBits = 0;
			for (int num5 = 5; num5 >= 0; num5--)
			{
				for (int num6 = dimension - 9; num6 >= num2; num6--)
				{
					versionBits = copyBit(num5, num6, versionBits);
				}
			}
			parsedVersion = Version.decodeVersionInformation(versionBits);
			if (parsedVersion != null && parsedVersion.DimensionForVersion == dimension)
			{
				return parsedVersion;
			}
			throw ReaderException.Instance;
		}

		private int copyBit(int i, int j, int versionBits)
		{
			return (!bitMatrix.get_Renamed(i, j)) ? (versionBits << 1) : ((versionBits << 1) | 1);
		}

		internal sbyte[] readCodewords()
		{
			FormatInformation formatInformation = readFormatInformation();
			Version version = readVersion();
			DataMask dataMask = DataMask.forReference(formatInformation.DataMask);
			int dimension = this.bitMatrix.Dimension;
			dataMask.unmaskBitMatrix(this.bitMatrix, dimension);
			BitMatrix bitMatrix = version.buildFunctionPattern();
			bool flag = true;
			sbyte[] array = new sbyte[version.TotalCodewords];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int num4 = dimension - 1; num4 > 0; num4 -= 2)
			{
				if (num4 == 6)
				{
					num4--;
				}
				for (int i = 0; i < dimension; i++)
				{
					int y = ((!flag) ? i : (dimension - 1 - i));
					for (int j = 0; j < 2; j++)
					{
						if (!bitMatrix.get_Renamed(num4 - j, y))
						{
							num3++;
							num2 <<= 1;
							if (this.bitMatrix.get_Renamed(num4 - j, y))
							{
								num2 |= 1;
							}
							if (num3 == 8)
							{
								array[num++] = (sbyte)num2;
								num3 = 0;
								num2 = 0;
							}
						}
					}
				}
				flag = (byte)((flag ? 1u : 0u) ^ 1u) != 0;
			}
			if (num != version.TotalCodewords)
			{
				throw ReaderException.Instance;
			}
			return array;
		}
	}
}
