using System;
using System.Text;

namespace com.google.zxing
{
	public class ResultPoint
	{
		private float x;

		private float y;

		public virtual float X
		{
			get
			{
				return x;
			}
		}

		public virtual float Y
		{
			get
			{
				return y;
			}
		}

		public ResultPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object other)
		{
			if (other is ResultPoint)
			{
				ResultPoint resultPoint = (ResultPoint)other;
				return x == resultPoint.x && y == resultPoint.y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(25);
			stringBuilder.Append('(');
			stringBuilder.Append(x);
			stringBuilder.Append(',');
			stringBuilder.Append(y);
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public static void orderBestPatterns(ResultPoint[] patterns)
		{
			float num = distance(patterns[0], patterns[1]);
			float num2 = distance(patterns[1], patterns[2]);
			float num3 = distance(patterns[0], patterns[2]);
			ResultPoint resultPoint;
			ResultPoint resultPoint2;
			ResultPoint resultPoint3;
			if (num2 >= num && num2 >= num3)
			{
				resultPoint = patterns[0];
				resultPoint2 = patterns[1];
				resultPoint3 = patterns[2];
			}
			else if (num3 >= num2 && num3 >= num)
			{
				resultPoint = patterns[1];
				resultPoint2 = patterns[0];
				resultPoint3 = patterns[2];
			}
			else
			{
				resultPoint = patterns[2];
				resultPoint2 = patterns[0];
				resultPoint3 = patterns[1];
			}
			if (crossProductZ(resultPoint2, resultPoint, resultPoint3) < 0f)
			{
				ResultPoint resultPoint4 = resultPoint2;
				resultPoint2 = resultPoint3;
				resultPoint3 = resultPoint4;
			}
			patterns[0] = resultPoint2;
			patterns[1] = resultPoint;
			patterns[2] = resultPoint3;
		}

		public static float distance(ResultPoint pattern1, ResultPoint pattern2)
		{
			float num = pattern1.X - pattern2.X;
			float num2 = pattern1.Y - pattern2.Y;
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		private static float crossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
		{
			float num = pointB.x;
			float num2 = pointB.y;
			return (pointC.x - num) * (pointA.y - num2) - (pointC.y - num2) * (pointA.x - num);
		}
	}
}
