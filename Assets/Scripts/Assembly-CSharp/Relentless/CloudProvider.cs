using System;
using UnityEngine;

namespace Relentless
{
	public abstract class CloudProvider<T> : ProviderBase where T : class, new()
	{
		public string Filename;

		public string ContainerName;

		public TextAsset DefaultData;

		private T m_blobData;

		private T m_blobDefaultData;

		private AbTests m_abTests;

		public T BlobData
		{
			get
			{
				return m_blobData;
			}
			protected set
			{
				m_blobData = value;
			}
		}

		public AbTests AbTests
		{
			get
			{
				return m_abTests;
			}
			protected set
			{
				m_abTests = value;
			}
		}

		protected CloudProvider()
		{
			m_blobData = new T();
		}

		public override void Initialise()
		{
			try
			{
				if (DefaultData == null || string.IsNullOrEmpty(DefaultData.text))
				{
					Logging.LogError("Default Data not set up. Creating empty object!");
					m_blobDefaultData = new T();
				}
				else
				{
					m_blobDefaultData = JSONSerialiser.Parse<T>(DefaultData.text);
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to parse default data : " + ex.ToString());
				m_blobDefaultData = new T();
			}
			m_blobData = m_blobDefaultData;
		}

		public abstract bool DownloadBlobData();

		protected void UseDefaultData()
		{
			Logging.LogError("Using default data.");
			m_blobData = m_blobDefaultData;
		}

		protected T ParseData(string data)
		{
			try
			{
				if (string.IsNullOrEmpty(data))
				{
					return (T)null;
				}
				return JSONSerialiser.Parse<T>(data);
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to parse data : " + ex.ToString());
				return (T)null;
			}
		}
	}
}
