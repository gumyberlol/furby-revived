using UnityEngine;

namespace Relentless
{
	public class MicrophoneMeter
	{
		public static float kPcmVolumeFloor = 20f * Mathf.Log10(1.5258789E-05f);

		public static void Initialise()
		{
		}

		public static float GetPeakVolume()
		{
			float value = kPcmVolumeFloor;
			return Mathf.Clamp(value, kPcmVolumeFloor, 0f);
		}

		public static void Shutdown()
		{
		}
	}
}
