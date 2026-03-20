namespace JsonFx.Utils
{
	public static class CharUtility
	{
		public static bool IsNullOrWhiteSpace(string value)
		{
			if (value != null)
			{
				int i = 0;
				for (int length = value.Length; i < length; i++)
				{
					if (!IsWhiteSpace(value[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool IsWhiteSpace(char ch)
		{
			return ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t';
		}

		public static bool IsControl(char ch)
		{
			return ch <= '\u001f' || (ch >= '\u007f' && ch <= '\u009f');
		}

		public static bool IsLetter(char ch)
		{
			return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
		}

		public static bool IsDigit(char ch)
		{
			return ch >= '0' && ch <= '9';
		}

		public static bool IsHexDigit(char ch)
		{
			return (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f');
		}

		public static char GetHexDigit(int i)
		{
			if (i < 10)
			{
				return (char)(i + 48);
			}
			return (char)(i - 10 + 97);
		}

		public static string GetHexString(ulong i)
		{
			string text = string.Empty;
			while (i != 0)
			{
				text = GetHexDigit((int)(i % 16)) + text;
				i >>= 4;
			}
			return text;
		}

		public static int ConvertToUtf32(string value, int index)
		{
			return char.ConvertToUtf32(value, index);
		}

		public static string ConvertFromUtf32(int utf32)
		{
			return char.ConvertFromUtf32(utf32);
		}
	}
}
