using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Relentless;
using Relentless.Network;
using Relentless.Network.Analytics;
using Relentless.Network.Security;
using UnityEngine;

namespace Furby
{
	public class GameConfigDownloader : SingletonInstance<GameConfigDownloader>
	{
		private object m_Lock = new object();

		private Thread m_Thread;

		public ManualResetEvent FinishedEvent = new ManualResetEvent(false);

		[HideInInspector]
		public ApplicationProperties m_AppProperties = new ApplicationProperties();

		[SerializeField]
		public Servers m_DownloadServer = Servers.None;

		[SerializeField]
		public DefaultGameConfigs m_DefaultGameConfigs;

		[SerializeField]
		public string[] m_AlternativeGameConfigs;

		[HideInInspector]
		public string m_TargetGameConfig = string.Empty;

		[SerializeField]
		private bool m_EnableVerboseLogging = true;

		[HideInInspector]
		public bool m_AmInitialised;

		[HideInInspector]
		private bool m_DownloadSuccess;

		[HideInInspector]
		private bool m_RequestSuccess;

		[HideInInspector]
		private bool m_AmDownloading;

		private string m_DecodedData = string.Empty;

		[HideInInspector]
		private TimeSpan m_DownloadTime = default(TimeSpan);

		[HideInInspector]
		private TimeSpan m_InitializationTime = default(TimeSpan);

		[SerializeField]
		public GameConfigDownloadMode m_Mode;

		private bool m_DownloadedDataWasApplied;

		[HideInInspector]
		public GameConfigBlob m_ServerResponse = new GameConfigBlob();

		private string m_CountryCode = string.Empty;

		public bool AmDownloading
		{
			get
			{
				return m_AmDownloading;
			}
		}

		public void GameConfigLogger(string logString)
		{
		}

		public override void Awake()
		{
			base.Awake();
			StartCoroutine(InitialiseSelf());
		}

		private IEnumerator InitialiseSelf()
		{
			GameConfigLogger("GameConfigDownloader::Initializing...");
			Stopwatch startupStopwatch = Stopwatch.StartNew();
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			m_AmInitialised = true;
			startupStopwatch.Stop();
			m_InitializationTime = new TimeSpan(startupStopwatch.ElapsedTicks);
			GameConfigLogger("GameConfigDownloader:: Initialization Took " + m_InitializationTime.ToString());
			SetupNetworking.GetServerDetails(Servers.Kepler);
			m_AppProperties.CacheApplicationProperties();
			GameConfigLogger("GameConfigDownloader:: ...Initialised!");
			if (m_Mode == GameConfigDownloadMode.AutomaticOnBootOnceOnly)
			{
				m_TargetGameConfig = GetPlatformAppropriateGameConfig();
				DebugUtils.Log_InMagenta("DownloadGameConfig_Ex:: [" + m_TargetGameConfig + "]");
				StartDownloadingNewGameConfiguration();
			}
		}

		public void AbortDownloadOfData()
		{
			if (m_AmInitialised && m_RequestSuccess && m_AmDownloading)
			{
				if (m_Thread != null)
				{
					m_Thread.Abort();
					m_Thread = null;
				}
				m_RequestSuccess = false;
				m_AmDownloading = false;
				m_DownloadSuccess = false;
			}
		}

		public void StartDownloadingNewGameConfiguration()
		{
			m_DownloadSuccess = false;
			m_RequestSuccess = false;
			if (m_AmInitialised)
			{
				StartCoroutine(DownloadNewGameConfig());
			}
			else
			{
				GameConfigLogger("Network NOT initialized, can't download anything...");
			}
		}

