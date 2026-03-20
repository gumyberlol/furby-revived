using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Relentless;
using Relentless.Network;
using Relentless.Network.Analytics;
using Relentless.Network.Security;
using UnityEngine;

namespace Furby.Scripts.FurMail
{
	public class FurMailManager : SingletonInstance<FurMailManager>
	{
		public delegate void MessageListUpdatedHandler();

		public delegate void ProcessMessageHandler(FurMailMessage message);

		private object m_lock = new object();

		private Thread m_downloadMessagesThread;

		private bool m_downloadSucceeded;

		private string m_osVer;

		private Relentless.Platforms m_serverPlatform;

		private ProfileFriendlyName m_friendlyProfileName;

		private ProfileType m_profileType;

		private Relentless.Resolution m_resolution;

		private string m_gameVersion;

		private string m_sysInfo;

		private SystemLanguage m_language;

		private string m_languageCode;

		private TargetGroupInformation m_targetGroupInformation;

		private FurMailPersistedStore[] m_store = new FurMailPersistedStore[3];

		private int m_currentStoreIndex;

		[SerializeField]
		private Servers m_server = Servers.None;

		private bool m_MessageListInitialised;

		protected virtual SystemLanguage Language
		{
			get
			{
				return m_language;
			}
		}

		protected virtual string LanguageCode
		{
			get
			{
				return m_languageCode;
			}
		}

		protected virtual string OSVer
		{
			get
			{
				return m_osVer;
			}
		}

		protected virtual Relentless.Platforms ServerPlatform
		{
			get
			{
				return m_serverPlatform;
			}
		}

		protected virtual Relentless.Resolution Resolution
		{
			get
			{
				return m_resolution;
			}
		}

		protected virtual ProfileType ProfileType
		{
			get
			{
				return m_profileType;
			}
		}

