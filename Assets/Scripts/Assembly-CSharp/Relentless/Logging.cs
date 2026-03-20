using System;
using UnityEngine;

namespace Relentless
{
	public static class Logging
	{
		public static void Log(object message)
		{
			Log(message, null);
		}

		public static void Log(object message, UnityEngine.Object context)
		{
		}

		public static void LogWarning(object message)
		{
		}

		public static void LogWarning(object message, UnityEngine.Object context)
		{
		}

		public static void LogError(object message)
		{
			LogError(message, null);
		}

		public static void LogError(object message, UnityEngine.Object context)
		{
			Debug.LogError(message, context);
		}

		public static void LogException(Exception exception)
		{
			LogException(exception, null);
		}

		public static void LogException(Exception exception, UnityEngine.Object context)
		{
			Debug.LogException(exception, context);
		}
	}
}
