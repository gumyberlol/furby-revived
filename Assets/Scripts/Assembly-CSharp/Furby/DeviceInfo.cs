using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class DeviceInfo
	{
		public string name;

		public string[] identifyingDeviceStrings;

		public float comAirVolume;

		public DeviceOrientation orientation;
	}
}
