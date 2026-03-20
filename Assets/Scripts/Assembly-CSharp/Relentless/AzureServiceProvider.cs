using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Relentless.Network;
using Relentless.Network.Analytics;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public abstract class AzureServiceProvider<T> : CloudProvider<T> where T : class, new()
	{
		[SerializeField]
		private Servers m_server = Servers.None;

		private string m_osVer;

		private Platforms m_serverPlatform;

		private ProfileFriendlyName m_currentFriendlyProfileName;

		private ProfileType m_currentProfileType;

		private Resolution m_currentResolution;

		private string m_gameVersion;

		private string m_sysInfo;

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

		protected virtual Resolution CurrentResolution
		{
			get
			{
				return m_currentResolution;
			}
		}

		protected virtual ProfileType CurrentProfileType
		{
			get
			{
				return m_currentProfileType;
			}
		}

		protected virtual ProfileFriendlyName CurrentProfileFriendlyName
		{
			get
			{
				return m_currentFriendlyProfileName;
			}
		}

		protected virtual string SysInfo
		{
			get
			{
				return m_sysInfo;
			}
		}

		protected virtual string GameVersion
		{
			get
			{
				return m_gameVersion;
			}
		}

		protected override string ProviderName
		{
			get
			{
				return "AzureServiceProvider";
			}
		}

		public override void Initialise()
		{
			base.Initialise();
			m_osVer = NetworkHelper.OSVer;
			m_serverPlatform = NetworkHelper.ServerPlatform;
			m_currentFriendlyProfileName = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName;
			m_currentProfileType = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType;
			m_currentResolution = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution;
			m_gameVersion = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleVersion;
			m_sysInfo = NetworkHelper.GetSysInfo();
		}

		public override bool DownloadBlobData()
		{
			try
			{
				StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_server);
				if (serverDetails == null)
				{
					return false;
				}
				GameDataServerRequestBuilder gameDataServerRequestBuilder = new GameDataServerRequestBuilder();
				gameDataServerRequestBuilder.StaticRequestDetails = serverDetails;
				gameDataServerRequestBuilder.OS = ServerPlatform;
				gameDataServerRequestBuilder.OSVer = OSVer;
				GameDataServerRequestBuilder arg = gameDataServerRequestBuilder;
				string requestUri = string.Format("{0}/{1}/{2}", arg, ContainerName, Filename);
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
				GameDataServerRequestContent gameDataServerRequestContent = new GameDataServerRequestContent();
				gameDataServerRequestContent.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
				gameDataServerRequestContent.SessionId = TelemetryManager.SessionId;
				gameDataServerRequestContent.DeviceId = DeviceIdManager.DeviceId;
				gameDataServerRequestContent.ProfileFriendlyName = CurrentProfileFriendlyName;
				gameDataServerRequestContent.ProfileType = CurrentProfileType;
				gameDataServerRequestContent.Resolution = CurrentResolution;
				gameDataServerRequestContent.SysInfo = SysInfo;
				gameDataServerRequestContent.VersionNumber = GameVersion;
				GameDataServerRequestContent gameDataServerRequestContent2 = gameDataServerRequestContent;
				GameDataServerResponse item = RESTfulApi.GetItem<GameDataServerResponse, GameDataServerRequestContent>(HttpVerb.POST, requestUri, gameDataServerRequestContent2, requestDetails2);
				if (item == null)
				{
					Logging.LogError("ServerGameData: Did not receive a response from server.");
					UseDefaultData();
					return false;
				}
				base.BlobData = (T)null;
				if (string.IsNullOrEmpty(item.RequestId))
				{
					Logging.LogError("ServerGameData: Did not receive a RequestId back from server. Not trusting this response.");
				}
				else if (string.Compare(item.RequestId, gameDataServerRequestContent2.RequestId) != 0)
				{
					Logging.LogError("ServerGameData: Received a different RequestId back from server than the one we sent. Not trusting this response.");
				}
				else if (!string.IsNullOrEmpty(item.Data))
				{
					string data = item.Data;
					string text = Encoding.UTF8.GetString(Convert.FromBase64String(data));
					Logging.Log("ServerData received: " + text);
					base.BlobData = ParseData(text);
				}
				else if (!string.IsNullOrEmpty(item.FileUrl))
				{
					string serverTimeFormattedForHttpHeader = SetupNetworking.ServerTimeFormattedForHttpHeader;
					if (!string.IsNullOrEmpty(item.FileUrlBase64))
					{
						string strA = Encoding.UTF8.GetString(Convert.FromBase64String(item.FileUrlBase64));
						if (string.Compare(strA, item.FileUrl) != 0)
						{
							Logging.LogError("Decoded url and original url are different.");
							DebugNotifications.AddNotification("Decoded url and original url are different", TimeSpan.FromSeconds(15.0));
						}
					}
					Uri uri = new Uri(item.FileUrl);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("date", serverTimeFormattedForHttpHeader);
					dictionary.Add("content-type", "application/json");
					dictionary.Add("accept-encoding", "gzip");
					dictionary.Add("host", uri.Host);
					Dictionary<string, string> headers = dictionary;
					requestDetails = new RESTfulApi.RequestDetails();
					requestDetails.Headers = headers;
					requestDetails.SendClientCertificate = false;
					requestDetails.ValidateServerCertificate = ValidateServerCertificate.Ignore;
					requestDetails.SslProtocol = SslProtocols.Ssl3;
					requestDetails.ContentType = ContentType.JSON;
					RESTfulApi.RequestDetails requestDetails3 = requestDetails;
					base.BlobData = RESTfulApi.GetItem<T>(item.FileUrl, requestDetails3);
				}
				else
				{
					Logging.LogError("Received invalid data back from server. Not trusting this response.");
				}
				if (base.BlobData == null)
				{
					UseDefaultData();
					return false;
				}
				base.AbTests = item.AbTests;
				return true;
			}
			catch (Exception ex)
			{
				Logging.LogError("DownloadBlobData caught exception " + ex);
			}
			UseDefaultData();
			return false;
		}
	}
}
