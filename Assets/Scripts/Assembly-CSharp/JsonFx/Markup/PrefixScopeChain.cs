using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Serialization;

namespace JsonFx.Markup
{
	internal class PrefixScopeChain
	{
		public class Scope : IEnumerable, IEnumerable<KeyValuePair<string, string>>
		{
			private SortedList<string, string> prefixes;

			public DataName TagName { get; set; }

			public string this[string prefix]
			{
				get
				{
					string value;
					if (prefixes == null || !prefixes.TryGetValue(prefix, out value))
					{
						return null;
					}
					return value;
				}
				set
				{
					if (prefixes == null)
					{
						prefixes = new SortedList<string, string>(StringComparer.Ordinal);
					}
					prefixes[prefix] = value;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public bool ContainsPrefix(string prefix)
			{
				return prefixes != null && prefixes.ContainsKey(prefix);
			}

			public bool ContainsNamespace(string namespaceUri)
			{
				return prefixes != null && prefixes.ContainsValue(namespaceUri);
			}

			public bool TryGetNamespace(string prefix, out string namespaceUri)
			{
				namespaceUri = null;
				return prefixes != null && prefixes.TryGetValue(prefix, out namespaceUri);
			}

			public bool TryGetPrefix(string namespaceUri, out string prefix)
			{
				prefix = null;
				if (prefixes == null)
				{
					return false;
				}
				int num = prefixes.IndexOfValue(namespaceUri);
				if (num < 0)
				{
					return false;
				}
				prefix = prefixes.Keys[num];
				return true;
			}

			public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
			{
				if (prefixes == null)
				{
					yield break;
				}
				foreach (KeyValuePair<string, string> prefix in prefixes)
				{
					yield return prefix;
				}
			}
		}

		private readonly List<Scope> Chain = new List<Scope>();

		private int nsCounter;

		public int Count
		{
			get
			{
				return Chain.Count;
			}
		}

		public bool HasScope
		{
			get
			{
				return Chain.Count > 0;
			}
		}

		public void Push(Scope scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			Chain.Add(scope);
		}

		public Scope Peek()
		{
			int num = Chain.Count - 1;
			if (num < 0)
			{
				return null;
			}
			return Chain[num];
		}

		public Scope Pop()
		{
			int num = Chain.Count - 1;
			if (num < 0)
			{
				return null;
			}
			Scope result = Chain[num];
			Chain.RemoveAt(num);
			return result;
		}

		public bool ContainsPrefix(string prefix)
		{
			return Chain.FindLastIndex((Scope item) => item.ContainsPrefix(prefix)) >= 0;
		}

		public bool ContainsNamespace(string namespaceUri)
		{
			return Chain.FindLastIndex((Scope item) => item.ContainsNamespace(namespaceUri)) >= 0;
		}

		public string GetNamespace(string prefix, bool throwOnUndeclared)
		{
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			Scope scope = Chain.FindLast((Scope item) => item.ContainsPrefix(prefix));
			if (scope == null && !string.IsNullOrEmpty(prefix))
			{
				if (throwOnUndeclared)
				{
					throw new InvalidOperationException(string.Format("Unknown scope prefix ({0})", prefix));
				}
				scope = Chain.FindLast((Scope item) => item.ContainsPrefix(string.Empty));
			}
			string namespaceUri;
			if (scope != null && scope.TryGetNamespace(prefix, out namespaceUri))
			{
				return namespaceUri;
			}
			return null;
		}

		public string GetPrefix(string namespaceUri, bool throwOnUndeclared)
		{
			if (namespaceUri == null)
			{
				namespaceUri = string.Empty;
			}
			Scope scope = Chain.FindLast((Scope item) => item.ContainsNamespace(namespaceUri));
			if (scope == null && !string.IsNullOrEmpty(namespaceUri))
			{
				if (throwOnUndeclared)
				{
					throw new InvalidOperationException(string.Format("Unknown scope prefix ({0})", namespaceUri));
				}
				scope = Chain.FindLast((Scope item) => item.ContainsNamespace(string.Empty));
			}
			string prefix;
			if (scope != null && scope.TryGetPrefix(namespaceUri, out prefix))
			{
				return prefix;
			}
			return null;
		}

		public bool ContainsTag(DataName closeTag)
		{
			int num = Chain.FindLastIndex((Scope item) => item.TagName == closeTag);
			return num >= 0;
		}

		public void Clear()
		{
			Chain.Clear();
		}

		public string EnsurePrefix(string preferredPrefix, string namespaceUri)
		{
			string text = GetPrefix(namespaceUri, false);
			if (text == null && !string.IsNullOrEmpty(namespaceUri))
			{
				text = ((!string.IsNullOrEmpty(preferredPrefix)) ? preferredPrefix : GeneratePrefix(namespaceUri));
			}
			return text;
		}

		public string GeneratePrefix(string namespaceUri)
		{
			string standardPrefix = DataName.GetStandardPrefix(namespaceUri);
			return standardPrefix ?? string.Concat('q', ++nsCounter);
		}
	}
}
