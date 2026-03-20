using System;
using System.IO;

namespace Relentless
{
	public static class SimplePersister
	{
		private static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		public static void SaveAsJSON<T>(string filename, T obj) where T : class
		{
			string contents = JSONSerialiser.AsString(obj);
			File.WriteAllText(filename, contents);
		}

		public static T LoadFromJSON<T>(string filename) where T : class
		{
			if (!File.Exists(filename))
			{
				return (T)null;
			}
			string content = FileExtensions.ReadAllText(filename);
			return JSONSerialiser.Parse<T>(content);
		}
	}
}
