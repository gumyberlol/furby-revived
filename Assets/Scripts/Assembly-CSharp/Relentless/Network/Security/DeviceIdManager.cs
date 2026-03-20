using System;
using Relentless.Core.Crypto;
using UnityEngine;

namespace Relentless.Network.Security
{
	public class DeviceIdManager : SingletonInstance<DeviceIdManager>
	{
		private static bool m_hasValidDeviceId = false;

		private static object m_lock = new object();

		private string m_hashSalt = "Relentless";

		private static string m_deviceHash = null;

		private static string m_oldDeviceHash = null;

		private static string m_locale = null;

		public static bool HasValidDeviceId
		{
			get
			{
				lock (m_lock)
				{
					return m_hasValidDeviceId;
				}
			}
			private set
			{
				lock (m_lock)
				{
					m_hasValidDeviceId = value;
				}
			}
		}

		public static string DeviceId
		{
			get
			{
				if (m_deviceHash == null)
				{
					Logging.LogError("Detected use of device Id before it was set");
					return "<unset>";
				}
				return m_deviceHash;
			}
		}

		public static string OldDeviceId
		{
			get
			{
				if (m_oldDeviceHash == null)
				{
					return "<unset>";
				}
				return m_oldDeviceHash;
			}
		}

		public static string UserLocale
		{
			get
			{
				return m_locale;
			}
		}

		public override void Awake()
		{
			base.Awake();
			Logging.Log("DeviceIdManager:Awake()");
			bool hasValidDeviceId = true;
			try
			{
				m_oldDeviceHash = "FailedToSet";
				string text = string.Empty;
				try
				{
					text = NetworkInfo.FirstEthernetMACAddress;
					if (!string.IsNullOrEmpty(text))
					{
						m_oldDeviceHash = Hash.ComputeHash(text, Hash.Algorithm.MD5, m_hashSalt);
					}
					else
					{
						text = string.Empty;
					}
				}
				catch (Exception)
				{
				}
				Logging.Log("OldDeviceHash = " + m_oldDeviceHash + " (" + text + ")");
				m_deviceHash = SystemInfo.deviceUniqueIdentifier;
			}
			catch (Exception ex2)
			{
				Logging.LogError("Failed to set DeviceId : " + ex2.ToString());
				m_deviceHash = null;
			}
			if (string.IsNullOrEmpty(m_deviceHash))
			{
				m_deviceHash = "FailedToSet";
			}
			HasValidDeviceId = hasValidDeviceId;
		}
	}
}
