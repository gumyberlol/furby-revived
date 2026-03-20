using System;

namespace Eye3.Ware
{
	public static class StringExtensions
	{
		public static string UCFirst(this string aString)
		{
			aString = aString.Trim();
			if (string.IsNullOrEmpty(aString))
			{
				return aString;
			}
			char[] array = aString.ToCharArray();
			array.UCFirst();
			return new string(array);
		}

		public static void UCFirst(this char[] chars)
		{
			if (chars.Length > 0 && char.IsLower(chars[0]))
			{
				chars[0] = char.ToUpper(chars[0]);
			}
		}

		public static string UCWords(this string aString)
		{
			char[] array = aString.ToCharArray();
			array.UCFirst();
			for (int i = 1; i < array.Length; i++)
			{
				if (char.IsWhiteSpace(array[i - 1]) && char.IsLower(array[i]))
				{
					array[i] = char.ToUpper(array[i]);
				}
			}
			return new string(array);
		}

		public static T ToEnum<T>(this string aString) where T : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (string.IsNullOrEmpty(aString))
			{
				return default(T);
			}
			return (T)Enum.Parse(typeof(T), aString, true);
		}
	}
}
