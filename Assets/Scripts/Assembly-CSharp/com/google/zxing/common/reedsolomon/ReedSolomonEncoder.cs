using System;
using System.Collections;

namespace com.google.zxing.common.reedsolomon
{
	public sealed class ReedSolomonEncoder
	{
		private GF256 field;

		private ArrayList cachedGenerators;

		public ReedSolomonEncoder(GF256 field)
		{
			if (!GF256.QR_CODE_FIELD.Equals(field))
			{
				throw new ArgumentException("Only QR Code is supported at this time");
			}
			this.field = field;
			cachedGenerators = ArrayList.Synchronized(new ArrayList(10));
			cachedGenerators.Add(new GF256Poly(field, new int[1] { 1 }));
		}

		private GF256Poly buildGenerator(int degree)
		{
			if (degree >= cachedGenerators.Count)
			{
				GF256Poly gF256Poly = (GF256Poly)cachedGenerators[cachedGenerators.Count - 1];
				for (int i = cachedGenerators.Count; i <= degree; i++)
				{
					GF256Poly gF256Poly2 = gF256Poly.multiply(new GF256Poly(field, new int[2]
					{
						1,
						field.exp(i - 1)
					}));
					cachedGenerators.Add(gF256Poly2);
					gF256Poly = gF256Poly2;
				}
			}
			return (GF256Poly)cachedGenerators[degree];
		}

		public void encode(int[] toEncode, int ecBytes)
		{
			if (ecBytes == 0)
			{
				throw new ArgumentException("No error correction bytes");
			}
			int num = toEncode.Length - ecBytes;
			if (num <= 0)
			{
				throw new ArgumentException("No data bytes provided");
			}
			GF256Poly other = buildGenerator(ecBytes);
			int[] array = new int[num];
			Array.Copy(toEncode, 0, array, 0, num);
			GF256Poly gF256Poly = new GF256Poly(field, array);
			gF256Poly = gF256Poly.multiplyByMonomial(ecBytes, 1);
			GF256Poly gF256Poly2 = gF256Poly.divide(other)[1];
			int[] coefficients = gF256Poly2.Coefficients;
			int num2 = ecBytes - coefficients.Length;
			for (int i = 0; i < num2; i++)
			{
				toEncode[num + i] = 0;
			}
			Array.Copy(coefficients, 0, toEncode, num + num2, coefficients.Length);
		}
	}
}
