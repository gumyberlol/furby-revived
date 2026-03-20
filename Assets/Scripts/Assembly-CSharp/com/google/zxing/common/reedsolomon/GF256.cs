using System;

namespace com.google.zxing.common.reedsolomon
{
	public sealed class GF256
	{
		public static readonly GF256 QR_CODE_FIELD = new GF256(285);

		public static readonly GF256 DATA_MATRIX_FIELD = new GF256(301);

		private int[] expTable;

		private int[] logTable;

		private GF256Poly zero;

		private GF256Poly one;

		internal GF256Poly Zero
		{
			get
			{
				return zero;
			}
		}

		internal GF256Poly One
		{
			get
			{
				return one;
			}
		}

		private GF256(int primitive)
		{
			expTable = new int[256];
			logTable = new int[256];
			int num = 1;
			for (int i = 0; i < 256; i++)
			{
				expTable[i] = num;
				num <<= 1;
				if (num >= 256)
				{
					num ^= primitive;
				}
			}
			for (int j = 0; j < 255; j++)
			{
				logTable[expTable[j]] = j;
			}
			zero = new GF256Poly(this, new int[1]);
			one = new GF256Poly(this, new int[1] { 1 });
		}

		internal GF256Poly buildMonomial(int degree, int coefficient)
		{
			if (degree < 0)
			{
				throw new ArgumentException();
			}
			if (coefficient == 0)
			{
				return zero;
			}
			int[] array = new int[degree + 1];
			array[0] = coefficient;
			return new GF256Poly(this, array);
		}

		internal static int addOrSubtract(int a, int b)
		{
			return a ^ b;
		}

		internal int exp(int a)
		{
			return expTable[a];
		}

		internal int log(int a)
		{
			if (a == 0)
			{
				throw new ArgumentException();
			}
			return logTable[a];
		}

		internal int inverse(int a)
		{
			if (a == 0)
			{
				throw new ArithmeticException();
			}
			return expTable[255 - logTable[a]];
		}

		internal int multiply(int a, int b)
		{
			if (a == 0 || b == 0)
			{
				return 0;
			}
			if (a == 1)
			{
				return b;
			}
			if (b == 1)
			{
				return a;
			}
			return expTable[(logTable[a] + logTable[b]) % 255];
		}
	}
}
