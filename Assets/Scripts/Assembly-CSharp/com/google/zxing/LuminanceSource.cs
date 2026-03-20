using System;

namespace com.google.zxing
{
	public abstract class LuminanceSource
	{
		private int width;

		private int height;

		public abstract sbyte[] Matrix { get; }

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public virtual int Height
		{
			get
			{
				return height;
			}
		}

		public virtual bool CropSupported
		{
			get
			{
				return false;
			}
		}

		public virtual bool RotateSupported
		{
			get
			{
				return false;
			}
		}

		protected internal LuminanceSource(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public abstract sbyte[] getRow(int y, sbyte[] row);

		public virtual LuminanceSource crop(int left, int top, int width, int height)
		{
			throw new SystemException("This luminance source does not support cropping.");
		}

		public virtual LuminanceSource rotateCounterClockwise()
		{
			throw new SystemException("This luminance source does not support rotation.");
		}
	}
}
