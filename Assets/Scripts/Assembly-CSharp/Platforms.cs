using UnityEngine;

public class Platforms : MonoBehaviour
{
	private const float ratioTolerance = 0.03f;

	public const string editorPlatformOverrideKey = "editorPlatformOverride";

	private static bool platformCalculated;

	private static Platform calculatedPlatform;

	public static Platform platform
	{
		get
		{
			if (!platformCalculated)
			{
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					if (Application.platform == RuntimePlatform.Android)
					{
						calculatedPlatform = Platform.Android;
					}
					else if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
					{
						calculatedPlatform = Platform.WebPlayer;
					}
					else
					{
						calculatedPlatform = Platform.Standalone;
					}
				}
				platformCalculated = true;
			}
			return calculatedPlatform;
		}
	}

	public static bool IsiOS
	{
		get
		{
			return platform == Platform.iPad || platform == Platform.iPhone || platform == Platform.iPadRetina || platform == Platform.iPhoneRetina || platform == Platform.iPhone5;
		}
	}

	public static bool IsiOSSlower
	{
		get
		{
			if (platform == Platform.iPad || platform == Platform.iPhone || platform == Platform.iPhoneRetina)
			{
				return false;
			}
			return false;
		}
	}

	public static bool IsPlatformAspectBased(string plat)
	{
		return plat == Platform.Standalone.ToString() || plat == Platform.Android.ToString() || plat == Platform.FlashPlayer.ToString() || plat == Platform.NaCl.ToString();
	}
}
