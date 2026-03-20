using System;
using System.Collections;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;
using com.google.zxing.qrcode.encoder;

namespace com.google.zxing.qrcode
{
	public sealed class QRCodeWriter : Writer
	{
		private const int QUIET_ZONE_SIZE = 4;

		public ByteMatrix encode(string contents, BarcodeFormat format, int width, int height)
		{
			return encode(contents, format, width, height, null);
		}

		public ByteMatrix encode(string contents, BarcodeFormat format, int width, int height, Hashtable hints)
		{
			if (contents == null || contents.Length == 0)
			{
				throw new ArgumentException("Found empty contents");
			}
			if (format != BarcodeFormat.QR_CODE)
			{
				throw new ArgumentException("Can only encode QR_CODE, but got " + format);
			}
			if (width < 0 || height < 0)
			{
				throw new ArgumentException("Requested dimensions are too small: " + width + 'x' + height);
			}
			ErrorCorrectionLevel ecLevel = ErrorCorrectionLevel.L;
			if (hints != null)
			{
				ErrorCorrectionLevel errorCorrectionLevel = (ErrorCorrectionLevel)hints[EncodeHintType.ERROR_CORRECTION];
				if (errorCorrectionLevel != null)
				{
					ecLevel = errorCorrectionLevel;
				}
			}
			QRCode qRCode = new QRCode();
			Encoder.encode(contents, ecLevel, hints, qRCode);
			return renderResult(qRCode, width, height);
		}

		private static ByteMatrix renderResult(QRCode code, int width, int height)
		{
			ByteMatrix matrix = code.Matrix;
			int width2 = matrix.Width;
			int height2 = matrix.Height;
			int num = width2 + 8;
			int num2 = height2 + 8;
			int num3 = Math.Max(width, num);
			int num4 = Math.Max(height, num2);
			int num5 = Math.Min(num3 / num, num4 / num2);
			int num6 = (num3 - width2 * num5) / 2;
			int num7 = (num4 - height2 * num5) / 2;
			ByteMatrix byteMatrix = new ByteMatrix(num3, num4);
			sbyte[][] array = byteMatrix.Array;
			sbyte[] array2 = new sbyte[num3];
			for (int i = 0; i < num7; i++)
			{
				setRowColor(array[i], (sbyte)SupportClass.Identity(255L));
			}
			sbyte[][] array3 = matrix.Array;
			for (int j = 0; j < height2; j++)
			{
				for (int k = 0; k < num6; k++)
				{
					array2[k] = (sbyte)SupportClass.Identity(255L);
				}
				int num8 = num6;
				for (int l = 0; l < width2; l++)
				{
					sbyte b = (sbyte)((array3[j][l] != 1) ? SupportClass.Identity(255L) : 0);
					for (int m = 0; m < num5; m++)
					{
						array2[num8 + m] = b;
					}
					num8 += num5;
				}
				num8 = num6 + width2 * num5;
				for (int n = num8; n < num3; n++)
				{
					array2[n] = (sbyte)SupportClass.Identity(255L);
				}
				num8 = num7 + j * num5;
				for (int num9 = 0; num9 < num5; num9++)
				{
					Array.Copy(array2, 0, array[num8 + num9], 0, num3);
				}
			}
			int num10 = num7 + height2 * num5;
			for (int num11 = num10; num11 < num4; num11++)
			{
				setRowColor(array[num11], (sbyte)SupportClass.Identity(255L));
			}
			return byteMatrix;
		}

		private static void setRowColor(sbyte[] row, sbyte value_Renamed)
		{
			for (int i = 0; i < row.Length; i++)
			{
				row[i] = value_Renamed;
			}
		}
	}
}
