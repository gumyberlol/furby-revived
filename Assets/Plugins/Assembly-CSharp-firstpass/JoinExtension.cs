using System;
using System.Collections.Generic;
using System.IO;

public static class JoinExtension
{
	public static string PathJoin(this List<string> pathParts)
	{
		return pathParts.ToArray().PathJoin();
	}

	public static string PathJoin(this string[] pathParts)
	{
		if (pathParts.Length < 2)
		{
			throw new ArgumentOutOfRangeException("Cannot join sequence with less than 2 items");
		}
		string text = pathParts[0];
		for (int i = 1; i < pathParts.Length; i++)
		{
			text = Path.Combine(text, pathParts[i]);
		}
		return text;
	}
}
