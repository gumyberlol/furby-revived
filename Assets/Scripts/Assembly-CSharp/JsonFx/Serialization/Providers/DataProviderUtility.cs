using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization.Providers
{
	public static class DataProviderUtility
	{
		public const string DefaultContentType = "application/octet-stream";

		public static IEnumerable<string> ParseHeaders(string accept, string contentType)
		{
			string mime;
			foreach (string type in SplitTrim(accept, ','))
			{
				mime = ParseMediaType(type);
				if (!string.IsNullOrEmpty(mime))
				{
					yield return mime;
				}
			}
			mime = ParseMediaType(contentType);
			if (!string.IsNullOrEmpty(mime))
			{
				yield return mime;
			}
		}

		public static string ParseMediaType(string type)
		{
			using (IEnumerator<string> enumerator = SplitTrim(type, ';').GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return string.Empty;
		}

		private static IEnumerable<string> SplitTrim(string source, char ch)
		{
			if (string.IsNullOrEmpty(source))
			{
				yield break;
			}
			int length = source.Length;
			int prev = 0;
			int next = 0;
			while (prev < length && next >= 0)
			{
				next = source.IndexOf(ch, prev);
				if (next < 0)
				{
					next = length;
				}
				string part = source.Substring(prev, next - prev).Trim();
				if (part.Length > 0)
				{
					yield return part;
				}
				prev = next + 1;
			}
		}

		public static string NormalizeExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				return string.Empty;
			}
			return Path.GetExtension(extension);
		}
	}
}
