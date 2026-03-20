using System;
using com.google.zxing.common;

namespace com.google.zxing
{
	public abstract class Binarizer
	{
		private LuminanceSource source;

		public virtual LuminanceSource LuminanceSource
		{
			get
			{
				return source;
			}
		}

		public abstract BitMatrix BlackMatrix { get; }

		protected internal Binarizer(LuminanceSource source)
		{
			if (source == null)
			{
				throw new ArgumentException("Source must be non-null.");
			}
			this.source = source;
		}

		public abstract BitArray getBlackRow(int y, BitArray row);

		public abstract Binarizer createBinarizer(LuminanceSource source);
	}
}
