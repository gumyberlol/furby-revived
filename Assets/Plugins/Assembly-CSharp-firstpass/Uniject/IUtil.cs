using System;
using UnityEngine;

namespace Uniject
{
	public interface IUtil
	{
		RuntimePlatform Platform { get; }

		bool IsEditor { get; }

		string persistentDataPath { get; }

		DateTime currentTime { get; }

		string DeviceModel { get; }

		string DeviceName { get; }

		DeviceType DeviceType { get; }

		string DeviceId { get; }

		string OperatingSystem { get; }

		T[] getAnyComponentsOfType<T>() where T : class;

		string loadedLevelName();
	}
}
