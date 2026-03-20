using System;

namespace Relentless
{
	[Serializable]
	public class DeviceProperties
	{
		public string m_DisplayName;

		public ApplicationModifiers m_ApplicationModifiers;

		public HardwareConfiguration m_HardwareConfiguration;

		public DeviceIdentifier m_DeviceIdentifier;
	}
}
