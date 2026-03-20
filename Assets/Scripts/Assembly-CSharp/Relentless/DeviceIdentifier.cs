using System;

namespace Relentless
{
	[Serializable]
	public class DeviceIdentifier
	{
		public AndroidProperties m_AndroidSpecific;

		public IOSProperties m_IPhoneSpecific;
	}
}
