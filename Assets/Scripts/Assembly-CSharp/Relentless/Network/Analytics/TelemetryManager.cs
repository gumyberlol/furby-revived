using System;
using System.Collections.Generic;
using Relentless.Core.Crypto;
using Relentless.Network.Analytics.Providers;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless.Network.Analytics
{
	public class TelemetryManager : ProviderManager<TelemetryManager, TelemetryProviderBase>, IProvider, ITelemetry
	{
		private static object m_lock = new object();

		public int m_telemetryVersion;

		public string m_productName;

		private string m_hashSalt = "Relentless";

		private static string m_deviceHash = null;

		private static string m_sessionId = Guid.Empty.ToString();

		private bool m_sessionStarted;

		public ITelemetry Providers
		{
			get
			{
				return this;
			}
		}

		public static string SessionId
		{
			get
			{
				return m_sessionId;
			}
			private set
			{
				m_sessionId = value;
			}
		}

		public static string DeviceId
		{
			get
			{
				lock (m_lock)
				{
					if (m_deviceHash == null)
					{
						Logging.LogError("Detected use of device Id before it was set");
						return "<unset>";
					}
					return m_deviceHash;
				}
			}
		}

		void ITelemetry.EndTimedEvent(string eventName)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.EndTimedEvent(eventName);
			}
		}

		void ITelemetry.EndTimedEvent(string eventName, Dictionary<string, string> parameters)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.EndTimedEvent(eventName, parameters);
			}
		}

		void ITelemetry.SetAge(int age)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.SetAge(age);
			}
		}

		void ITelemetry.SetGender(string gender)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.SetGender(gender);
			}
		}

		void ITelemetry.SetUserId(string userId)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.SetUserId(userId);
			}
		}

		void ITelemetry.SetSessionReportsOnCloseEnabled(bool sendSessionReportsOnClose)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.SetSessionReportsOnCloseEnabled(sendSessionReportsOnClose);
			}
		}

		void ITelemetry.SetSessionReportsOnPauseEnabled(bool setSessionReportsOnPauseEnabled)
		{
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.SetSessionReportsOnPauseEnabled(setSessionReportsOnPauseEnabled);
			}
		}

		public override void Awake()
		{
			try
			{
				m_deviceHash = DeviceIdManager.DeviceId;
				m_sessionId = NetworkHelper.NewGuid().ToString();
				Logging.Log(string.Format("SessionId = {0}, Initial DeviceId = {1}", m_sessionId, m_deviceHash));
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to set DeviceId : " + ex.ToString());
			}
			base.Awake();
		}

		protected override void Initialise()
		{
			base.Initialise();
			if (m_sessionStarted)
			{
				return;
			}
			Logging.Log("TelemetryManager:Initialise");
			m_sessionStarted = true;
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.StartSession();
			}
			LogSessionEvent(m_telemetryVersion);
		}

		public void Destroy()
		{
			m_providers.ForEach(delegate(TelemetryProviderBase provider)
			{
				provider.EndSession();
			});
		}

		private void LogSessionEvent(int version)
		{
			string value = Hash.ComputeHash(m_productName, Hash.Algorithm.MD5, m_hashSalt);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("DeviceId", m_deviceHash);
			dictionary.Add("ProductId", value);
			dictionary.Add("TelemetryVersion", version.ToString());
			dictionary.Add("Locale", Singleton<Localisation>.Instance.CurrentLocale.ToString());
			dictionary.Add("ProfileType", SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			if (SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName == ProfileFriendlyName.Enterprise || SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName == ProfileFriendlyName.Developer)
			{
				dictionary2.Add("Profile", SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName.ToString());
				dictionary2.Add("DevId", SystemInfo.deviceUniqueIdentifier);
			}
			LogEvent("Session", new TelemetryParams(dictionary2), false);
		}

		public void LogError(string component, string message, Dictionary<string, string> parameters)
		{
			parameters.Add("Component", component);
			parameters.Add("Message", message);
			LogEvent("Error", new TelemetryParams(parameters), false);
		}

		public void LogEvent(string eventName, bool isTimed)
		{
			TelemetryParams telemParams = new TelemetryParams();
			LogEvent(eventName, telemParams, isTimed);
		}

		public void LogEvent(string eventName, TelemetryParams telemParams, bool isTimed)
		{
			Logging.Log(string.Format("TelemetryManager:LogEvent: {0}, Parameters: {1}", eventName, telemParams.DictionaryToString()));
			foreach (TelemetryProviderBase provider in m_providers)
			{
				try
				{
					provider.LogEvent(eventName, telemParams, isTimed);
				}
				catch (Exception exception)
				{
					Logging.LogException(exception);
				}
			}
		}

		public void TagScreen(string screenName)
		{
			Logging.LogWarning(string.Format("TelemetryManager:TagScreen: {0}", screenName));
			foreach (TelemetryProviderBase provider in m_providers)
			{
				provider.TagScreen(screenName);
			}
		}
	}
}
