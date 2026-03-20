using System;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class UIGradientAddin
	{
		[SerializeField]
		[HideInInspector]
		public Color mGradient1 = Color.white;

		[HideInInspector]
		[SerializeField]
		public Color mGradient2 = Color.white;

		[SerializeField]
		[HideInInspector]
		public float mGradientAngle;

		public void ApplyGradient(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Color color, Transform transform)
		{
			Vector3 gradientDirection = transform.right * Mathf.Cos(mGradientAngle * ((float)Math.PI / 180f)) + transform.up * Mathf.Sin(mGradientAngle * ((float)Math.PI / 180f));
			float[] array = (from vertex in verts.ToArray()
				select Vector3.Dot(gradientDirection, vertex)).ToArray();
			float num = array.Min();
			float num2 = array.Max();
			float num3 = 1f / (num2 - num);
			Color32 a = color * mGradient1;
			Color32 b = color * mGradient2;
			for (int num4 = 0; num4 < verts.size; num4++)
			{
				cols[num4] = Color32.Lerp(a, b, (array[num4] - num) * num3);
			}
		}
	}
}
