namespace com.google.zxing.common
{
	public sealed class PerspectiveTransform
	{
		private float a11;

		private float a12;

		private float a13;

		private float a21;

		private float a22;

		private float a23;

		private float a31;

		private float a32;

		private float a33;

		private PerspectiveTransform(float a11, float a21, float a31, float a12, float a22, float a32, float a13, float a23, float a33)
		{
			this.a11 = a11;
			this.a12 = a12;
			this.a13 = a13;
			this.a21 = a21;
			this.a22 = a22;
			this.a23 = a23;
			this.a31 = a31;
			this.a32 = a32;
			this.a33 = a33;
		}

		public static PerspectiveTransform quadrilateralToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float x0p, float y0p, float x1p, float y1p, float x2p, float y2p, float x3p, float y3p)
		{
			PerspectiveTransform other = quadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
			PerspectiveTransform perspectiveTransform = squareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p);
			return perspectiveTransform.times(other);
		}

		public void transformPoints(float[] points)
		{
			int num = points.Length;
			float num2 = a11;
			float num3 = a12;
			float num4 = a13;
			float num5 = a21;
			float num6 = a22;
			float num7 = a23;
			float num8 = a31;
			float num9 = a32;
			float num10 = a33;
			for (int i = 0; i < num; i += 2)
			{
				float num11 = points[i];
				float num12 = points[i + 1];
				float num13 = num4 * num11 + num7 * num12 + num10;
				points[i] = (num2 * num11 + num5 * num12 + num8) / num13;
				points[i + 1] = (num3 * num11 + num6 * num12 + num9) / num13;
			}
		}

		public void transformPoints(float[] xValues, float[] yValues)
		{
			int num = xValues.Length;
			for (int i = 0; i < num; i++)
			{
				float num2 = xValues[i];
				float num3 = yValues[i];
				float num4 = a13 * num2 + a23 * num3 + a33;
				xValues[i] = (a11 * num2 + a21 * num3 + a31) / num4;
				yValues[i] = (a12 * num2 + a22 * num3 + a32) / num4;
			}
		}

		public static PerspectiveTransform squareToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
		{
			float num = y3 - y2;
			float num2 = y0 - y1 + y2 - y3;
			if (num == 0f && num2 == 0f)
			{
				return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0f, 0f, 1f);
			}
			float num3 = x1 - x2;
			float num4 = x3 - x2;
			float num5 = x0 - x1 + x2 - x3;
			float num6 = y1 - y2;
			float num7 = num3 * num - num4 * num6;
			float num8 = (num5 * num - num4 * num2) / num7;
			float num9 = (num3 * num2 - num5 * num6) / num7;
			return new PerspectiveTransform(x1 - x0 + num8 * x1, x3 - x0 + num9 * x3, x0, y1 - y0 + num8 * y1, y3 - y0 + num9 * y3, y0, num8, num9, 1f);
		}

		public static PerspectiveTransform quadrilateralToSquare(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
		{
			return squareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).buildAdjoint();
		}

		internal PerspectiveTransform buildAdjoint()
		{
			return new PerspectiveTransform(a22 * a33 - a23 * a32, a23 * a31 - a21 * a33, a21 * a32 - a22 * a31, a13 * a32 - a12 * a33, a11 * a33 - a13 * a31, a12 * a31 - a11 * a32, a12 * a23 - a13 * a22, a13 * a21 - a11 * a23, a11 * a22 - a12 * a21);
		}

		internal PerspectiveTransform times(PerspectiveTransform other)
		{
			return new PerspectiveTransform(a11 * other.a11 + a21 * other.a12 + a31 * other.a13, a11 * other.a21 + a21 * other.a22 + a31 * other.a23, a11 * other.a31 + a21 * other.a32 + a31 * other.a33, a12 * other.a11 + a22 * other.a12 + a32 * other.a13, a12 * other.a21 + a22 * other.a22 + a32 * other.a23, a12 * other.a31 + a22 * other.a32 + a32 * other.a33, a13 * other.a11 + a23 * other.a12 + a33 * other.a13, a13 * other.a21 + a23 * other.a22 + a33 * other.a23, a13 * other.a31 + a23 * other.a32 + a33 * other.a33);
		}
	}
}
