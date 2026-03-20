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
	public class BannerAdController : SingletonInstance<BannerAdController>
	{
		public UITexture m_TargetTexture;

		public BannerHandler m_BannerHandler;

		public BoxCollider m_BoxCollider;

		public float m_TimeSecondsToWait;

		public float m_FadeUpDuration;

		public int m_SessionInterval = 5;

		private Texture2D m_Texture2D;

		public GameObject m_AdvertDeclaration_SD;

		public GameObject m_AdvertDeclaration_HD;

		[SerializeField]
		private string m_CountryCode;

		[SerializeField]
		public ManualResetEvent m_ThreadEvent = new ManualResetEvent(false);

		[SerializeField]
		[HideInInspector]
		private bool m_RequestSuccess;

		[SerializeField]
		[HideInInspector]
		private bool m_AmDownloading;

		[SerializeField]
		[HideInInspector]
		private Thread m_Thread;

		[SerializeField]
		[HideInInspector]
		private string m_RawImageData = string.Empty;

		[SerializeField]
		[HideInInspector]
		private bool m_DownloadSuccess;

		[HideInInspector]
		[SerializeField]
		private bool m_AmInitialised;

		[SerializeField]
		public ApplicationProperties m_AppProperties = new ApplicationProperties();

		[SerializeField]
		public Servers m_DownloadServer = Servers.AzureLive;

		[HideInInspector]
		public DeviceProperties m_DeviceProperties;

		[HideInInspector]
		public BannerAdvert m_BannerAdvert;

		public override void Awake()
		{
			base.Awake();
			StartCoroutine(InitialiseSelf());
		}

		private IEnumerator InitialiseSelf()
		{
			BannerAdLogger("InitialiseSelf() START");
			SetupNetworking.GetServerDetails(Servers.Kepler);
			yield return StartCoroutine(WaitForNetwork());
			yield return StartCoroutine(WaitForGameConfigResolution());
			yield return new WaitForEndOfFrame();
			GameConfigBlob gameConfig = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob();
			if (!gameConfig.SuppressAllBanners && gameConfig.DoesGeoCodeAllowBannerAdverts())
			{
				yield return StartCoroutine(DownloadAndShowAdvert(BannerAdLogic.FollowGameLogic));
			}
		}

		public void ManuallyInvokeBannerAd(BannerAdLogic bannerAdLogic)
		{
			StartCoroutine(DownloadAndShowAdvert(bannerAdLogic));
		}

		private IEnumerator DownloadAndShowAdvert(BannerAdLogic bannerAdLogic)
		{
			m_AmInitialised = true;
			BannerAdLogger("InitialiseSelf: Caching Application props");
			m_AppProperties = new ApplicationProperties();
			m_AppProperties.CacheApplicationProperties();
			BannerAdLogger("InitialiseSelf: Caching current Device Props");
			m_DeviceProperties = FurbyGlobals.DeviceSettings.DeviceProperties;
			BannerAdLogger("InitialiseSelf: Working out from GameConfig");
			m_BannerAdvert = GetBannerAdvert();
			yield return StartCoroutine(TriggerAdvertDownload());
			yield return new WaitForSeconds(m_TimeSecondsToWait);
			m_BannerHandler.gameObject.SetActive(false);
			bool shouldShowAdverts = ShouldShowAdverts();
			if (bannerAdLogic == BannerAdLogic.IgnoreGameLogic)
			{
				shouldShowAdverts = true;
			}
			if (shouldShowAdverts)
			{
				BannerAdLogger("InitialiseSelf: Going to try and show adverts...");
				if (m_DownloadSuccess)
				{
					BannerAdLogger("InitialiseSelf: Have downloaded, munging...");
					ApplyTextureToBannerContainer(m_Texture2D, m_BannerAdvert.m_BannerURL, m_BannerAdvert.m_ScaleFactor);
				}
				else
				{
					BannerAdLogger("InitialiseSelf: No download...");
				}
			}
			else
			{
				BannerAdLogger("InitialiseSelf: No adverts...");
			}
		}

		private void BannerAdLogger(string logString)
		{
		}

		private IEnumerator TriggerAdvertDownload()
		{
			AbortCurrentDownload();
			yield return StartCoroutine(DownloadAdvert());
		}

		private IEnumerator WaitForNetwork()
		{
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForGameConfigResolution()
		{
			while (SingletonInstance<GameConfigDownloader>.Instance.AmDownloading)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator DownloadAdvert()
		{
			BannerAdLogger("DownloadAdvert() START");
			if (SetupNetworking.IsNetworkingEnabled)
			{
				m_CountryCode = Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode;
				m_RequestSuccess = true;
				m_AmDownloading = true;
				BannerAdLogger("DownloadAdvert() Starting thread");
				m_Thread = null;
				m_Thread = new Thread(DownloadImageThread);
				m_Thread.Start();
				yield return new WaitForEndOfFrame();
				while (!m_ThreadEvent.WaitOne(0))
				{
					yield return null;
				}
				m_AmDownloading = false;
				BannerAdLogger("DownloadAdvert() thread finished, decoding...");
				ConvertDownloadedStringToTextureData();
				m_Thread = null;
			}
			else
			{
				m_RequestSuccess = false;
				m_AmDownloading = false;
			}
			BannerAdLogger("DownloadAdvert() END");
		}

		private void ConvertDownloadedStringToTextureData()
		{
			if (m_RawImageData != string.Empty)
			{
				BannerAdLogger("BannerAdController.ApplyDownloadedAdvert() From a string with " + m_RawImageData.Length + " chars");
				string s = Encoding.ASCII.GetString(Convert.FromBase64String(m_RawImageData));
				byte[] data = Convert.FromBase64String(s);
				m_Texture2D = new Texture2D(1, 1);
				m_Texture2D.LoadImage(data);
				m_Texture2D.Apply();
			}
		}

		public void AbortCurrentDownload()
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

		private BannerAdvert GetBannerAdvert()
		{
			GameConfigBlob gameConfigBlob = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob();
			switch (m_DeviceProperties.m_ApplicationModifiers.m_BannerDimension)
			{
			case BannerDimension.BannerSize_320x50:
				return gameConfigBlob.m_Banner320x50;
			case BannerDimension.BannerSize_768x66:
				return gameConfigBlob.m_Banner768x66;
			default:
				return null;
			}
		}

		private void DownloadImageThread()
		{
			BannerAdLogger("DownloadImageThread START");
			m_DownloadSuccess = false;
			try
			{
				m_DownloadSuccess = DownloadImage_Ex(m_BannerAdvert.m_BannerIdent);
			}
			catch (Exception ex)
			{
				BannerAdLogger(ex.ToString());
				m_DownloadSuccess = false;
			}
			finally
			{
				m_ThreadEvent.Set();
			}
			BannerAdLogger("DownloadImageThread END");
		}

		private bool DownloadImage_Ex(string advertIdent)
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
			string url = gameConfigurationRequestBuilder2.GetUrl(advertIdent);
			DebugUtils.Log_InOrange("gcRequestURL: " + url);
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
			ImageBlobResponse item = RESTfulApi.GetItem<ImageBlobResponse, MessageRequestContent>(HttpVerb.POST, url, messageRequestContent2, requestDetails2);
			if (item == null)
			{
				flag = false;
			}
			else if (string.IsNullOrEmpty(item.RequestId))
			{
				flag = false;
			}
			else if (string.Compare(item.RequestId, messageRequestContent2.RequestId) != 0)
			{
				flag = false;
			}
			stopwatch.Stop();
			if (flag)
			{
				BannerAdLogger("BannerAdController.DownloadImageThread() Success, got " + item.Data.Length + " chars");
				m_RawImageData = item.Data;
			}
			else
			{
				BannerAdLogger("BannerAdController.DownloadImageThread() FAILED...");
			}
			return flag;
		}

		private bool ShouldShowAdverts()
		{
			int sessionCount = GetSessionCount();
			bool flag = HasSessonThresholdBeenMet(sessionCount);
			return !HasConnectedAFurby() && flag;
		}

		private bool HasSessonThresholdBeenMet(int sessionCount)
		{
			return sessionCount > 0 && sessionCount % m_SessionInterval == 0;
		}

		private int GetSessionCount()
		{
			int num = 0;
			int num2 = 0;
			int numSlots = Singleton<GameDataStoreObject>.Instance.GetNumSlots();
			for (int i = 0; i < numSlots; i++)
			{
				GameData slot = Singleton<GameDataStoreObject>.Instance.GetSlot(i);
				if (slot.HasCompletedFirstTimeFlow)
				{
					num += slot.m_numFurbySessions;
					num += slot.m_numNoFurbySessions;
				}
			}
			return num + num2;
		}

		private bool HasConnectedAFurby()
		{
			int numSlots = Singleton<GameDataStoreObject>.Instance.GetNumSlots();
			for (int i = 0; i < numSlots; i++)
			{
				GameData slot = Singleton<GameDataStoreObject>.Instance.GetSlot(i);
				if (slot.HasCompletedFirstTimeFlow && !slot.NoFurbyMode)
				{
					return true;
				}
			}
			return false;
		}

		private void ApplyTextureToBannerContainer(Texture2D rawTexture, string targetURL, float scaleFactor)
		{
			m_TargetTexture.mainTexture = rawTexture;
			m_TargetTexture.pivot = UIWidget.Pivot.Bottom;
			m_TargetTexture.color = new Color(0f, 0f, 0f, 0f);
			m_TargetTexture.MakePixelPerfect();
			m_TargetTexture.transform.localScale *= scaleFactor;
			m_BannerHandler.URL = targetURL;
			m_BoxCollider.size = m_TargetTexture.gameObject.transform.localScale;
			m_BannerHandler.gameObject.SetActive(true);
			FadeUpTheAdvert();
		}

		public void FadeUpTheAdvert()
		{
			m_TargetTexture.color = new Color(1f, 1f, 1f, 0f);
			TweenAlpha.Begin(m_TargetTexture.gameObject, m_FadeUpDuration, 1f);
			Invoke("ShowAdvertDeclaration", 1f);
		}

		private void ShowAdvertDeclaration()
		{
			switch (m_DeviceProperties.m_ApplicationModifiers.m_BannerDimension)
			{
			case BannerDimension.BannerSize_320x50:
				m_AdvertDeclaration_SD.SetActive(true);
				break;
			case BannerDimension.BannerSize_768x66:
				m_AdvertDeclaration_HD.SetActive(true);
				break;
			}
		}
	}
}
