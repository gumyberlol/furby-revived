using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DeviceDatabase : MonoBehaviour
	{
		[SerializeField]
		private DeviceInfo[] m_devices;

		private DeviceInfo m_currentDevice;

		public DeviceInfo DeviceInfo
		{
			get
			{
				if (m_currentDevice == null)
				{
					FindDevice();
				}
				return m_currentDevice;
			}
		}

		private void FindDevice()
		{
			string deviceModel = SystemInfo.deviceModel.ToLower();
			Logging.Log("Device Model: " + deviceModel);
			m_currentDevice = (from device in m_devices
				from deviceString in device.identifyingDeviceStrings
				where deviceModel.Contains(deviceString.ToLower())
				select device).FirstOrDefault();
			if (m_currentDevice == null)
			{
				Logging.Log("No matching device! choosing first one");
				m_currentDevice = m_devices[0];
			}
			Logging.Log("Chosen Device: " + m_currentDevice.name);
		}
	}
}
