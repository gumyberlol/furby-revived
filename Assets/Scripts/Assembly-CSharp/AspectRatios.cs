using UnityEngine;

public class AspectRatios : MonoBehaviour
{
	private const float ratioTolerance = 0.03f;

	private const float aspect4By3Ratio = 1.3333334f;

	private const float aspect5by4Ratio = 1.25f;

	private const float aspect16By9Ratio = 1.7777778f;

	private const float aspect16By10Ratio = 1.6f;

	private const float aspect3by2Ratio = 1.5f;

	private const float aspectCustom1024x600 = 1.7066667f;

	private const float aspectCustom800x480 = 1.6666666f;

	public static AspectRatio GetAspectRatio()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		float num3 = num / num2;
		if (num == 1024f && num2 == 600f)
		{
			return AspectRatio.AspectCustom1024x600;
		}
		if (num == 800f && num2 == 480f)
		{
			return AspectRatio.AspectCustom800x480;
		}
		if (Mathf.Abs(num3 - 1.3333334f) < 0.03f)
		{
			return AspectRatio.Aspect4by3;
		}
		if (Mathf.Abs(num3 - 1.25f) < 0.03f)
		{
			return AspectRatio.Aspect5by4;
		}
		if (Mathf.Abs(num3 - 1.7777778f) < 0.03f)
		{
			return AspectRatio.Aspect16by9;
		}
		if (Mathf.Abs(num3 - 1.6f) < 0.03f)
		{
			return AspectRatio.Aspect16by10;
		}
		if (Mathf.Abs(num3 - 1.5f) < 0.03f)
		{
			return AspectRatio.Aspect3by2;
		}
		return FindNearestAspectRatio(num3);
	}

	private static AspectRatio FindNearestAspectRatio(float calculatedAspectRatio)
	{
		float num = float.MinValue;
		float num2 = float.MaxValue;
		float[] array = new float[5] { 1.3333334f, 1.25f, 1.7777778f, 1.6f, 1.5f };
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = Mathf.Abs(calculatedAspectRatio - array[i]);
			if (num3 < num2)
			{
				num = array[i];
				num2 = num;
			}
		}
		if (num == 1.3333334f)
		{
			return AspectRatio.Aspect4by3;
		}
		if (num == 1.25f)
		{
			return AspectRatio.Aspect5by4;
		}
		if (num == 1.7777778f)
		{
			return AspectRatio.Aspect16by9;
		}
		if (num == 1.6f)
		{
			return AspectRatio.Aspect16by10;
		}
		if (num == 1.5f)
		{
			return AspectRatio.Aspect3by2;
		}
		return AspectRatio.AspectOthers;
	}
}
