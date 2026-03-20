using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

[Serializable]
public class PersistentException
{
	public string message;

	public string fullMessage;

	public string stackTrace;

	public string source;

	public string targetPath;

	public int targetLine = -1;

	public int targetColumn;

	public bool isAssertException;

	public PersistentException()
	{
	}

	public PersistentException(Exception e)
	{
		SetException(e);
	}

	public void SetException(Exception e)
	{
		isAssertException = e is AssertionException;
		message = e.Message;
		string[] array = e.ToString().Trim().Split('\n');
		int num = Array.FindLastIndex(array, (string s) => string.IsNullOrEmpty(s.Trim()));
		fullMessage = string.Join("\n", array.Take(num).ToArray());
		stackTrace = string.Join("\n", array.Skip(num + 1).ToArray());
		StackFrame[] frames = new StackTrace(e, true).GetFrames();
		foreach (StackFrame stackFrame in frames)
		{
			targetPath = stackFrame.GetFileName();
			targetLine = stackFrame.GetFileLineNumber();
			targetColumn = stackFrame.GetFileColumnNumber();
			if (targetPath != null && !targetPath.Contains("Eye3"))
			{
				break;
			}
		}
	}
}
