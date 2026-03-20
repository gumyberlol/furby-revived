using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class SerializableStringMap
	{
		[SerializeField]
		private List<string> m_keys;

		[SerializeField]
		private List<string> m_values;

		private Dictionary<string, int> m_valueLookup;

		public int Count
		{
			get
			{
				return m_keys.Count;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return m_keys;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				return m_values;
			}
		}

		public string this[string key]
		{
			get
			{
				return m_values[m_valueLookup[key]];
			}
			set
			{
				int value2;
				if (m_valueLookup.TryGetValue(key, out value2))
				{
					m_values[value2] = value;
					return;
				}
				value2 = m_keys.Count;
				m_valueLookup[key] = value2;
				m_keys.Add(key);
				m_values.Add(value);
			}
		}

		public SerializableStringMap()
		{
			m_keys = new List<string>();
			m_values = new List<string>();
			m_valueLookup = new Dictionary<string, int>();
		}

		public bool ContainsKey(string key)
		{
			return m_valueLookup.ContainsKey(key);
		}

		public void Add(string key, string value)
		{
			m_valueLookup[key] = m_keys.Count;
			m_keys.Add(key);
			m_values.Add(value);
		}

		public bool Remove(string key)
		{
			int value;
			if (!m_valueLookup.TryGetValue(key, out value))
			{
				return false;
			}
			m_valueLookup.Remove(key);
			m_keys.RemoveAt(value);
			m_values.RemoveAt(value);
			return true;
		}

		public bool TryGetValue(string key, out string value)
		{
			int value2;
			if (!m_valueLookup.TryGetValue(key, out value2))
			{
				value = null;
				return false;
			}
			value = m_values[value2];
			return true;
		}

		public void Initialise()
		{
			int count = m_keys.Count;
			for (int i = 0; i < count; i++)
			{
				m_valueLookup[m_keys[i]] = i;
			}
		}
	}
}
