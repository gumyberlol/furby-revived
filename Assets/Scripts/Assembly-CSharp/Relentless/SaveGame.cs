using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class SaveGame : Singleton<SaveGame>
	{
		private List<SaveGameItem> m_saveGameItems = new List<SaveGameItem>();

		private void Start()
		{
		}

		public void Register(SaveGameItem item)
		{
			m_saveGameItems.Add(item);
		}

		public void Save()
		{
			foreach (SaveGameItem saveGameItem in m_saveGameItems)
			{
				saveGameItem.Save();
			}
			PlayerPrefs.Save();
		}

		public void Load()
		{
			foreach (SaveGameItem saveGameItem in m_saveGameItems)
			{
				saveGameItem.Load();
			}
		}
	}
}