		private IEnumerator DownloadNewGameConfig()
		{
			SingletonInstance<GameConfiguration>.Instance.ClearDownloadedConfig();
			if (SetupNetworking.IsNetworkingEnabled)
			{
				m_CountryCode = Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode;
				m_RequestSuccess = true;
				m_AmDownloading = true;
				m_Thread = null;
				m_Thread = new Thread(DownloadConfigThread);
				m_Thread.Start();
				GameConfigLogger("Starting download thread...");
				yield return new WaitForEndOfFrame();
				while (!FinishedEvent.WaitOne(0))
				{
					yield return null;
				}
				m_AmDownloading = false;
				m_ServerResponse = JSONSerialiser.Parse<GameConfigBlob>(m_DecodedData);
				if (m_ServerResponse != null)
				{
					GameConfigLogger("Got a response, assimilating.");
					AssimilateGameConfiguration(m_ServerResponse);
				}
				else
				{
					GameConfigLogger("Did not receive response.");
				}
				GameConfigLogger("Download thread complete.");
				m_Thread = null;
			}
			else
			{
				m_RequestSuccess = false;
				m_AmDownloading = false;
				GameConfigLogger("Can't download config, no networking!");
			}
		}

		private void DownloadConfigThread()
		{
			m_DownloadSuccess = false;
			try
			{
				m_DownloadSuccess = DownloadGameConfig_Ex();
			}
			catch (Exception ex)
			{
				m_DownloadSuccess = false;
				GameConfigLogger("DownloadConfigThread: Failed to download config. Exception: " + ex.ToString());
			}
			finally
			{
				FinishedEvent.Set();
			}
		}

		private bool DownloadGameConfig_Ex()
		{
			StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_DownloadServer);
			if (serverDetails == null)
			{
				return false;
			}
			bool flag = true;
			Stopwatch stopwatch = Stopwatch.StartNew();
			GameConfigurationRequestBuilder gameConfigurationRequestBuilder = new GameConfigurationRequestBuilder();
			gameConfigurationRequestBuilder.StaticRequestDetails = serverDetails;
			gameConfigurationRequestBuilder.OS = m_AppProperties.m_ServerPlatform;
			gameConfigurationRequestBuilder.OSVer = m_AppProperties.m_OSVersion;
			GameConfigurationRequestBuilder gameConfigurationRequestBuilder2 = gameConfigurationRequestBuilder;
			string url = gameConfigurationRequestBuilder2.GetUrl(m_TargetGameConfig);
			RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
			requestDetails.ContentType = ContentType.JSON;
			requestDetails.SendClientCertificate = false;
			RESTfulApi.RequestDetails requestDetails2 = requestDetails;
			if (serverDetails.Protocol == Protocol.https)
			{
				requestDetails2.ValidateServerCertificate = ValidateServerCertificate.Check;
				requestDetails2.SslProtocol = serverDetails.ServerProtocol;
			}
			else
			{
				requestDetails2.ValidateServerCertificate = ValidateServerCertificate.None;
			}
			MessageRequestContent messageRequestContent = new MessageRequestContent();
			messageRequestContent.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
			messageRequestContent.SessionId = TelemetryManager.SessionId;
			messageRequestContent.DeviceId = DeviceIdManager.DeviceId;
			messageRequestContent.ProfileFriendlyName = m_AppProperties.m_ProfileFriendlyName;
			messageRequestContent.ProfileType = m_AppProperties.m_ProfileType;
			messageRequestContent.Resolution = m_AppProperties.m_Resolution;
			messageRequestContent.SysInfo = m_AppProperties.m_SysInfo;
			messageRequestContent.VersionNumber = m_AppProperties.m_GameVersion;
			messageRequestContent.Language = m_AppProperties.m_Language;
			messageRequestContent.LanguageCode = m_AppProperties.m_LanguageCode;
			messageRequestContent.CountryCode = m_CountryCode;
			MessageRequestContent messageRequestContent2 = messageRequestContent;
			GameConfigurationResponse item = RESTfulApi.GetItem<GameConfigurationResponse, MessageRequestContent>(HttpVerb.POST, url, messageRequestContent2, requestDetails2);
			if (item == null)
			{
				GameConfigLogger("DownloadGameConfig_Ex: Did not receive a response from server.");
				flag = false;
			}
			else if (string.IsNullOrEmpty(item.RequestId))
			{
				GameConfigLogger("DownloadGameConfig_Ex: Did not receive a RequestId back from server. Not trusting this response.");
				flag = false;
			}
			else if (string.Compare(item.RequestId, messageRequestContent2.RequestId) != 0)
			{
				GameConfigLogger("DownloadGameConfig_Ex: Received a different RequestId back from server than the one we sent. Not trusting this response.");
				flag = false;
			}
			stopwatch.Stop();
			if (flag)
			{
				m_DownloadTime = new TimeSpan(stopwatch.ElapsedTicks);
				GameConfigLogger("DownloadGameConfig_Ex: Completed download in " + stopwatch.Elapsed.ToString());
				GameConfigLogger("DownloadGameConfig_Ex: Completed download in " + m_DownloadTime.ToString());
				m_DecodedData = Encoding.UTF8.GetString(Convert.FromBase64String(item.Data));
			}
			return flag;
		}

