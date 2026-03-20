using Relentless;
using UnityEngine;

namespace Furby
{
	public class DebugMenuButton : RelentlessMonoBehaviour
	{
		public enum DebugFunctions
		{
			ClearSaveGame = 0,
			GiveLotsOfFurbucks = 1,
			LevelUp = 2
		}

		public DebugFunctions m_debugFunction;

		private void Start()
		{
		}

		private void OnClick()
		{
			switch (m_debugFunction)
			{
			case DebugFunctions.ClearSaveGame:
				Singleton<GameDataStoreObject>.Instance.Clear();
				PlayerPrefs.DeleteAll();
				PlayerPrefs.Save();
				break;
			case DebugFunctions.GiveLotsOfFurbucks:
				Singleton<GameDataStoreObject>.Instance.Data.FurbucksBalance += 1000;
				break;
			case DebugFunctions.LevelUp:
				Singleton<GameDataStoreObject>.Instance.Data.Level = FurbyGlobals.AdultLibrary.XpLevels.Count - 1;
				Singleton<GameDataStoreObject>.Instance.Data.Xp = FurbyGlobals.AdultLibrary.XpLevels[FurbyGlobals.AdultLibrary.XpLevels.Count - 1] + 1;
				Singleton<GameDataStoreObject>.Instance.Save();
				break;
			}
		}
	}
}
