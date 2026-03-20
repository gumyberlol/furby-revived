using System;
using UnityEngine;

namespace Eye3.Ware
{
	public static class UnityEngineObjectExtensions
	{
		public static string GetNiceTypeName(this UnityEngine.Object obj)
		{
			return obj.GetType().GetNiceTypeName();
		}

		public static string GetNiceTypeName(this Type type)
		{
			string text = type.ToString();
			if (text.Contains("."))
			{
				return text.Substring(text.LastIndexOf('.') + 1);
			}
			return text;
		}
	}
}
