using System;
using System.Collections;
using System.Threading;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public class ServerGameDataManager : ProviderManager<ServerGameDataManager, CloudServiceGameDataProvider>, IProvider, SaveObject
	{
		private bool m_saveDataLoaded;

		private CloudServiceGameDataProvider m_provider;

		private ServerGameData m_gameData;

		private AbTests m_abTests;

		private Thread m_downloadBlobThread;

		private bool m_downloadSucceeded;

		public bool SaveDataLoaded
		{
			get
			{
				return m_saveDataLoaded;
			}
		}

		public ServerGameData GameData
		{
			get
			{
				return m_gameData;
			}
		}

		public AbTests AbTests
		{
			get
			{
				return m_abTests;
			}
		}

		public void SerializeTo(SaveGameWriter writer)
		{
		}

		public void DeserializeFrom(SaveGameReader reader)
		{
		}

		public int GetVersion()
		{
			return 1;
		}

		protected override void Initialise()
		{
			Singleton<SaveGame>.Instance.Register(new SaveGameItem("GameDataBlobDownloader", this));
			base.Initialise();
			if (m_providers.Count == 0)
			{
				Logging.LogError("GameDataBlobDownloader could not find a valid provider for this platform!");
				return;
			}
			m_provider = m_providers[0];
			StartCoroutine("DownloadBlobData");
		}

		private IEnumerator DownloadBlobData()
		{
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			SetupNetworking.GetServerDetails(Servers.Impala);
			if (SetupNetworking.IsNetworkingEnabled)
			{
				m_downloadBlobThread = new Thread(DownloadBlobThread);
				m_downloadBlobThread.Start();
				yield return new WaitForEndOfFrame();
				while (m_downloadBlobThread.IsAlive)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				Logging.LogWarning("Skipping bob data from cloud since networking is disabled");
			}
			if (!m_downloadSucceeded)
			{
				Logging.LogError("Failed to download ServerGameData from Azure. Using default data.");
				DebugNotifications.AddNotification("Failed to download ServerGameData from the cloud. Using default data.", TimeSpan.FromSeconds(15.0));
			}
			else
			{
				DebugNotifications.AddNotification("Downloaded ServerGameData from the cloud.", TimeSpan.FromSeconds(15.0));
			}
			m_gameData = m_provider.BlobData;
			m_abTests = m_provider.AbTests;
			if (m_abTests != null)
			{
				string msg = string.Format("AB Group info: {0}", m_abTests);
				DebugNotifications.AddNotification(msg, TimeSpan.FromSeconds(15.0));
			}
			Logging.Log("ServerGameData downloaded is ready.\r\n" + m_gameData);
		}

		private void DownloadBlobThread()
		{
			try
			{
				m_downloadSucceeded = m_provider.DownloadBlobData();
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to get server time: " + ex.ToString());
			}
		}
	}
}
