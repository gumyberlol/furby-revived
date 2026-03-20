namespace Relentless
{
	public class PersistedDataStoreObject<T, U> where T : class, new() where U : PersistedDataStore<T>, new()
	{
		private readonly PersistedDataStore<T> m_persistedDataStore = new U();

		public T Data
		{
			get
			{
				return m_persistedDataStore.Data;
			}
		}

		public PersistedDataStoreObject()
		{
			m_persistedDataStore.Load();
		}

		public void Save()
		{
			m_persistedDataStore.Save();
		}
	}
}
