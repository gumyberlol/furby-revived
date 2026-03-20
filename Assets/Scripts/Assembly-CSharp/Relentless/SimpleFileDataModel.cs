using System;

namespace Relentless
{
	public class SimpleFileDataModel : IDataModel
	{
		private SimpleNameValueStore m_persistedData;

		public string DataText { get; set; }

		public void Load()
		{
			if (!string.IsNullOrEmpty(DataText))
			{
				SimpleNameValueStore simpleNameValueStore = JSONSerialiser.Parse<SimpleNameValueStore>(DataText);
				if (simpleNameValueStore != null)
				{
					m_persistedData = simpleNameValueStore;
				}
			}
		}

		public void Save()
		{
		}

		public TData GetValue<TData>(string key)
		{
			if (!m_persistedData.DataDictionary.ContainsKey(key))
			{
				return default(TData);
			}
			object obj = m_persistedData.DataDictionary[key];
			if (obj == null)
			{
				return default(TData);
			}
			return (TData)Convert.ChangeType(obj, typeof(TData));
		}
	}
}