		protected virtual ProfileFriendlyName ProfileFriendlyName
		{
			get
			{
				return m_friendlyProfileName;
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

		protected TargetGroupInformation TargetGroupInformation
		{
			get
			{
				return m_targetGroupInformation;
			}
		}

		public int MessageCount
		{
			get
			{
				lock (m_lock)
				{
					return GetFurMailData().m_messages.Count;
				}
			}
		}

		public int NewMessageCount
		{
			get
			{
				lock (m_lock)
				{
					int num = 0;
					foreach (FurMailMessage message in GetFurMailData().m_messages)
					{
						if (!message.IsRead)
						{
							num++;
						}
					}
					return num;
				}
			}
		}

		public event MessageListUpdatedHandler MessageListUpdated;

		private void FireMessageListUpdated()
		{
			MessageListUpdatedHandler messageListUpdated = this.MessageListUpdated;
			if (messageListUpdated != null)
			{
				messageListUpdated();
			}
		}

		public override void Awake()
		{
			base.Awake();
			for (int i = 0; i < 3; i++)
			{
				m_store[i] = new FurMailPersistedStore(i);
				m_store[i].Load();
			}
			StartCoroutine(InitMessageListCoroutine());
		}

		private FurMailData GetFurMailData()
		{
			if (m_store[m_currentStoreIndex].Data.m_messages == null)
			{
				m_store[m_currentStoreIndex].Data.m_messages = new List<FurMailMessage>();
			}
			return m_store[m_currentStoreIndex].Data;
		}

		private FurMailPersistedStore GetFurMailDataStore()
		{
			return m_store[m_currentStoreIndex];
		}

		public void IterateMessages(ProcessMessageHandler processMessage)
		{
			if (processMessage == null)
			{
				Logging.LogError("processMessage == null");
				return;
			}
			lock (m_lock)
			{
				GetFurMailData().m_messages.Sort(new FurMailMessageSorter());
				foreach (FurMailMessage message in GetFurMailData().m_messages)
				{
					processMessage(message);
				}
			}
		}

		public void Save()
		{
			lock (m_lock)
			{
				GetFurMailDataStore().Save();
			}
		}

		public void StartCheckingForNewMessages()
		{
			StartCoroutine(DownloadMessagesCoroutine());
		}

		private IEnumerator InitMessageListCoroutine()
		{
			m_MessageListInitialised = false;
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			SetupNetworking.GetServerDetails(Servers.Kepler);
			m_osVer = NetworkHelper.OSVer;
			m_serverPlatform = NetworkHelper.ServerPlatform;
			m_friendlyProfileName = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName;
			m_profileType = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType;
			m_resolution = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution;
			m_gameVersion = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleVersion;
			m_sysInfo = NetworkHelper.GetSysInfo();
			m_MessageListInitialised = true;
		}

		public void OnSaveSlotChanged(int currentSaveSlot)
		{
			m_currentStoreIndex = currentSaveSlot;
		}

		public void OnDashboardStart()
		{
			StartCoroutine(UpdateTargetInformationAndCheckForMessages());
		}

		public void OnSaveSlotDeleted(int saveSlotToDelete)
		{
			m_store[saveSlotToDelete].Clear();
		}

		private IEnumerator UpdateTargetInformationAndCheckForMessages()
		{
			while (!m_MessageListInitialised)
			{
				yield return new WaitForEndOfFrame();
			}
			m_language = NetworkHelper.GetSystemLanguage(Singleton<Localisation>.Instance.CurrentLocale);
			m_languageCode = NetworkHelper.GetLanguageCode_FromLocale(Singleton<Localisation>.Instance.CurrentLocale);
			m_targetGroupInformation = GetTargetGroupInformation();
			Logging.Log("FurMail: TargetGroupInfo: " + m_targetGroupInformation);
			StartCheckingForNewMessages();
		}

		private IEnumerator DownloadMessagesCoroutine()
		{
			Logging.Log("FurMail: DownloadMessages");
			if (SetupNetworking.IsNetworkingEnabled)
			{
				while (m_downloadMessagesThread != null)
				{
					yield return new WaitForEndOfFrame();
				}
				m_downloadMessagesThread = new Thread(DownloadMessagesThread);
				m_downloadMessagesThread.Start();
				yield return new WaitForEndOfFrame();
				while (m_downloadMessagesThread.IsAlive)
				{
					yield return new WaitForEndOfFrame();
				}
				m_downloadMessagesThread = null;
			}
			else
			{
				Logging.LogWarning("Skipping download of FurMail from cloud since networking is disabled");
			}
			if (!m_downloadSucceeded)
			{
				Logging.LogError("Failed to download FurMail from cloud.");
				DebugNotifications.AddNotification("Failed to download FurMail from the cloud.", TimeSpan.FromSeconds(15.0));
			}
			else
			{
				StringBuilder notification = new StringBuilder();
				notification.AppendFormat("Downloaded {0} FurMail messages from the cloud.\n\n", GetFurMailData().m_messages.Count);
				foreach (FurMailMessage msg in GetFurMailData().m_messages)
				{
					notification.AppendFormat("subject={0}\n", msg.MessageSubject);
				}
				DebugNotifications.AddNotification(notification.ToString(), TimeSpan.FromSeconds(15.0));
				GetFurMailDataStore().Save();
				FireMessageListUpdated();
			}
			Logging.Log("FurMail downloaded is ready.\r\n");
			yield return null;
		}

		private void DownloadMessagesThread()
		{
			try
			{
				m_downloadSucceeded = DownloadMessages();
			}
			catch (Exception ex)
			{
				m_downloadSucceeded = false;
				Logging.LogError("Failed to download FurMail : " + ex.ToString());
			}
		}

		private bool DownloadMessages()
		{
			StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_server);
			if (serverDetails == null)
			{
				return false;
			}
			FurMailRequestBuilder furMailRequestBuilder = new FurMailRequestBuilder();
			furMailRequestBuilder.StaticRequestDetails = serverDetails;
			furMailRequestBuilder.OS = ServerPlatform;
			furMailRequestBuilder.OSVer = OSVer;
			FurMailRequestBuilder furMailRequestBuilder2 = furMailRequestBuilder;
			string requestUri = furMailRequestBuilder2.ToString();
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
			GetMessagesRequestContent getMessagesRequestContent = new GetMessagesRequestContent();
			getMessagesRequestContent.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
			getMessagesRequestContent.SessionId = TelemetryManager.SessionId;
			getMessagesRequestContent.DeviceId = DeviceIdManager.DeviceId;
			getMessagesRequestContent.ProfileFriendlyName = ProfileFriendlyName;
			getMessagesRequestContent.ProfileType = ProfileType;
			getMessagesRequestContent.Resolution = Resolution;
			getMessagesRequestContent.SysInfo = SysInfo;
			getMessagesRequestContent.VersionNumber = GameVersion;
			getMessagesRequestContent.TargetGroupInformation = TargetGroupInformation;
			getMessagesRequestContent.Language = Language;
			getMessagesRequestContent.LanguageCode = LanguageCode;
			GetMessagesRequestContent getMessagesRequestContent2 = getMessagesRequestContent;
			GetMessagesServerResponse item = RESTfulApi.GetItem<GetMessagesServerResponse, GetMessagesRequestContent>(HttpVerb.POST, requestUri, getMessagesRequestContent2, requestDetails2);
			if (item == null)
			{
				Logging.LogError("FurMail: Did not receive a response from server.");
				return false;
			}
			if (string.IsNullOrEmpty(item.RequestId))
			{
				Logging.LogError("FurMail: Did not receive a RequestId back from server. Not trusting this response.");
				return false;
			}
			if (string.Compare(item.RequestId, getMessagesRequestContent2.RequestId) != 0)
			{
				Logging.LogError("FurMail: Received a different RequestId back from server than the one we sent. Not trusting this response.");
				return false;
			}
			SynchroniseMessageList(item.Messages);
			Logging.Log("FurMail : Got " + GetFurMailData().m_messages.Count + " FurMail messages");
			return true;
		}

		private void SynchroniseMessageList(List<FurMailServerMessage> serverMessages)
		{
			lock (m_lock)
			{
				foreach (FurMailServerMessage serverMessage in serverMessages)
				{
					FurMailMessage furMailMessage = FindMessage(serverMessage.MessageId);
					if (furMailMessage == null)
					{
						FurMailMessage item = new FurMailMessage(serverMessage);
						GetFurMailData().m_messages.Add(item);
					}
					else
					{
						furMailMessage.Update(serverMessage);
					}
				}
				List<FurMailMessage> list = new List<FurMailMessage>();
				FurMailMessage existingMessage;
				foreach (FurMailMessage message in GetFurMailData().m_messages)
				{
					existingMessage = message;
					if (serverMessages.FirstOrDefault((FurMailServerMessage serverMsg) => serverMsg.MessageId == existingMessage.MessageId) == null)
					{
						list.Add(existingMessage);
					}
				}
				foreach (FurMailMessage item2 in list)
				{
					GetFurMailData().m_messages.Remove(item2);
				}
			}
		}

		private TargetGroupInformation GetTargetGroupInformation()
		{
			bool isNonFurbyUser = true;
			if (FurbyGlobals.Player != null)
			{
				isNonFurbyUser = FurbyGlobals.Player.NoFurbyOnSaveGame();
			}
			bool isFurbyUser = false;
			if (FurbyGlobals.AdultLibrary != null && FurbyGlobals.AdultLibrary.Furbies != null)
			{
				isFurbyUser = FurbyGlobals.AdultLibrary.Furbies.Count > 0;
			}
			bool hasGoldenBaby = false;
			List<FurbyTribeType> list = new List<FurbyTribeType>();
			int num = 0;
			if (FurbyGlobals.BabyRepositoryHelpers != null)
			{
				foreach (FurbyBaby item in FurbyGlobals.BabyRepositoryHelpers.GetBabiesOfProgress(FurbyBabyProgresss.N))
				{
					if (!list.Contains(item.Tribe) && FurbyGlobals.BabyRepositoryHelpers.HasCompletedTribe(item.Tribe))
					{
						list.Add(item.Tribe);
					}
					if (item.Tribe.Name == "Gold")
					{
						hasGoldenBaby = true;
					}
					num++;
				}
			}
			TargetGroupInformation targetGroupInformation = new TargetGroupInformation();
			targetGroupInformation.Language = LanguageCode;
			targetGroupInformation.NumberOfUniqueBabies = num;
			targetGroupInformation.NumberOfTribesCompleted = list.Count;
			targetGroupInformation.HasGoldenBaby = hasGoldenBaby;
			targetGroupInformation.IsFurbyUser = isFurbyUser;
			targetGroupInformation.IsNonFurbyUser = isNonFurbyUser;
			return targetGroupInformation;
		}

		private FurMailMessage FindMessage(Guid messageId)
		{
			lock (m_lock)
			{
				return GetFurMailData().m_messages.FirstOrDefault((FurMailMessage message) => message.MessageId == messageId);
			}
		}
	}
}
