using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Relentless.Network;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public class SetupNetworking : SingletonInstance<SetupNetworking>
	{
		public enum NetworkStatusHeartbeatPolicy
		{
			PollTimeServer = 0,
			UseNativeAPIs = 1,
			PingSomething = 2
		}

		private class GetServerTimeThreadParams
		{
			public StaticRequestDetails ServerDetails { get; set; }

			public GetTimeResponse GetTimeResponse { get; set; }
		}

		private class GetTimeResponse
		{
			public DateTime UtcNow = DateTime.UtcNow;
		}

		public delegate void ConnectionChange();

		private static bool m_isReady = false;

		private static bool m_isNetworkAvailable = false;

		private static bool m_retry = false;

		private static string m_countryCode_Truncated = string.Empty;

		private static string m_countryCode_Full = string.Empty;

		private static readonly object m_lock = new object();

		private static TimeSpan m_serverTimeOffSetUtc = TimeSpan.FromMinutes(0.0);

		[SerializeField]
		private StaticRequestDetails[] m_serverData;

		private static Dictionary<Servers, StaticRequestDetails> m_serverDetails = new Dictionary<Servers, StaticRequestDetails>();

		[SerializeField]
		private Servers m_timeServer = Servers.None;

		[SerializeField]
		private float m_upConnectionTestTime = 10f;

		[SerializeField]
		private float m_downConnectionTestTime = 10f;

		public static string CountryCode
		{
			get
			{
				lock (m_lock)
				{
					return m_countryCode_Truncated;
				}
			}
		}

		public static string CountryCode_Raw
		{
			get
			{
				lock (m_lock)
				{
					return m_countryCode_Full;
				}
			}
		}

		public static bool IsReady
		{
			get
			{
				lock (m_lock)
				{
					return m_isReady;
				}
			}
			private set
			{
				lock (m_lock)
				{
					m_isReady = value;
				}
			}
		}

		public static bool IsNetworkReady
		{
			get
			{
				lock (m_lock)
				{
					return m_isNetworkAvailable;
				}
			}
			private set
			{
				lock (m_lock)
				{
					m_isNetworkAvailable = value;
				}
			}
		}

		public static bool Retry
		{
			get
			{
				lock (m_lock)
				{
					return m_retry;
				}
			}
			set
			{
				lock (m_lock)
				{
					m_retry = value;
				}
			}
		}

		public static DateTime ServerTimeUtcNow
		{
			get
			{
				lock (m_lock)
				{
					return DateTime.UtcNow - m_serverTimeOffSetUtc;
				}
			}
		}

		public static string ServerTimeFormattedForHttpHeader
		{
			get
			{
				return ServerTimeUtcNow.ToString("R", CultureInfo.InvariantCulture);
			}
		}

		public static bool IsNetworkingEnabled
		{
			get
			{
				return true;
			}
		}

		public static bool IsInternetReady
		{
			get
			{
				return Application.internetReachability != NetworkReachability.NotReachable && IsNetworkReady;
			}
		}

		public event ConnectionChange OnConnectionEstablished;

		public event ConnectionChange OnConnectionLost;

		public static StaticRequestDetails GetServerDetails(Servers server)
		{
			if (server == Servers.None)
			{
				return null;
			}
			lock (m_lock)
			{
				if (m_serverDetails.Count == 0)
				{
					Logging.LogError("SetupNetworking: Failed to find any servers (none saved)");
					return null;
				}
				if (!m_serverDetails.ContainsKey(server))
				{
					Logging.LogError("SetupNetworking: Failed to find server " + server);
					return null;
				}
				return m_serverDetails[server];
			}
		}

		public override void Awake()
		{
			base.Awake();
			lock (m_lock)
			{
				StaticRequestDetails[] serverData = m_serverData;
				foreach (StaticRequestDetails staticRequestDetails in serverData)
				{
					m_serverDetails.Add(staticRequestDetails.Server, staticRequestDetails);
				}
			}
			if (IsNetworkingEnabled)
			{
				StartCoroutine(NetworkStatusHeartbeat(NetworkStatusHeartbeatPolicy.PollTimeServer));
				return;
			}
			Logging.LogWarning("SetupNetworking: Skipping getting time from server as networking is not enabled.", this);
			IsReady = true;
		}

		private IEnumerator NetworkStatusHeartbeat(NetworkStatusHeartbeatPolicy policy)
		{
			Logging.Log("SetupNetworking: Waiting for certificates and thumbprints to be registered ...", this);
			yield return StartCoroutine(WaitForNetwork());
			yield return StartCoroutine(GetCountryCode());
			StaticRequestDetails serverDetails = GetServerDetails(m_timeServer);
			if (serverDetails == null || string.IsNullOrEmpty(serverDetails.ServerName))
			{
				yield break;
			}
			while (true)
			{
				if (policy == NetworkStatusHeartbeatPolicy.PollTimeServer)
				{
					yield return StartCoroutine(UpdateStatus_ViaTimeServer());
				}
				if (!IsReady)
				{
					Logging.Log("SetupNetworking: Ready");
					IsReady = true;
				}
				yield return StartCoroutine(WaitForInterval());
			}
		}

		private IEnumerator WaitForInterval()
		{
			float waitTime = m_upConnectionTestTime;
			if (!IsNetworkReady)
			{
				waitTime = m_downConnectionTestTime;
			}
			float startTime = Time.realtimeSinceStartup;
			float duration = 0f;
			while (duration < waitTime)
			{
				yield return new WaitForEndOfFrame();
				duration = Time.realtimeSinceStartup - startTime;
				if (Retry)
				{
					break;
				}
			}
			Retry = false;
		}

		private IEnumerator UpdateStatus_ViaTimeServer()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				IsNetworkReady = false;
				yield break;
			}
			Logging.Log("SetupNetworking: Getting time from server");
			StaticRequestDetails serverDetails = GetServerDetails(m_timeServer);
			GetServerTimeThreadParams getTimeResponse = new GetServerTimeThreadParams
			{
				ServerDetails = serverDetails
			};
			Thread serverTimeThread = new Thread(GetServerTimeThread);
			serverTimeThread.Start(getTimeResponse);
			yield return new WaitForEndOfFrame();
			while (serverTimeThread.IsAlive)
			{
				yield return new WaitForEndOfFrame();
			}
			if (getTimeResponse.GetTimeResponse != null)
			{
				m_serverTimeOffSetUtc = DateTime.UtcNow - getTimeResponse.GetTimeResponse.UtcNow;
				if (!IsNetworkReady)
				{
					Logging.Log("SetupNetworking: OnConnectionEstablished");
					ConnectionChange onConnectionEstablished = this.OnConnectionEstablished;
					if (onConnectionEstablished != null)
					{
						onConnectionEstablished();
					}
				}
				IsNetworkReady = true;
				Logging.LogWarning(string.Format("SetupNetworking: Server Time: {0}, offset from local is {1}", getTimeResponse.GetTimeResponse.UtcNow, m_serverTimeOffSetUtc));
				if (!IsReady)
				{
					if (Math.Abs(m_serverTimeOffSetUtc.TotalMinutes) > 15.0)
					{
						string message = string.Format("Your device time is out by {0} minutes.", m_serverTimeOffSetUtc.TotalMinutes);
						DebugNotifications.AddNotification(message, TimeSpan.FromSeconds(10.0));
					}
					else
					{
						DebugNotifications.AddNotification("Device time is within a 15 minute window of Server time.", TimeSpan.FromSeconds(10.0));
					}
				}
				yield break;
			}
			if (IsNetworkReady)
			{
				Logging.Log("SetupNetworking: onConnectionLost");
				ConnectionChange onConnectionLost = this.OnConnectionLost;
				if (onConnectionLost != null)
				{
					onConnectionLost();
				}
			}
			else
			{
				Logging.Log("SetupNetworking: IsNetworkReady = false and getTimeResponse.GetTimeResponse == null");
			}
			IsNetworkReady = false;
		}

		private IEnumerator WaitForNetwork()
		{
			do
			{
				yield return new WaitForEndOfFrame();
			}
			while (SingletonInstance<SetupNetworkingFromEditor>.Instance != null && !SingletonInstance<SetupNetworkingFromEditor>.Instance.IsReady);
		}

		private IEnumerator GetCountryCode()
		{
			using (WWW www = new WWW("http://furbymobilemsg.hosting-hasbro.com"))
			{
				while (!www.isDone)
				{
					yield return null;
				}
				if (!string.IsNullOrEmpty(www.error))
				{
					Logging.Log("SetupNetworking: Failed to read CountryCode : " + www.error);
					yield break;
				}
				m_countryCode_Full = www.text.Trim();
				if (m_countryCode_Full.Length >= 2)
				{
					if (m_countryCode_Full.Length >= 5)
					{
						m_countryCode_Full = m_countryCode_Full.Truncate(5);
					}
					m_countryCode_Truncated = m_countryCode_Full.Truncate(2);
				}
				Logging.Log("SetupNetworking: CountryCode is (Full)" + m_countryCode_Full);
				Logging.Log("SetupNetworking: CountryCode is (Truncated)" + m_countryCode_Truncated);
				Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode = m_countryCode_Truncated;
				GameEventRouter.SendEvent(GeoCodeEvents.GeoCodeDownloaded, null, m_countryCode_Truncated, m_countryCode_Full);
			}
		}

		private void GetServerTimeThread(object parameters)
		{
			try
			{
				GetServerTimeThreadParams getServerTimeThreadParams = (GetServerTimeThreadParams)parameters;
				RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
				requestDetails.SendClientCertificate = false;
				requestDetails.ValidateServerCertificate = ValidateServerCertificate.Check;
				requestDetails.SslProtocol = getServerTimeThreadParams.ServerDetails.ServerProtocol;
				requestDetails.ContentType = ContentType.JSON;
				RESTfulApi.RequestDetails requestDetails2 = requestDetails;
				string requestUri = string.Format("{0}://{1}/api/{2}/time", getServerTimeThreadParams.ServerDetails.Protocol, getServerTimeThreadParams.ServerDetails.ServerName, getServerTimeThreadParams.ServerDetails.ApiVersion);
				getServerTimeThreadParams.GetTimeResponse = RESTfulApi.PostItem<string, GetTimeResponse>(requestUri, "UtfNow", requestDetails2);
			}
			catch (Exception ex)
			{
				Logging.LogError("SetupNetworking: Failed to get server time: " + ex.ToString());
			}
		}
	}
}
