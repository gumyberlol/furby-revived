using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public class ConventionResolverStrategy : PocoResolverStrategy
	{
		public enum WordCasing
		{
			NoChange = 0,
			PascalCase = 1,
			CamelCase = 2,
			Lowercase = 3,
			Uppercase = 4
		}

		public readonly string WordSeparator;

		public readonly WordCasing Casing;

		public ConventionResolverStrategy(WordCasing casing)
			: this(casing, null)
		{
		}

		public ConventionResolverStrategy(WordCasing casing, string wordSeparator)
		{
			WordSeparator = wordSeparator ?? string.Empty;
			Casing = casing;
		}

		public override IEnumerable<DataName> GetName(MemberInfo member)
		{
			string[] words = SplitWords(member.Name);
			if (Casing != WordCasing.NoChange)
			{
				int i = 0;
				for (int length = words.Length; i < length; i++)
				{
					switch (Casing)
					{
					case WordCasing.PascalCase:
					{
						string word2 = words[i];
						if (word2.Length <= 1)
						{
							words[i] = word2.ToUpperInvariant();
						}
						else
						{
							words[i] = char.ToUpperInvariant(word2[0]) + word2.Substring(1).ToLowerInvariant();
						}
						break;
					}
					case WordCasing.CamelCase:
					{
						string word = words[i];
						if (i == 0)
						{
							words[i] = word.ToLowerInvariant();
						}
						else if (word.Length <= 1)
						{
							words[i] = word.ToUpperInvariant();
						}
						else
						{
							words[i] = char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant();
						}
						break;
					}
					case WordCasing.Lowercase:
						words[i] = words[i].ToLowerInvariant();
						break;
					case WordCasing.Uppercase:
						words[i] = words[i].ToUpperInvariant();
						break;
					}
				}
			}
			yield return new DataName(string.Join(WordSeparator, words));
		}

		private string[] SplitWords(string multiword)
		{
			if (string.IsNullOrEmpty(multiword))
			{
				return new string[0];
			}
			List<string> list = new List<string>(5);
			bool flag = true;
			int num = 0;
			int length = multiword.Length;
			for (int i = 0; i < length; i++)
			{
				if (!char.IsLetterOrDigit(multiword, i))
				{
					if (i > num)
					{
						list.Add(multiword.Substring(num, i - num));
					}
					num = i + 1;
					flag = true;
					continue;
				}
				bool flag2 = char.IsLower(multiword, i);
				if (flag)
				{
					if (flag2 && num < i - 1)
					{
						list.Add(multiword.Substring(num, i - num - 1));
						num = i - 1;
					}
				}
				else if (!flag2)
				{
					list.Add(multiword.Substring(num, i - num));
					num = i;
				}
				flag = !flag2;
			}
			int num2 = length - num - 1;
			if ((num2 == 1 && char.IsLetterOrDigit(multiword[num])) || num2 > 1)
			{
				list.Add(multiword.Substring(num));
			}
			return list.ToArray();
		}
	}
}
