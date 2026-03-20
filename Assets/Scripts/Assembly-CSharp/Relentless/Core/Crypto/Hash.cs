using System;
using System.Security.Cryptography;
using System.Text;

namespace Relentless.Core.Crypto
{
	public class Hash
	{
		public enum Algorithm
		{
			SHA1 = 0,
			SHA256 = 1,
			SHA384 = 2,
			SHA512 = 3,
			MD5 = 4
		}

		public static string ComputeHash(string plainText, Algorithm hashAlgorithm, string saltString)
		{
			return ComputeHash(plainText, hashAlgorithm, Encoding.UTF8.GetBytes(saltString));
		}

		public static string ComputeHash(string plainText, Algorithm hashAlgorithm, byte[] saltBytes)
		{
			if (saltBytes == null)
			{
				int minValue = 4;
				int maxValue = 8;
				Random random = new Random();
				int num = random.Next(minValue, maxValue);
				saltBytes = new byte[num];
				RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
				rNGCryptoServiceProvider.GetNonZeroBytes(saltBytes);
			}
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			byte[] array = new byte[bytes.Length + saltBytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				array[i] = bytes[i];
			}
			for (int j = 0; j < saltBytes.Length; j++)
			{
				array[bytes.Length + j] = saltBytes[j];
			}
			HashAlgorithm hashAlgorithm2;
			switch (hashAlgorithm)
			{
			case Algorithm.SHA1:
				hashAlgorithm2 = new SHA1Managed();
				break;
			case Algorithm.SHA256:
				hashAlgorithm2 = new SHA256Managed();
				break;
			case Algorithm.SHA384:
				hashAlgorithm2 = new SHA384Managed();
				break;
			case Algorithm.SHA512:
				hashAlgorithm2 = new SHA512Managed();
				break;
			default:
				hashAlgorithm2 = new MD5CryptoServiceProvider();
				break;
			}
			byte[] array2 = hashAlgorithm2.ComputeHash(array);
			byte[] array3 = new byte[array2.Length + saltBytes.Length];
			for (int k = 0; k < array2.Length; k++)
			{
				array3[k] = array2[k];
			}
			for (int l = 0; l < saltBytes.Length; l++)
			{
				array3[array2.Length + l] = saltBytes[l];
			}
			return Convert.ToBase64String(array3);
		}

		public static bool VerifyHash(string plainText, Algorithm hashAlgorithm, string hashValue)
		{
			byte[] array = Convert.FromBase64String(hashValue);
			int num;
			switch (hashAlgorithm)
			{
			case Algorithm.SHA1:
				num = 160;
				break;
			case Algorithm.SHA256:
				num = 256;
				break;
			case Algorithm.SHA384:
				num = 384;
				break;
			case Algorithm.SHA512:
				num = 512;
				break;
			default:
				num = 128;
				break;
			}
			int num2 = num / 8;
			if (array.Length < num2)
			{
				return false;
			}
			byte[] array2 = new byte[array.Length - num2];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[num2 + i];
			}
			string text = ComputeHash(plainText, hashAlgorithm, array2);
			return hashValue == text;
		}
	}
}
