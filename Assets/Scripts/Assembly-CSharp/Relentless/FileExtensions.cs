using System.IO;

namespace Relentless
{
	public static class FileExtensions
	{
		public static byte[] ReadAllBytes(string path)
		{
			return File.ReadAllBytes(path);
		}

		public static string ReadAllText(string path)
		{
			return File.ReadAllText(path);
		}
	}
}
