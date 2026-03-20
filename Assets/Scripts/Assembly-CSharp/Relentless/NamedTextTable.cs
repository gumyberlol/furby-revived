using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class NamedTextTable : ScriptableObject
	{
		[SerializeField]
		private SerializableStringMap m_textTable;

		public ICollection<string> Keys
		{
			get
			{
				return m_textTable.Keys;
			}
		}

		public string this[string key]
		{
			get
			{
				return GetString(key);
			}
			set
			{
				SetString(key, value);
			}
		}

		public NamedTextTable()
		{
			m_textTable = new SerializableStringMap();
		}

		public string GetString(string key)
		{
			string value;
			if (!m_textTable.TryGetValue(key, out value))
			{
				return key;
			}
			return value;
		}

		public bool TryGetString(string key, out string returnValue)
		{
			return m_textTable.TryGetValue(key, out returnValue);
		}

		public void SetString(string key, string value)
		{
			m_textTable[key] = value;
		}

		public bool HasString(string key)
		{
			return m_textTable.ContainsKey(key);
		}

		public string GetFormattedString(string key, params object[] values)
		{
			string format = GetString(key);
			return string.Format(format, values);
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (string key in m_textTable.Keys)
			{
				yield return key;
			}
		}

		private void OnEnable()
		{
			m_textTable.Initialise();
		}
	}
}
