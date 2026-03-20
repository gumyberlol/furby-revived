using System.Linq;
using UnityEngine;

namespace Relentless
{
	public class DeviceRegister : RelentlessMonoBehaviour
	{
		[SerializeField]
		private DeviceProperties[] m_DeviceDatabase;

		public DeviceProperties GetAndroidPropertiesFromDeviceModel(string deviceModel)
		{
			return (from device in m_DeviceDatabase
				from deviceString in device.m_DeviceIdentifier.m_AndroidSpecific.m_InclusiveModels
				where deviceModel.Contains(deviceString.ToLower())
				select device).FirstOrDefault();
		}
	}
}
