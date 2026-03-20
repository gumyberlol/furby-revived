using UnityEngine;

namespace Relentless
{
	public class HSV
	{
		public static Vector3 RGBtoHSV(Color RGB)
		{
			return RGBtoHSV(new Vector3(RGB.r, RGB.g, RGB.b));
		}

		public static Vector3 RGBtoHSV(Vector3 RGB)
		{
			Vector3 zero = Vector3.zero;
			zero.z = Mathf.Max(RGB.x, Mathf.Max(RGB.y, RGB.z));
			float num = Mathf.Min(RGB.x, Mathf.Min(RGB.y, RGB.z));
			float num2 = zero.z - num;
			if (num2 != 0f)
			{
				zero.y = num2 / zero.z;
				Vector3 vector = new Vector3((zero.z - RGB.x) / num2, (zero.z - RGB.y) / num2, (zero.z - RGB.z) / num2);
				vector -= new Vector3(vector.z, vector.x, vector.y);
				vector.x += 2f;
				vector.y += 4f;
				if (RGB.x >= zero.z)
				{
					zero.x = vector.z;
				}
				else if (RGB.y >= zero.z)
				{
					zero.x = vector.x;
				}
				else
				{
					zero.x = vector.y;
				}
				float num3 = zero.x / 6f;
				zero.x = num3 - Mathf.Floor(num3);
			}
			return zero;
		}

		private static Vector3 Hue(float H)
		{
			float x = Mathf.Abs(H * 6f - 3f) - 1f;
			float y = 2f - Mathf.Abs(H * 6f - 2f);
			float z = 2f - Mathf.Abs(H * 6f - 4f);
			return Vector3.Min(Vector3.Max(new Vector3(x, y, z), Vector3.zero), Vector3.one);
		}

		public static Vector3 HSVtoRGB(Vector3 HSV)
		{
			return ((Hue(HSV.x) - Vector3.one) * HSV.y + Vector3.one) * HSV.z;
		}
	}
}
