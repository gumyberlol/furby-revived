using System;
using System.Security.Cryptography;
using System.Text;
using Relentless.Network.HTTP;

namespace Relentless.Network.Security
{
	public static class RSAuthHeader
	{
		public static readonly string Version = "1";

		public static string GetAuthorizationHeader(IRsHttpRequestHeader request, string accountName, string accountKey)
		{
			string text = request.GetHeader("x-rs-version");
			if (string.IsNullOrEmpty(text))
			{
				text = Version;
				request.AddHeader("x-rs-version", text);
			}
			string text2 = request.GetHeader("x-rs-date");
			if (string.IsNullOrEmpty(text2))
			{
				string text3 = request.GetHeader("x-ms-date");
				if (string.IsNullOrEmpty(text3))
				{
					text3 = SetupNetworking.ServerTimeFormattedForHttpHeader;
					request.AddHeader("x-ms-date", text3);
				}
				text2 = text3;
				request.AddHeader("x-rs-date", text2);
			}
			string empty = string.Empty;
			string text4 = string.Format("/{0}/{1}", accountName, request.Uri.PathAndQuery);
			string header = request.GetHeader("Content-Encoding");
			string header2 = request.GetHeader("Content-Language");
			string header3 = request.GetHeader("Content-Length");
			if (string.IsNullOrEmpty(header3))
			{
				header3 = request.GetHeader("x-rs-content-length");
			}
			string header4 = request.GetHeader("Content-Md5");
			string header5 = request.GetHeader("Content-Type");
			string header6 = request.GetHeader("Date");
			string header7 = request.GetHeader("If-Modified-Since");
			string header8 = request.GetHeader("If-Match");
			string header9 = request.GetHeader("If-None-Match");
			string header10 = request.GetHeader("If-Unmodified-Since");
			string header11 = request.GetHeader("Range");
			string s = string.Format("{15}\n{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\nx-rs-date:{11}\nx-rs-version:{12}\n{13}{14}", header, header2, header3, header4, header5, header6, header7, header8, header9, header10, header11, text2, text, empty, text4, request.Verb.ToUpper());
			HMACSHA256 hMACSHA = new HMACSHA256(Convert.FromBase64String(accountKey));
			byte[] inArray = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(s));
			string arg = Convert.ToBase64String(inArray);
			return string.Format("SharedKey {0}:{1}", accountName, arg);
		}
	}
}