		private static string ToReadableString(TimeSpan span)
		{
			string text = string.Format("{0}{1}{2}{3}", (span.Duration().Days <= 0) ? string.Empty : string.Format("{0:0} day{1}, ", span.Days, (span.Days != 1) ? "s" : string.Empty), (span.Duration().Hours <= 0) ? string.Empty : string.Format("{0:0} hour{1}, ", span.Hours, (span.Hours != 1) ? "s" : string.Empty), (span.Duration().Minutes <= 0) ? string.Empty : string.Format("{0:0} minute{1}, ", span.Minutes, (span.Minutes != 1) ? "s" : string.Empty), (span.Duration().Seconds <= 0) ? string.Empty : string.Format("{0:0} second{1}", span.Seconds, (span.Seconds != 1) ? "s" : string.Empty));
			if (text.EndsWith(", "))
			{
				text = text.Substring(0, text.Length - 2);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "0 seconds";
			}
			return text;
		}

		private void AssimilateGameConfiguration(GameConfigBlob gcData)
		{
			lock (m_Lock)
			{
				bool flag = string.IsNullOrEmpty(gcData.m_IncubatorTimings.Validate());
				GameConfigLogger("GameConfig is " + ((!flag) ? "NOT " : string.Empty) + "valid");
				if (flag)
				{
					GameConfigLogger("GameConfig APPLIED!");
					SingletonInstance<GameConfiguration>.Instance.SetDownloadedGameConfig(gcData);
					m_DownloadedDataWasApplied = true;
					GameConfigLogger("GameConfig stored in save data!");
					Singleton<GameDataStoreObject>.Instance.Data.m_LastGameConfig = gcData;
					Singleton<GameDataStoreObject>.Instance.Data.m_HaveStoredADownloadedGameConfig = true;
					Singleton<GameDataStoreObject>.Instance.Save();
				}
				else
				{
					m_DownloadedDataWasApplied = false;
					SingletonInstance<GameConfiguration>.Instance.ClearDownloadedConfig();
				}
			}
		}

		private string GetPlatformAppropriateGameConfig()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
			{
				if (string.IsNullOrEmpty(SystemInfo.deviceModel))
				{
					DebugUtils.Log_InMagenta("DownloadGameConfig_Ex::GetPlatformAppropriateGameConfig() AndroidPlayer, unknown");
					return m_DefaultGameConfigs.m_AndroidGooglePlay;
				}
				string text = SystemInfo.deviceModel.ToLower();
				if (text.Contains("kindle") || text.Contains("KFTT") || text.Contains("amazon"))
				{
					DebugUtils.Log_InMagenta("DownloadGameConfig_Ex::GetPlatformAppropriateGameConfig() AndroidPlayer, Kindle");
					return m_DefaultGameConfigs.m_AndroidAmazon;
				}
				DebugUtils.Log_InMagenta("DownloadGameConfig_Ex::GetPlatformAppropriateGameConfig() AndroidPlayer, GooglePlay");
				return m_DefaultGameConfigs.m_AndroidGooglePlay;
			}
			case RuntimePlatform.IPhonePlayer:
				DebugUtils.Log_InMagenta("DownloadGameConfig_Ex::GetPlatformAppropriateGameConfig() iPhonePlayer");
				return m_DefaultGameConfigs.m_iOSAppStore;
			default:
				DebugUtils.Log_InMagenta("DownloadGameConfig_Ex::GetPlatformAppropriateGameConfig() Some other device...");
				return m_DefaultGameConfigs.m_Fallback;
			}
		}
	}
}
