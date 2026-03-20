using System;

namespace Relentless.Core.Crypto
{
	public class HashTest
	{
		[STAThread]
		private static void Main()
		{
			string plainText = "myP@5sw0rd";
			string plainText2 = "password";
			string text = Hash.ComputeHash(plainText, Hash.Algorithm.MD5, string.Empty);
			string text2 = Hash.ComputeHash(plainText, Hash.Algorithm.SHA1, string.Empty);
			string text3 = Hash.ComputeHash(plainText, Hash.Algorithm.SHA256, string.Empty);
			string text4 = Hash.ComputeHash(plainText, Hash.Algorithm.SHA384, string.Empty);
			string text5 = Hash.ComputeHash(plainText, Hash.Algorithm.SHA512, string.Empty);
			Console.WriteLine("COMPUTING HASH VALUES\r\n");
			Console.WriteLine("MD5   : {0}", text);
			Console.WriteLine("SHA1  : {0}", text2);
			Console.WriteLine("SHA256: {0}", text3);
			Console.WriteLine("SHA384: {0}", text4);
			Console.WriteLine("SHA512: {0}", text5);
			Console.WriteLine();
			Console.WriteLine("COMPARING PASSWORD HASHES\r\n");
			Console.WriteLine("MD5    (good): {0}", Hash.VerifyHash(plainText, Hash.Algorithm.MD5, text).ToString());
			Console.WriteLine("MD5    (bad) : {0}", Hash.VerifyHash(plainText2, Hash.Algorithm.MD5, text).ToString());
			Console.WriteLine("SHA1   (good): {0}", Hash.VerifyHash(plainText, Hash.Algorithm.SHA1, text2).ToString());
			Console.WriteLine("SHA1   (bad) : {0}", Hash.VerifyHash(plainText2, Hash.Algorithm.SHA1, text2).ToString());
			Console.WriteLine("SHA256 (good): {0}", Hash.VerifyHash(plainText, Hash.Algorithm.SHA256, text3).ToString());
			Console.WriteLine("SHA256 (bad) : {0}", Hash.VerifyHash(plainText2, Hash.Algorithm.SHA256, text3).ToString());
			Console.WriteLine("SHA384 (good): {0}", Hash.VerifyHash(plainText, Hash.Algorithm.SHA384, text4).ToString());
			Console.WriteLine("SHA384 (bad) : {0}", Hash.VerifyHash(plainText2, Hash.Algorithm.SHA384, text4).ToString());
			Console.WriteLine("SHA512 (good): {0}", Hash.VerifyHash(plainText, Hash.Algorithm.SHA512, text5).ToString());
			Console.WriteLine("SHA512 (bad) : {0}", Hash.VerifyHash(plainText2, Hash.Algorithm.SHA512, text5).ToString());
		}
	}
}
