namespace com.google.zxing.common.reedsolomon
{
	public sealed class ReedSolomonDecoder
	{
		private GF256 field;

		public ReedSolomonDecoder(GF256 field)
		{
			this.field = field;
		}

		public void decode(int[] received, int twoS)
		{
			GF256Poly gF256Poly = new GF256Poly(field, received);
			int[] array = new int[twoS];
			bool flag = field.Equals(GF256.DATA_MATRIX_FIELD);
			bool flag2 = true;
			for (int i = 0; i < twoS; i++)
			{
				int num = gF256Poly.evaluateAt(field.exp((!flag) ? i : (i + 1)));
				array[array.Length - 1 - i] = num;
				if (num != 0)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				return;
			}
			GF256Poly b = new GF256Poly(field, array);
			GF256Poly[] array2 = runEuclideanAlgorithm(field.buildMonomial(twoS, 1), b, twoS);
			GF256Poly errorLocator = array2[0];
			GF256Poly errorEvaluator = array2[1];
			int[] array3 = findErrorLocations(errorLocator);
			int[] array4 = findErrorMagnitudes(errorEvaluator, array3, flag);
			for (int j = 0; j < array3.Length; j++)
			{
				int num2 = received.Length - 1 - field.log(array3[j]);
				if (num2 < 0)
				{
					throw new ReedSolomonException("Bad error location");
				}
				received[num2] = GF256.addOrSubtract(received[num2], array4[j]);
			}
		}

		private GF256Poly[] runEuclideanAlgorithm(GF256Poly a, GF256Poly b, int R)
		{
			if (a.Degree < b.Degree)
			{
				GF256Poly gF256Poly = a;
				a = b;
				b = gF256Poly;
			}
			GF256Poly gF256Poly2 = a;
			GF256Poly gF256Poly3 = b;
			GF256Poly gF256Poly4 = field.One;
			GF256Poly gF256Poly5 = field.Zero;
			GF256Poly gF256Poly6 = field.Zero;
			GF256Poly gF256Poly7 = field.One;
			while (gF256Poly3.Degree >= R / 2)
			{
				GF256Poly gF256Poly8 = gF256Poly2;
				GF256Poly other = gF256Poly4;
				GF256Poly other2 = gF256Poly6;
				gF256Poly2 = gF256Poly3;
				gF256Poly4 = gF256Poly5;
				gF256Poly6 = gF256Poly7;
				if (gF256Poly2.Zero)
				{
					throw new ReedSolomonException("r_{i-1} was zero");
				}
				gF256Poly3 = gF256Poly8;
				GF256Poly gF256Poly9 = field.Zero;
				int coefficient = gF256Poly2.getCoefficient(gF256Poly2.Degree);
				int b2 = field.inverse(coefficient);
				while (gF256Poly3.Degree >= gF256Poly2.Degree && !gF256Poly3.Zero)
				{
					int degree = gF256Poly3.Degree - gF256Poly2.Degree;
					int coefficient2 = field.multiply(gF256Poly3.getCoefficient(gF256Poly3.Degree), b2);
					gF256Poly9 = gF256Poly9.addOrSubtract(field.buildMonomial(degree, coefficient2));
					gF256Poly3 = gF256Poly3.addOrSubtract(gF256Poly2.multiplyByMonomial(degree, coefficient2));
				}
				gF256Poly5 = gF256Poly9.multiply(gF256Poly4).addOrSubtract(other);
				gF256Poly7 = gF256Poly9.multiply(gF256Poly6).addOrSubtract(other2);
			}
			int coefficient3 = gF256Poly7.getCoefficient(0);
			if (coefficient3 == 0)
			{
				throw new ReedSolomonException("sigmaTilde(0) was zero");
			}
			int scalar = field.inverse(coefficient3);
			GF256Poly gF256Poly10 = gF256Poly7.multiply(scalar);
			GF256Poly gF256Poly11 = gF256Poly3.multiply(scalar);
			return new GF256Poly[2] { gF256Poly10, gF256Poly11 };
		}

		private int[] findErrorLocations(GF256Poly errorLocator)
		{
			int degree = errorLocator.Degree;
			if (degree == 1)
			{
				return new int[1] { errorLocator.getCoefficient(1) };
			}
			int[] array = new int[degree];
			int num = 0;
			for (int i = 1; i < 256; i++)
			{
				if (num >= degree)
				{
					break;
				}
				if (errorLocator.evaluateAt(i) == 0)
				{
					array[num] = field.inverse(i);
					num++;
				}
			}
			if (num != degree)
			{
				throw new ReedSolomonException("Error locator degree does not match number of roots");
			}
			return array;
		}

		private int[] findErrorMagnitudes(GF256Poly errorEvaluator, int[] errorLocations, bool dataMatrix)
		{
			int num = errorLocations.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = field.inverse(errorLocations[i]);
				int a = 1;
				for (int j = 0; j < num; j++)
				{
					if (i != j)
					{
						a = field.multiply(a, GF256.addOrSubtract(1, field.multiply(errorLocations[j], num2)));
					}
				}
				array[i] = field.multiply(errorEvaluator.evaluateAt(num2), field.inverse(a));
				if (dataMatrix)
				{
					array[i] = field.multiply(array[i], num2);
				}
			}
			return array;
		}
	}
}
