using System;
using System.Linq.Expressions;
using System.Reflection;
using Relentless;
using UnityEngine;

public class DebugUtils
{
	public enum LogColor
	{
		black = 0,
		blue = 1,
		brown = 2,
		cyan = 3,
		darkblue = 4,
		green = 5,
		grey = 6,
		lightblue = 7,
		lime = 8,
		magenta = 9,
		maroon = 10,
		navy = 11,
		olive = 12,
		orange = 13,
		purple = 14,
		red = 15,
		silver = 16,
		teal = 17,
		white = 18,
		yellow = 19
	}

	public static void Assert(bool condition, string msg = "")
	{
		if (condition)
		{
		}
	}

	public static void AssertReference(UnityEngine.Object passed_object)
	{
		Logging.Log(MethodBase.GetCurrentMethod().GetParameters()[0].Name);
		string name = GetName(() => passed_object);
		Assert(passed_object, name + " is NULL");
	}

	public static string GetName<T>(Expression<Func<T>> expr)
	{
		MemberExpression memberExpression = (MemberExpression)expr.Body;
		return memberExpression.Member.Name;
	}

	public static void AssertNotNull(UnityEngine.Object passed_object)
	{
		Assert(passed_object != null, "UnityEngine.Object is null!");
	}

	public static void Log_InCyan(string str)
	{
		Log_InColor(LogColor.cyan, str);
	}

	public static void Log_InGreen(string str)
	{
		Log_InColor(LogColor.green, str);
	}

	public static void Log_InOrange(string str)
	{
		Log_InColor(LogColor.orange, str);
	}

	public static void Log_InMagenta(string str)
	{
		Log_InColor(LogColor.magenta, str);
	}

	public static void Log_InColor(LogColor color, string str)
	{
		Logging.Log("<color=" + color.ToString() + ">" + str + "</color>");
	}
}
