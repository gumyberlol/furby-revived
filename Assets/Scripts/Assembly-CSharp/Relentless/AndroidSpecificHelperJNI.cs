using UnityEngine;

namespace Relentless
{
	public class AndroidSpecificHelperJNI : SingletonInstance<AndroidSpecificHelperJNI>
	{
		private const string CLASS_NAME = "com.relentless.AndroidSpecificHelperActivity";

		private AndroidJavaClass m_javaClass;

		public override void Awake()
		{
			Logging.Log("AndroidSpecificHelperJNI:Awake()");
			base.Awake();
			AndroidJNI.AttachCurrentThread();
			m_javaClass = new AndroidJavaClass("com.relentless.AndroidSpecificHelperActivity");
			if (m_javaClass == null)
			{
				Logging.LogError("AndroidSpecificHelperJNI: Failed to get java class com.relentless.AndroidSpecificHelperActivity");
			}
		}

		public string GetIpAddress()
		{
			if (m_javaClass != null)
			{
				return m_javaClass.CallStatic<string>("GetIPAddress", new object[0]);
			}
			return string.Empty;
		}

		public string GetMacAddressHex()
		{
			if (m_javaClass != null)
			{
				return m_javaClass.CallStatic<string>("GetMacAddress", new object[0]);
			}
			return string.Empty;
		}
	}
}
