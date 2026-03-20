using System.Text.RegularExpressions;

namespace Relentless
{
	public static class StringExtensions
	{
		public static string Truncate(this string originalString, int truncateLength)
		{
			return (originalString.Length <= truncateLength) ? originalString : originalString.Substring(0, truncateLength);
		}

		public static bool MatchesWildcardPattern(this string str, string wildcardPattern)
		{
			string pattern = "^" + Regex.Escape(wildcardPattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
			Regex regex = new Regex(pattern);
			return regex.IsMatch(str);
		}

		public static string ForwardSlashes(this string originalString)
		{
			if (!string.IsNullOrEmpty(originalString))
			{
				originalString = originalString.Replace('\\', '/');
			}
			return originalString;
		}
	}
}
