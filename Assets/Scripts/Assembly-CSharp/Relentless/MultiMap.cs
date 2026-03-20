using System.Collections.Generic;

namespace Relentless
{
	public class MultiMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
	{
		public void Add(TKey key, TValue value)
		{
			List<TValue> value2 = null;
			if (!TryGetValue(key, out value2))
			{
				value2 = new List<TValue>();
				Add(key, value2);
			}
			value2.Add(value);
		}

		public List<TKey> GetKeys()
		{
			return new List<TKey>(base.Keys);
		}

		public bool ContainsValue(TKey key, TValue value)
		{
			bool result = false;
			List<TValue> value2 = null;
			if (TryGetValue(key, out value2))
			{
				result = value2.Contains(value);
			}
			return result;
		}

		public void Remove(TKey key, TValue value)
		{
			List<TValue> value2 = null;
			if (TryGetValue(key, out value2))
			{
				value2.Remove(value);
				if (value2.Count <= 0)
				{
					Remove(key);
				}
			}
		}

		public void Merge(MultiMap<TKey, TValue> toMergeWith)
		{
			if (toMergeWith == null)
			{
				return;
			}
			foreach (KeyValuePair<TKey, List<TValue>> item in toMergeWith)
			{
				foreach (TValue item2 in item.Value)
				{
					Add(item.Key, item2);
				}
			}
		}

		public List<TValue> GetValues(TKey key, bool returnEmptySet)
		{
			List<TValue> value = null;
			if (!TryGetValue(key, out value) && returnEmptySet)
			{
				value = new List<TValue>();
			}
			return value;
		}
	}
}
