using UnityEngine;

namespace Relentless
{
	public class SaveGameItem
	{
		private string m_name;

		private SaveObject m_saveObject;

		public SaveGameItem(string name, SaveObject saveObject)
		{
			m_name = name;
			m_saveObject = saveObject;
		}

		public void Load()
		{
			if (PlayerPrefs.HasKey(m_name))
			{
				SaveGameReader saveGameReader = new SaveGameReader(PlayerPrefs.GetString(m_name));
				int num = saveGameReader.ReadInt();
				if (num == m_saveObject.GetVersion())
				{
					m_saveObject.DeserializeFrom(saveGameReader);
				}
				else
				{
					Save();
				}
			}
			else
			{
				Save();
			}
		}

		public void Save()
		{
			SaveGameWriter saveGameWriter = new SaveGameWriter();
			saveGameWriter.WriteInt(m_saveObject.GetVersion());
			m_saveObject.SerializeTo(saveGameWriter);
			string writtenString = saveGameWriter.GetWrittenString();
			PlayerPrefs.SetString(m_name, writtenString);
		}
	}
}
