using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Relentless.Network.Analytics.Providers
{
	public abstract class TelemetryProviderBase : ProviderBase, IProvider, ITelemetry
	{
		private const string m_playerPrefsKeyBase = "TelemeteryQueue";

		private const string PlayerPrefsKeyVersion = "TelemeteryQueueVersion";

		private const int PlayerPrefsVersion = 2;

		private Thread m_postingThread;

		private bool m_shouldPumpQueue = true;

		private bool m_shouldPumpQueueOnCoroutine;

		private object m_lock = new object();

		private PersistentQueue m_persistentQueue;

		private string m_osVer;

		private string m_fullOsVer;

		private string m_gameVersion;

		private string m_devicePlatform;

		private string m_deviceModel;

		private string m_sysInfo;

		private string m_userId;

		private Platforms m_serverPlatform;

		private DateTime ServerDateTimeForSession = DateTime.MinValue;

		private int m_sessionIndex = 1;

		private bool m_shouldSaveQueueNextTime;

		public int TelemeteryThreadWaitTimeInMilliseconds = 3000;

		public int MaxQueueSize = 70;

		protected virtual string OSVer
		{
			get
			{
				return m_osVer;
			}
		}

		protected virtual Platforms ServerPlatform
		{
			get
			{
				return m_serverPlatform;
			}
		}

		protected virtual string DevicePlatform
		{
			get
			{
				return m_devicePlatform;
			}
		}

		protected virtual string DeviceModel
		{
			get
			{
				return m_deviceModel;
			}
		}

		protected virtual string UserId
		{
			get
			{
				return m_userId;
			}
		}

		protected virtual string FullOSVer
		{
			get
			{
				return m_fullOsVer;
			}
		}

		protected virtual string GameVersion
		{
			get
			{
				return m_gameVersion;
			}
		}

		private int NextSessionIndex
		{
			get
			{
				lock (m_lock)
				{
					return m_sessionIndex++;
				}
			}
		}

		private bool ShouldPumpQueue
		{
			get
			{
				lock (m_lock)
				{
					return m_shouldPumpQueue;
				}
			}
			set
			{
				lock (m_lock)
				{
					m_shouldPumpQueue = value;
				}
			}
		}

		protected bool ShouldPumpQueueOnCoroutine
		{
			get
			{
				lock (m_lock)
				{
					return m_shouldPumpQueueOnCoroutine;
				}
			}
			set
			{
				lock (m_lock)
				{
					m_shouldPumpQueueOnCoroutine = value;
				}
			}
		}

		private string PlayerPrefsKey
		{
			get
			{
				return string.Format("{0}_{1}", "TelemeteryQueue", ProviderName);
			}
		}

		protected string SysInfo
		{
			get
			{
				return m_sysInfo;
			}
		}

		public virtual void Awake()
		{
			if (IsValid(false))
			{
				LoadOrCreateQueue();
			}
		}

		public override void Initialise()
		{
			base.Initialise();
			LoadOrCreateQueue();
			m_osVer = NetworkHelper.OSVer;
			m_serverPlatform = NetworkHelper.ServerPlatform;
			m_fullOsVer = SystemInfo.operatingSystem;
			m_devicePlatform = Application.platform.ToString();
			m_deviceModel = SystemInfo.deviceModel;
			m_gameVersion = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleVersion;
			m_sysInfo = NetworkHelper.GetSysInfo();
			if (SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName == ProfileFriendlyName.Developer || SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName == ProfileFriendlyName.Enterprise)
			{
				m_userId = SystemInfo.deviceName;
			}
			StartCoroutine(ProcessIncomingEvents());
		}

		public virtual void StartSession()
		{
			LogEvent("StartSession", false);
		}

		public virtual void EndSession()
		{
			LogEvent("EndSession", false);
		}

		public virtual void TagScreen(string screenName)
		{
			throw new NotImplementedException();
		}

		public void LogEvent(string eventName, bool isTimed)
		{
			LogEvent(eventName, null, isTimed);
		}

		public void LogEvent(string eventName, TelemetryParams telemParams, bool isTimed)
		{
			Dictionary<string, string> dictionary = null;
			if (telemParams != null)
			{
				dictionary = telemParams.Params;
			}
			else
			{
				TelemetryParams telemetryParams = new TelemetryParams();
				dictionary = telemetryParams.Params;
			}
			QueuedTelemetryEvent queuedTelemetryEvent = new QueuedTelemetryEvent();
			queuedTelemetryEvent.Timestamp = ServerDateTimeForSession;
			queuedTelemetryEvent.SessionIndex = NextSessionIndex;
			queuedTelemetryEvent.Event = eventName;
			queuedTelemetryEvent.Params = dictionary;
			QueuedTelemetryEvent telemetryEvent = queuedTelemetryEvent;
			Enqueue(telemetryEvent);
		}

		public void EndTimedEvent(string eventName)
		{
			throw new NotImplementedException();
		}

		public void EndTimedEvent(string eventName, Dictionary<string, string> parameters)
		{
			throw new NotImplementedException();
		}

		public void SetAge(int age)
		{
			throw new NotImplementedException();
		}

		public void SetGender(string gender)
		{
			throw new NotImplementedException();
		}

		public void SetUserId(string userId)
		{
			throw new NotImplementedException();
		}

		public void SetSessionReportsOnCloseEnabled(bool sendSessionReportsOnClose)
		{
			throw new NotImplementedException();
		}

		public void SetSessionReportsOnPauseEnabled(bool setSessionReportsOnPauseEnabled)
		{
			throw new NotImplementedException();
		}

		private void LoadOrCreateQueue()
		{
			lock (m_lock)
			{
				if (m_persistentQueue != null)
				{
					return;
				}
				string text = string.Empty;
				if (PlayerPrefs.HasKey("TelemeteryQueueVersion"))
				{
					int num = PlayerPrefs.GetInt("TelemeteryQueueVersion");
					if (num == 2 && PlayerPrefs.HasKey(PlayerPrefsKey))
					{
						text = PlayerPrefs.GetString(PlayerPrefsKey);
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						m_persistentQueue = JSONSerialiser.Parse<PersistentQueue>(text);
						Logging.Log("Loaded last Telemetry Queue for " + ProviderName + ". Message count = " + ((m_persistentQueue.EventQueue != null) ? m_persistentQueue.EventQueue.Count : 0));
					}
					catch (Exception ex)
					{
						m_persistentQueue = null;
						Logging.LogError("Failed to load last Telemetry Queue for " + ProviderName + " : " + ex.ToString());
					}
				}
				if (m_persistentQueue == null)
				{
					Logging.Log("Creating a new Telemetry Queue for " + ProviderName);
					m_persistentQueue = new PersistentQueue
					{
						EventQueue = new List<QueuedTelemetryEvent>(),
						LastUpdatedUtc = DateTime.UtcNow,
						Version = 2
					};
				}
			}
		}

		private void SaveQueue()
		{
			lock (m_lock)
			{
				if (m_persistentQueue != null)
				{
					m_persistentQueue.LastUpdatedUtc = DateTime.UtcNow;
					string value = JSONSerialiser.AsString(m_persistentQueue);
					PlayerPrefs.SetInt("TelemeteryQueueVersion", 2);
					PlayerPrefs.SetString(PlayerPrefsKey, value);
					PlayerPrefs.Save();
				}
			}
		}

		private QueuedTelemetryEvent PeekQueue()
		{
			lock (m_lock)
			{
				if (m_persistentQueue.EventQueue.Count == 0)
				{
					return null;
				}
				return m_persistentQueue.EventQueue[0];
			}
		}

		private QueuedTelemetryEvent PopQueue()
		{
			lock (m_lock)
			{
				if (m_persistentQueue.EventQueue.Count == 0)
				{
					return null;
				}
				QueuedTelemetryEvent result = m_persistentQueue.EventQueue[0];
				m_persistentQueue.EventQueue.RemoveAt(0);
				m_shouldSaveQueueNextTime = true;
				return result;
			}
		}

		private void Enqueue(QueuedTelemetryEvent telemetryEvent)
		{
			lock (m_lock)
			{
				if (m_persistentQueue == null)
				{
					Logging.LogError("Telemetry Queue for " + ProviderName + " is not ready yet. Dropping this message.");
					return;
				}
				if (m_persistentQueue.EventQueue.Count >= MaxQueueSize)
				{
					Logging.LogError("Reached maximum Telemetry Queue size for " + ProviderName + ". Dropping this message.");
					return;
				}
				telemetryEvent.QueueLength = m_persistentQueue.EventQueue.Count;
				m_persistentQueue.EventQueue.Add(telemetryEvent);
				m_shouldSaveQueueNextTime = true;
			}
		}

		private IEnumerator ProcessIncomingEvents()
		{
			yield return new WaitForSeconds(5f);
			if (SingletonInstance<SetupNetworking>.Instance != null)
			{
				while (!SetupNetworking.IsReady)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			ServerDateTimeForSession = SetupNetworking.ServerTimeUtcNow;
			if (SetupNetworking.IsNetworkingEnabled)
			{
				lock (m_lock)
				{
					foreach (QueuedTelemetryEvent item in m_persistentQueue.EventQueue)
					{
						if (item.Timestamp == DateTime.MinValue)
						{
							item.Timestamp = ServerDateTimeForSession;
						}
					}
					SaveQueue();
				}
				if (!ShouldPumpQueueOnCoroutine)
				{
					m_postingThread = new Thread(PostingThread);
					m_postingThread.Start();
				}
				yield return new WaitForEndOfFrame();
				Logging.Log("Started logging telemetry");
				DebugNotifications.AddNotification("Started logging telemetry for " + ProviderName, TimeSpan.FromSeconds(15.0));
				while (ShouldPumpQueue)
				{
					if (ShouldPumpQueueOnCoroutine)
					{
						while (PumpQueue())
						{
							yield return new WaitForEndOfFrame();
						}
					}
					lock (m_lock)
					{
						if (m_shouldSaveQueueNextTime)
						{
							SaveQueue();
							m_shouldSaveQueueNextTime = false;
						}
					}
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				Logging.LogError("Not logging telemetry since networking is disabled");
				DebugNotifications.AddNotification("Not logging telemetry since networking is disabled", TimeSpan.FromSeconds(15.0));
			}
		}

		private void PostingThread()
		{
			try
			{
				while (ShouldPumpQueue)
				{
					while (PumpQueue())
					{
						Thread.Sleep(10);
					}
					Thread.Sleep(TelemeteryThreadWaitTimeInMilliseconds);
				}
				PumpQueue();
			}
			catch (Exception ex)
			{
				Logging.LogError("Telemetry posting thread caught exception: " + ex.ToString());
			}
		}

		private bool PumpQueue()
		{
			QueuedTelemetryEvent queuedTelemetryEvent = PeekQueue();
			if (queuedTelemetryEvent == null)
			{
				return false;
			}
			try
			{
				queuedTelemetryEvent.Description = string.Empty;
				queuedTelemetryEvent.Version = GameVersion;
				queuedTelemetryEvent.OSVersion = FullOSVer;
				queuedTelemetryEvent.DeviceModel = DeviceModel;
				queuedTelemetryEvent.DevicePlatform = DevicePlatform;
				queuedTelemetryEvent.UserID = UserId;
				if (ReallyLogEventToServer(queuedTelemetryEvent))
				{
					PopQueue();
					return true;
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to post telemetry to server. Re-queuing event. Caught exception " + ex);
			}
			return false;
		}

		protected abstract bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent);
	}
}
