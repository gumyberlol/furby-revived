using System;
using System.Text;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;

namespace com.google.zxing.qrcode.encoder
{
	public sealed class QRCode
	{
		public const int NUM_MASK_PATTERNS = 8;

		private Mode mode;

		private ErrorCorrectionLevel ecLevel;

		private int version;

		private int matrixWidth;

		private int maskPattern;

		private int numTotalBytes;

		private int numDataBytes;

		private int numECBytes;

		private int numRSBlocks;

		private ByteMatrix matrix;

		public Mode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		public ErrorCorrectionLevel ECLevel
		{
			get
			{
				return ecLevel;
			}
			set
			{
				ecLevel = value;
			}
		}

		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public int MatrixWidth
		{
			get
			{
				return matrixWidth;
			}
			set
			{
				matrixWidth = value;
			}
		}

		public int MaskPattern
		{
			get
			{
				return maskPattern;
			}
			set
			{
				maskPattern = value;
			}
		}

		public int NumTotalBytes
		{
			get
			{
				return numTotalBytes;
			}
			set
			{
				numTotalBytes = value;
			}
		}

		public int NumDataBytes
		{
			get
			{
				return numDataBytes;
			}
			set
			{
				numDataBytes = value;
			}
		}

		public int NumECBytes
		{
			get
			{
				return numECBytes;
			}
			set
			{
				numECBytes = value;
			}
		}

		public int NumRSBlocks
		{
			get
			{
				return numRSBlocks;
			}
			set
			{
				numRSBlocks = value;
			}
		}

		public ByteMatrix Matrix
		{
			get
			{
				return matrix;
			}
			set
			{
				matrix = value;
			}
		}

		public bool Valid
		{
			get
			{
				return mode != null && ecLevel != null && version != -1 && matrixWidth != -1 && maskPattern != -1 && numTotalBytes != -1 && numDataBytes != -1 && numECBytes != -1 && numRSBlocks != -1 && isValidMaskPattern(maskPattern) && numTotalBytes == numDataBytes + numECBytes && matrix != null && matrixWidth == matrix.Width && matrix.Width == matrix.Height;
			}
		}

		public QRCode()
		{
			mode = null;
			ecLevel = null;
			version = -1;
			matrixWidth = -1;
			maskPattern = -1;
			numTotalBytes = -1;
			numDataBytes = -1;
			numECBytes = -1;
			numRSBlocks = -1;
			matrix = null;
		}

		public int at(int x, int y)
		{
			int num = matrix.get_Renamed(x, y);
			if (num != 0 && num != 1)
			{
				throw new SystemException("Bad value");
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("<<\n");
			stringBuilder.Append(" mode: ");
			stringBuilder.Append(mode);
			stringBuilder.Append("\n ecLevel: ");
			stringBuilder.Append(ecLevel);
			stringBuilder.Append("\n version: ");
			stringBuilder.Append(version);
			stringBuilder.Append("\n matrixWidth: ");
			stringBuilder.Append(matrixWidth);
			stringBuilder.Append("\n maskPattern: ");
			stringBuilder.Append(maskPattern);
			stringBuilder.Append("\n numTotalBytes: ");
			stringBuilder.Append(numTotalBytes);
			stringBuilder.Append("\n numDataBytes: ");
			stringBuilder.Append(numDataBytes);
			stringBuilder.Append("\n numECBytes: ");
			stringBuilder.Append(numECBytes);
			stringBuilder.Append("\n numRSBlocks: ");
			stringBuilder.Append(numRSBlocks);
			if (matrix == null)
			{
				stringBuilder.Append("\n matrix: null\n");
			}
			else
			{
				stringBuilder.Append("\n matrix:\n");
				stringBuilder.Append(matrix.ToString());
			}
			stringBuilder.Append(">>\n");
			return stringBuilder.ToString();
		}

		public static bool isValidMaskPattern(int maskPattern)
		{
			return maskPattern >= 0 && maskPattern < 8;
		}
	}
}
