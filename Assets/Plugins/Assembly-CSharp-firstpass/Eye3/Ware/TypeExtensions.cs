using System;
using System.Reflection;

namespace Eye3.Ware
{
	internal static class TypeExtensions
	{
		public static bool IsCastableTo(this Type from, Type to)
		{
			if (to.IsAssignableFrom(from))
			{
				return true;
			}
			MethodInfo[] methods = from.GetMethods(BindingFlags.Static | BindingFlags.Public);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if ((methodInfo.ReturnType == to && methodInfo.Name == "op_Implicit") || methodInfo.Name == "op_Explicit")
				{
					return true;
				}
			}
			return false;
		}
	}
}
