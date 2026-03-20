using System;
using System.Text;

namespace com.google.zxing.common.reedsolomon
{
	internal sealed class GF256Poly
	{
		private GF256 field;

		private int[] coefficients;

		internal int[] Coefficients
		{
			get
			{
				return coefficients;
			}
		}

		internal int Degree
		{
			get
			{
				return coefficients.Length - 1;
			}
		}

		internal bool Zero
		{
			get
			{
				return coefficients[0] == 0;
			}
		}

		internal GF256Poly(GF256 field, int[] coefficients)
		{
			if (coefficients == null || coefficients.Length == 0)
			{
				throw new ArgumentException();
			}
			this.field = field;
			int num = coefficients.Length;
			if (num > 1 && coefficients[0] == 0)
			{
				int i;
				for (i = 1; i < num && coefficients[i] == 0; i++)
				{
				}
				if (i == num)
				{
					this.coefficients = field.Zero.coefficients;
					return;
				}
				this.coefficients = new int[num - i];
				Array.Copy(coefficients, i, this.coefficients, 0, this.coefficients.Length);
			}
			else
			{
				this.coefficients = coefficients;
			}
		}

		internal int getCoefficient(int degree)
		{
			return coefficients[coefficients.Length - 1 - degree];
		}

		internal int evaluateAt(int a)
		{
			if (a == 0)
			{
				return getCoefficient(0);
			}
			int num = coefficients.Length;
			if (a == 1)
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					num2 = GF256.addOrSubtract(num2, coefficients[i]);
				}
				return num2;
			}
			int num3 = coefficients[0];
			for (int j = 1; j < num; j++)
			{
				num3 = GF256.addOrSubtract(field.multiply(a, num3), coefficients[j]);
			}
			return num3;
		}

		internal GF256Poly addOrSubtract(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (Zero)
			{
				return other;
			}
			if (other.Zero)
			{
				return this;
			}
			int[] array = coefficients;
			int[] array2 = other.coefficients;
			if (array.Length > array2.Length)
			{
				int[] array3 = array;
				array = array2;
				array2 = array3;
			}
			int[] array4 = new int[array2.Length];
			int num = array2.Length - array.Length;
			Array.Copy(array2, 0, array4, 0, num);
			for (int i = num; i < array2.Length; i++)
			{
				array4[i] = GF256.addOrSubtract(array[i - num], array2[i]);
			}
			return new GF256Poly(field, array4);
		}

		internal GF256Poly multiply(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (Zero || other.Zero)
			{
				return field.Zero;
			}
			int[] array = coefficients;
			int num = array.Length;
			int[] array2 = other.coefficients;
			int num2 = array2.Length;
			int[] array3 = new int[num + num2 - 1];
			for (int i = 0; i < num; i++)
			{
				int a = array[i];
				for (int j = 0; j < num2; j++)
				{
					array3[i + j] = GF256.addOrSubtract(array3[i + j], field.multiply(a, array2[j]));
				}
			}
			return new GF256Poly(field, array3);
		}

		internal GF256Poly multiply(int scalar)
		{
			switch (scalar)
			{
			case 0:
				return field.Zero;
			case 1:
				return this;
			default:
			{
				int num = coefficients.Length;
				int[] array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = field.multiply(coefficients[i], scalar);
				}
				return new GF256Poly(field, array);
			}
			}
		}

		internal GF256Poly multiplyByMonomial(int degree, int coefficient)
		{
			if (degree < 0)
			{
				throw new ArgumentException();
			}
			if (coefficient == 0)
			{
				return field.Zero;
			}
			int num = coefficients.Length;
			int[] array = new int[num + degree];
			for (int i = 0; i < num; i++)
			{
				array[i] = field.multiply(coefficients[i], coefficient);
			}
			return new GF256Poly(field, array);
		}

		internal GF256Poly[] divide(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (other.Zero)
			{
				throw new ArgumentException("Divide by 0");
			}
			GF256Poly gF256Poly = field.Zero;
			GF256Poly gF256Poly2 = this;
			int coefficient = other.getCoefficient(other.Degree);
			int b = field.inverse(coefficient);
			while (gF256Poly2.Degree >= other.Degree && !gF256Poly2.Zero)
			{
				int degree = gF256Poly2.Degree - other.Degree;
				int coefficient2 = field.multiply(gF256Poly2.getCoefficient(gF256Poly2.Degree), b);
				GF256Poly other2 = other.multiplyByMonomial(degree, coefficient2);
				GF256Poly other3 = field.buildMonomial(degree, coefficient2);
				gF256Poly = gF256Poly.addOrSubtract(other3);
				gF256Poly2 = gF256Poly2.addOrSubtract(other2);
			}
			return new GF256Poly[2] { gF256Poly, gF256Poly2 };
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(8 * Degree);
			for (int num = Degree; num >= 0; num--)
			{
				int num2 = getCoefficient(num);
				if (num2 != 0)
				{
					if (num2 < 0)
					{
						stringBuilder.Append(" - ");
						num2 = -num2;
					}
					else if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" + ");
					}
					if (num == 0 || num2 != 1)
					{
						int num3 = field.log(num2);
						switch (num3)
						{
						case 0:
							stringBuilder.Append('1');
							break;
						case 1:
							stringBuilder.Append('a');
							break;
						default:
							stringBuilder.Append("a^");
							stringBuilder.Append(num3);
							break;
						}
					}
					switch (num)
					{
					case 1:
						stringBuilder.Append('x');
						break;
					default:
						stringBuilder.Append("x^");
						stringBuilder.Append(num);
						break;
					case 0:
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
