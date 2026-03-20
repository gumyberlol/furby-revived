using System;
using System.Collections.Generic;

namespace JsonFx.Serialization.Providers
{
	public class DataWriterProvider : IDataWriterProvider
	{
		private readonly IDataWriter DefaultWriter;

		private readonly IDictionary<string, IDataWriter> WritersByExt = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		private readonly IDictionary<string, IDataWriter> WritersByMime = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		public IDataWriter DefaultDataWriter
		{
			get
			{
				return DefaultWriter;
			}
		}

		public DataWriterProvider(IEnumerable<IDataWriter> writers)
		{
			if (writers == null)
			{
				return;
			}
			foreach (IDataWriter writer in writers)
			{
				if (DefaultWriter == null)
				{
					DefaultWriter = writer;
				}
				foreach (string item in writer.ContentType)
				{
					if (!string.IsNullOrEmpty(item) && !WritersByMime.ContainsKey(item))
					{
						WritersByMime[item] = writer;
					}
				}
				foreach (string item2 in writer.FileExtension)
				{
					if (!string.IsNullOrEmpty(item2) && !WritersByExt.ContainsKey(item2))
					{
						string key = DataProviderUtility.NormalizeExtension(item2);
						WritersByExt[key] = writer;
					}
				}
			}
		}

		public virtual IDataWriter Find(string extension)
		{
			extension = DataProviderUtility.NormalizeExtension(extension);
			IDataWriter value;
			if (WritersByExt.TryGetValue(extension, out value))
			{
				return value;
			}
			return null;
		}

		public virtual IDataWriter Find(string acceptHeader, string contentTypeHeader)
		{
			foreach (string item in DataProviderUtility.ParseHeaders(acceptHeader, contentTypeHeader))
			{
				IDataWriter value;
				if (WritersByMime.TryGetValue(item, out value))
				{
					return value;
				}
			}
			return null;
		}
	}
}
