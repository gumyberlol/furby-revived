using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Relentless
{
	public abstract class PersistedDataStore<T> where T : class, new()
	{
		private class UpdaterToVersion
		{
			public int m_to;

			public VersionUpdater m_func;

			public UpdaterToVersion(int to, VersionUpdater func)
			{
				m_to = to;
				m_func = func;
			}
		}

		public delegate string VersionUpdater(string rawData);

		private T m_persistedData;

		private byte[] m_key = new byte[16]
		{
			244, 36, 134, 224, 131, 22, 131, 178, 168, 29,
			95, 29, 131, 27, 51, 134
		};

		private SymmetricAlgorithm m_algo = new AesManaged();

		private List<UpdaterToVersion> m_updaters = new List<UpdaterToVersion>();

		public T Data
		{
			get
			{
				return m_persistedData;
			}
		}

		protected abstract int Version { get; }

		public string Name { get; set; }

		public void Save()
		{
			if (m_persistedData != null)
			{
				PlayerPrefs.SetInt(MakeKeyName(PersistedDataStoreKeys.Version), Version);
				string data = JSONSerialiser.AsString(m_persistedData);
				data = Encrypt(data);
				PlayerPrefs.SetString(MakeKeyName(PersistedDataStoreKeys.Data), data);
				PlayerPrefs.Save();
			}
		}

		protected string Encrypt(string data)
		{
			try
			{
				m_algo.KeySize = m_key.Length * 8;
				m_algo.Key = m_key;
				m_algo.GenerateIV();
				using (MemoryStream memoryStream = new MemoryStream())
				{
					memoryStream.Write(m_algo.IV, 0, m_algo.IV.Length);
					StreamWriter streamWriter = new StreamWriter(new CryptoStream(memoryStream, m_algo.CreateEncryptor(), CryptoStreamMode.Write));
					streamWriter.Write(data);
					streamWriter.Flush();
					streamWriter.Close();
					byte[] inArray = memoryStream.ToArray();
					data = Convert.ToBase64String(inArray);
				}
				return data;
			}
			catch (Exception ex)
			{
				Logging.Log(string.Format("While encoding, {0}", ex.Message));
				throw;
			}
		}

		private string Decrypt(string data)
		{
			m_algo.KeySize = m_key.Length * 8;
			m_algo.Key = m_key;
			byte[] buffer = Convert.FromBase64String(data);
			using (Stream stream = new MemoryStream(buffer))
			{
				int num = m_algo.IV.Length;
				byte[] array = new byte[num];
				stream.Read(array, 0, array.Length);
				m_algo.IV = array;
				StreamReader streamReader = new StreamReader(new CryptoStream(stream, m_algo.CreateDecryptor(), CryptoStreamMode.Read));
				data = streamReader.ReadToEnd();
				return data;
			}
		}

		public void Load()
		{
			try
			{
				LoadAndUpgrade();
			}
			catch (Exception ex)
			{
				Logging.LogError(string.Format("While loading save data, caught exception {0} with message {1}", ex.GetType().ToString(), ex.Message));
				Application.Quit();
			}
		}

		private void LoadAndUpgrade()
		{
			int num = 0;
			try
			{
				num = GetVersion();
			}
			catch (ApplicationException ex)
			{
				Logging.LogError(string.Format("While trying to establish version number of existing save game, caught exception of type \"{0}\" with message \"{1}\".  This probably means you simply do not have a save game yet.  Making one for you now...", ex.GetType().ToString(), ex.Message));
				m_persistedData = new T();
				Save();
				num = GetVersion();
			}
			int version = Version;
			string data = GetData();
			data = DoUpgrades(data, num, version);
			data = Decrypt(data);
			m_persistedData = JSONSerialiser.Parse<T>(data);
		}

		protected VersionUpdater WrapEncryption(VersionUpdater updater)
		{
			return delegate(string data)
			{
				data = Decrypt(data);
				data = updater(data);
				data = Encrypt(data);
				return data;
			};
		}

		public void Clear()
		{
			m_persistedData = new T();
			Save();
		}

		private string MakeKeyName(PersistedDataStoreKeys keyType)
		{
			if (string.IsNullOrEmpty(Name))
			{
				throw new InvalidOperationException("Empty Name property");
			}
			return string.Format("{0}_{1}", Name, keyType);
		}

		private int GetVersion()
		{
			string text = MakeKeyName(PersistedDataStoreKeys.Version);
			if (!PlayerPrefs.HasKey(text))
			{
				throw new ApplicationException(string.Format("Key \"{0}\" does not exist.", text));
			}
			return PlayerPrefs.GetInt(text);
		}

		private string GetData()
		{
			string text = MakeKeyName(PersistedDataStoreKeys.Data);
			if (!PlayerPrefs.HasKey(text))
			{
				throw new ApplicationException(string.Format("Key \"{0}\" does not exist.", text));
			}
			return PlayerPrefs.GetString(text);
		}

		private string DoUpgrades(string data, int fromVersion, int toVersion)
		{
			if (fromVersion != toVersion)
			{
			}
			if (fromVersion > toVersion)
			{
				throw new ApplicationException(string.Format("Cannot upgrade data from {0} to {1}", fromVersion, toVersion));
			}
			while (fromVersion < toVersion)
			{
				int nextVersion = fromVersion + 1;
				Logging.Log(string.Format("Applying incremental upgrade from {0} to {1}", fromVersion, nextVersion));
				UpdaterToVersion updaterToVersion = m_updaters.Find((UpdaterToVersion x) => x.m_to == nextVersion);
				if (updaterToVersion == null)
				{
					throw new ApplicationException(string.Format("No version updater to version {0}", nextVersion));
				}
				data = updaterToVersion.m_func(data);
				fromVersion = nextVersion;
			}
			return data;
		}

		protected void AddVersionUpdater(int to, VersionUpdater updater)
		{
			if (m_updaters.Find((UpdaterToVersion x) => x.m_to == to) != null)
			{
				throw new ApplicationException(string.Format("Cannot add VersionUpdater to version {0} when one is already added.", to));
			}
			m_updaters.Add(new UpdaterToVersion(to, updater));
		}
	}
}
