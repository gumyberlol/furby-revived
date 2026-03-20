using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class SerializableDictionary<K, V>
	{
		[SerializeField]
		private List<K> m_keys;

		[SerializeField]
		private List<V> m_values;

		private Dictionary<K, int> m_valueLookup;

		public int Count
		{
			get
			{
				return m_keys.Count;
			}
		}

		public ICollection Keys
		{
			get
			{
				return m_keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return m_values;
			}
		}

		public V this[K key]
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

		public SerializableDictionary()
		{
			m_keys = new List<K>();
			m_values = new List<V>();
			m_valueLookup = new Dictionary<K, int>();
		}

		public bool ContainsKey(K key)
		{
			return m_valueLookup.ContainsKey(key);
		}

		public void Add(K key, V value)
		{
			m_valueLookup[key] = m_keys.Count;
			m_keys.Add(key);
			m_values.Add(value);
		}

		public bool Remove(K key)
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

		public bool TryGetValue(K key, out V value)
		{
			int value2;
			if (!m_valueLookup.TryGetValue(key, out value2))
			{
				value = default(V);
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
