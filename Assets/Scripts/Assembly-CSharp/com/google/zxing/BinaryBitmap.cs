using System;
using com.google.zxing.common;

namespace com.google.zxing
{
	public sealed class BinaryBitmap
	{
		private Binarizer binarizer;

		private BitMatrix matrix;

		public int Width
		{
			get
			{
				return binarizer.LuminanceSource.Width;
			}
		}

		public int Height
		{
			get
			{
				return binarizer.LuminanceSource.Height;
			}
		}

		public BitMatrix BlackMatrix
		{
			get
			{
				if (matrix == null)
				{
					matrix = binarizer.BlackMatrix;
				}
				return matrix;
			}
		}

		public bool CropSupported
		{
			get
			{
				return binarizer.LuminanceSource.CropSupported;
			}
		}

		public bool RotateSupported
		{
			get
			{
				return binarizer.LuminanceSource.RotateSupported;
			}
		}

		public BinaryBitmap(Binarizer binarizer)
		{
			if (binarizer == null)
			{
				throw new ArgumentException("Binarizer must be non-null.");
			}
			this.binarizer = binarizer;
			matrix = null;
		}

		public void Reset()
		{
			matrix = null;
		}

		public BitArray getBlackRow(int y, BitArray row)
		{
			return binarizer.getBlackRow(y, row);
		}

		public BinaryBitmap crop(int left, int top, int width, int height)
		{
			LuminanceSource source = binarizer.LuminanceSource.crop(left, top, width, height);
			return new BinaryBitmap(binarizer.createBinarizer(source));
		}

		public BinaryBitmap rotateCounterClockwise()
		{
			LuminanceSource source = binarizer.LuminanceSource.rotateCounterClockwise();
			return new BinaryBitmap(binarizer.createBinarizer(source));
		}
	}
}
