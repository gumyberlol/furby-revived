using System;
using System.Collections.Generic;
using System.Text;

namespace Eye3.Ware
{
	public class Util
	{
		public const string randomChars = "ABCDEFGHJKMNPQRSTWXYZ0123456789";

		public static Util instance = new Util();

		public readonly Random rng = new Random();

		public static string RandomString(int length)
		{
			return RandomString(length, null);
		}

		public static string RandomString(int length, string prefix)
		{
			StringBuilder stringBuilder = new StringBuilder(prefix);
			if (prefix != null)
			{
				stringBuilder.Append("-");
			}
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append("ABCDEFGHJKMNPQRSTWXYZ0123456789"[instance.rng.Next("ABCDEFGHJKMNPQRSTWXYZ0123456789".Length)]);
			}
			return stringBuilder.ToString();
		}

		public static string MakeUnique(string newName, string joinWith, List<string> similarNames)
		{
			if (similarNames.Count == 0)
			{
				return newName;
			}
			string builtName = newName;
			int num = 0;
			List<string> list = new List<string>();
			do
			{
				list = similarNames.FindAll((string s) => s.Contains(builtName));
				if (list.Count > 0)
				{
					num = ((num <= 0) ? (list.Count + 1) : (num + 1));
					builtName = newName + joinWith + num;
				}
			}
			while (list.Count > 0);
			return builtName;
		}

		public static string MakeUnique(string newName, List<string> similarNames)
		{
			return MakeUnique(newName, " ", similarNames);
		}
	}
}
