using UnityEngine;

namespace Relentless
{
	public static class MathHelper
	{
		public static bool SolveQuadratic(out float first, out float second, float a, float b, float c)
		{
			float num = b * b - 4f * a * c;
			if (num < 0f)
			{
				first = 0f;
				second = 0f;
				return false;
			}
			num = Mathf.Sqrt(num);
			float num2 = 2f * a;
			first = (0f - b + num) / num2;
			second = (0f - b - num) / num2;
			return true;
		}
	}
}
