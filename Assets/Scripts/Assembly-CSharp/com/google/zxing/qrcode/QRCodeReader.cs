using System;
using System.Collections;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;
using com.google.zxing.qrcode.detector;

namespace com.google.zxing.qrcode
{
	public class QRCodeReader : Reader
	{
		private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

		private Decoder decoder = new Decoder();

		protected internal virtual Decoder Decoder
		{
			get
			{
				return decoder;
			}
		}

		public virtual Result decode(BinaryBitmap image)
		{
			return decode(image, null);
		}

		public virtual Result decode(BinaryBitmap image, Hashtable hints)
		{
			DecoderResult decoderResult;
			ResultPoint[] resultPoints;
			if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
			{
				BitMatrix bits = extractPureBits(image.BlackMatrix);
				decoderResult = decoder.decode(bits);
				resultPoints = NO_POINTS;
			}
			else
			{
				DetectorResult detectorResult = new Detector(image.BlackMatrix).detect(hints);
				decoderResult = decoder.decode(detectorResult.Bits);
				resultPoints = detectorResult.Points;
			}
			Result result = new Result(decoderResult.Text, decoderResult.RawBytes, resultPoints, BarcodeFormat.QR_CODE);
			if (decoderResult.ByteSegments != null)
			{
				result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, decoderResult.ByteSegments);
			}
			if (decoderResult.ECLevel != null)
			{
				result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, decoderResult.ECLevel.ToString());
			}
			return result;
		}

		private static BitMatrix extractPureBits(BitMatrix image)
		{
			int height = image.Height;
			int width = image.Width;
			int num = Math.Min(height, width);
			int i;
			for (i = 0; i < num && !image.get_Renamed(i, i); i++)
			{
			}
			if (i == num)
			{
				throw ReaderException.Instance;
			}
			int j;
			for (j = i; j < num && image.get_Renamed(j, j); j++)
			{
			}
			if (j == num)
			{
				throw ReaderException.Instance;
			}
			int num2 = j - i;
			int num3 = width - 1;
			while (num3 >= 0 && !image.get_Renamed(num3, i))
			{
				num3--;
			}
			if (num3 < 0)
			{
				throw ReaderException.Instance;
			}
			num3++;
			if ((num3 - i) % num2 != 0)
			{
				throw ReaderException.Instance;
			}
			int num4 = (num3 - i) / num2;
			i += num2 >> 1;
			int num5 = i + (num4 - 1) * num2;
			if (num5 >= width || num5 >= height)
			{
				throw ReaderException.Instance;
			}
			BitMatrix bitMatrix = new BitMatrix(num4);
			for (int k = 0; k < num4; k++)
			{
				int y = i + k * num2;
				for (int l = 0; l < num4; l++)
				{
					if (image.get_Renamed(i + l * num2, y))
					{
						bitMatrix.set_Renamed(l, k);
					}
				}
			}
			return bitMatrix;
		}
	}
}
