using Relentless;

namespace Furby
{
	public class SettingsHelper : Singleton<FurbyGlobals>
	{
		private bool m_DeleteBabyRequested;

		private bool m_BabyWasDeleted;

		private string m_DeletedBabyName = string.Empty;

		private bool m_DeleteEggRequested;

		private bool m_ChangeFurbyRequested;

		private AdultFurbyType m_PreviousFurbyType = AdultFurbyType.NoFurby;

		private bool m_RequestCameFromFrontEnd;

		private bool m_CameFromSettings;

		public void RequestDeleteBaby()
		{
			m_DeleteBabyRequested = true;
		}

		public void ClearDeleteBabyRequest()
		{
			m_DeleteBabyRequested = false;
		}

		public bool IsDeleteBabyRequested()
		{
			return m_DeleteBabyRequested;
		}

		public void SetBabyWasDeleted(string name)
		{
			m_BabyWasDeleted = true;
			m_DeletedBabyName = name;
		}

		public void ClearBabyWasDeleted()
		{
			m_BabyWasDeleted = false;
			m_DeletedBabyName = string.Empty;
		}

		public bool WasBabyDeleted()
		{
			return m_BabyWasDeleted;
		}

		public string GetDeletedBabyName()
		{
			return m_DeletedBabyName;
		}

		public void RequestDeleteEgg()
		{
			m_DeleteEggRequested = true;
		}

		public void ClearDeleteEggRequest()
		{
			m_DeleteEggRequested = false;
		}

		public bool IsDeleteEggRequested()
		{
			return m_DeleteEggRequested;
		}

		public void RequestChangeFurby(AdultFurbyType furbyType, bool requestCameFromFrontEnd)
		{
			m_ChangeFurbyRequested = true;
			m_PreviousFurbyType = furbyType;
			m_RequestCameFromFrontEnd = requestCameFromFrontEnd;
		}

		public bool IsChangeFurbyRequested()
		{
			return m_ChangeFurbyRequested;
		}

		public void ClearChangeFurbyRequest()
		{
			m_ChangeFurbyRequested = false;
			m_PreviousFurbyType = AdultFurbyType.NoFurby;
		}

		public AdultFurbyType GetPreviousFurbyType()
		{
			return m_PreviousFurbyType;
		}

		public bool RequestCameFromFrontEnd()
		{
			return m_RequestCameFromFrontEnd;
		}

		public bool IsUpgradableGame()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode && Singleton<GameDataStoreObject>.Instance.Data.FlowStage == FlowStage.Normal;
		}

		public void SetCameFromSettings()
		{
			m_CameFromSettings = true;
		}

		public bool CameFromSettings()
		{
			return m_CameFromSettings;
		}

		public void ClearCameFromSettings()
		{
			m_CameFromSettings = false;
		}
	}
}
