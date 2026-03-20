using System;
using System.Collections.Generic;

namespace Relentless
{
	public static class EnumExtensions
	{
		public static Type Parse<Type>(string value) where Type : struct
		{
			return (Type)Enum.Parse(typeof(Type), value, true);
		}

		public static string GetName<Type>(long value) where Type : struct
		{
			return Enum.GetName(typeof(Type), value);
		}

		public static Array Array<Type>()
		{
			return Enum.GetValues(typeof(Type));
		}

		public static IEnumerable<Type> Values<Type>()
		{
			foreach (Type item in Array<Type>())
			{
				yield return item;
			}
		}

		public static int Count<Type>()
		{
			return Array<Type>().Length;
		}
	}
}
