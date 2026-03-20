using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class GameConfiguration : SingletonInstance<GameConfiguration>
	{
		[SerializeField]
		public GameConfigBlob m_DefaultGameConfig = new GameConfigBlob();

		private GameConfigBlob m_DownloadedGameConfigBlob;

		public override void Awake()
		{
			m_DownloadedGameConfigBlob = null;
		}

		public GameConfigBlob GetGameConfigBlob()
		{
			SingletonInstance<GameConfigDownloader>.Instance.AbortDownloadOfData();
			if (m_DownloadedGameConfigBlob != null)
			{
				return m_DownloadedGameConfigBlob;
			}
			Logging.LogWarning("GetGameConfig: Don't have a downloaded GameConfig, checking if persisted");
			if (Singleton<GameDataStoreObject>.Instance.Data.m_HaveStoredADownloadedGameConfig)
			{
				Logging.LogWarning("GetGameConfig: Returning persisted GameConfig");
				return Singleton<GameDataStoreObject>.Instance.Data.m_LastGameConfig;
			}
			Logging.LogWarning("GetGameConfig: Don't have a persisted GameConfig... Using default");
			return m_DefaultGameConfig;
		}

		public bool IsIAPAvailable()
{
    return false;
}

		public void ClearDownloadedConfig()
		{
			m_DownloadedGameConfigBlob = null;
		}

		public void SetDownloadedGameConfig(GameConfigBlob config)
		{
			m_DownloadedGameConfigBlob = config;
		}
	}
}
