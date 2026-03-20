using System;
using System.Collections.Generic;

namespace JsonFx.Serialization.Providers
{
	public class DataReaderProvider : IDataReaderProvider
	{
		private readonly IDictionary<string, IDataReader> ReadersByMime = new Dictionary<string, IDataReader>(StringComparer.OrdinalIgnoreCase);

		public DataReaderProvider(IEnumerable<IDataReader> readers)
		{
			if (readers == null)
			{
				return;
			}
			foreach (IDataReader reader in readers)
			{
				foreach (string item in reader.ContentType)
				{
					if (!string.IsNullOrEmpty(item) && !ReadersByMime.ContainsKey(item))
					{
						ReadersByMime[item] = reader;
					}
				}
			}
		}

		public virtual IDataReader Find(string contentTypeHeader)
		{
			string key = DataProviderUtility.ParseMediaType(contentTypeHeader);
			IDataReader value;
			if (ReadersByMime.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}
	}
}
