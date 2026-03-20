using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Relentless.Network;

namespace Relentless
{
	public class AzureRequest
	{
		public string ServerUrl { get; private set; }

		public bool UseHttps { get; private set; }

		public string AccountName { get; private set; }

		public string AccountKey { get; private set; }

		public AzureRequest(string serverUrl, bool useHttps, string accountName, string accountKey)
		{
			ServerUrl = serverUrl;
			UseHttps = useHttps;
			AccountName = accountName;
			AccountKey = accountKey;
		}

		public T GetItem<T>(string containerName, string filename) where T : class, new()
		{
			try
			{
				string resourcePath = GetResourcePath(filename, containerName);
				string text = string.Format("{0}://{1}/{2}", (!UseHttps) ? "http" : "https", ServerUrl, resourcePath);
				Uri uri = new Uri(text);
				string serverTimeFormattedForHttpHeader = SetupNetworking.ServerTimeFormattedForHttpHeader;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("x-ms-date", serverTimeFormattedForHttpHeader);
				dictionary.Add("x-ms-version", "2012-02-12");
				dictionary.Add("content-type", "application/json");
				dictionary.Add("accept-encoding", "gzip");
				dictionary.Add("host", uri.Host);
				Dictionary<string, string> headers = dictionary;
				RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
				requestDetails.Headers = headers;
				requestDetails.SendClientCertificate = false;
				requestDetails.ValidateServerCertificate = ValidateServerCertificate.Check;
				requestDetails.SslProtocol = SslProtocols.Ssl3;
				requestDetails.ContentType = ContentType.JSON;
				RESTfulApi.RequestDetails requestDetails2 = requestDetails;
				if (!string.IsNullOrEmpty(AccountName))
				{
					AddBlobAuthorizationHeader(headers, resourcePath);
				}
				return RESTfulApi.GetItem<T>(text, requestDetails2);
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to download item from Azure server: " + ex.ToString());
			}
			return (T)null;
		}

		public string PutTableEntity<T>(string tablename, T content) where T : class, new()
		{
			try
			{
				string text = string.Format("{0}://{1}/{2}", (!UseHttps) ? "http" : "https", ServerUrl, tablename);
				Uri uri = new Uri(text);
				string serverTimeFormattedForHttpHeader = SetupNetworking.ServerTimeFormattedForHttpHeader;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("x-ms-date", serverTimeFormattedForHttpHeader);
				dictionary.Add("x-ms-version", "2012-02-12");
				dictionary.Add("content-type", "application/xml");
				dictionary.Add("accept-encoding", "gzip");
				dictionary.Add("host", uri.Host);
				Dictionary<string, string> headers = dictionary;
				RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
				requestDetails.Headers = headers;
				requestDetails.SendClientCertificate = false;
				requestDetails.ValidateServerCertificate = ValidateServerCertificate.Check;
				requestDetails.SslProtocol = SslProtocols.Ssl3;
				requestDetails.ContentType = ContentType.RAW;
				RESTfulApi.RequestDetails requestDetails2 = requestDetails;
				if (!string.IsNullOrEmpty(AccountName))
				{
					AddBlobAuthorizationHeader(headers, tablename);
				}
				return RESTfulApi.PostItem<T, string>(text, content, requestDetails2);
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to download item from Azure server: " + ex.ToString());
			}
			return null;
		}

		private string GetResourcePath(string filename, string containerName)
		{
			string empty = string.Empty;
			empty = ((!string.IsNullOrEmpty(containerName)) ? Path.Combine(containerName, filename) : filename);
			return empty.Replace('\\', '/');
		}

		public static void AddAuthorizationHeader(string method, Dictionary<string, string> headers, string resourcePath, string accountName, string accountKey)
		{
			string text = headers["x-ms-date"];
			string text2 = headers["x-ms-version"];
			string empty = string.Empty;
			string text3 = string.Format("/{0}/{1}", accountName, resourcePath);
			string header = GetHeader("content-encoding", headers);
			string header2 = GetHeader("content-language", headers);
			string header3 = GetHeader("content-length", headers);
			string header4 = GetHeader("content-md5", headers);
			string header5 = GetHeader("content-type", headers);
			string header6 = GetHeader("date", headers);
			string header7 = GetHeader("if-modified-since", headers);
			string header8 = GetHeader("if-match", headers);
			string header9 = GetHeader("if-none-match", headers);
			string header10 = GetHeader("if-unmodified-since", headers);
			string header11 = GetHeader("range", headers);
			string text4 = string.Format("{15}\n{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\nx-ms-date:{11}\nx-ms-version:{12}\n{13}{14}", header, header2, header3, header4, header5, header6, header7, header8, header9, header10, header11, text, text2, empty, text3, method);
			Logging.Log("rawSignature = " + text4);
			HMACSHA256 hMACSHA = new HMACSHA256(Convert.FromBase64String(accountKey));
			byte[] inArray = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(text4));
			string arg = Convert.ToBase64String(inArray);
			string value = string.Format("SharedKey {0}:{1}", accountName, arg);
			headers.Add("authorization", value);
		}

		private void AddBlobAuthorizationHeader(Dictionary<string, string> headers, string resourcePath)
		{
			string text = headers["x-ms-date"];
			string text2 = headers["x-ms-version"];
			string empty = string.Empty;
			string text3 = string.Format("/{0}/{1}", AccountName, resourcePath);
			string header = GetHeader("content-encoding", headers);
			string header2 = GetHeader("content-language", headers);
			string header3 = GetHeader("content-length", headers);
			string header4 = GetHeader("content-md5", headers);
			string header5 = GetHeader("content-type", headers);
			string header6 = GetHeader("date", headers);
			string header7 = GetHeader("if-modified-since", headers);
			string header8 = GetHeader("if-match", headers);
			string header9 = GetHeader("if-none-match", headers);
			string header10 = GetHeader("if-unmodified-since", headers);
			string header11 = GetHeader("range", headers);
			string s = string.Format("GET\n{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\nx-ms-date:{11}\nx-ms-version:{12}\n{13}{14}", header, header2, header3, header4, header5, header6, header7, header8, header9, header10, header11, text, text2, empty, text3);
			byte[] key = Convert.FromBase64String(AccountKey);
			HMACSHA256 hMACSHA = new HMACSHA256(key);
			byte[] inArray = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(s));
			string arg = Convert.ToBase64String(inArray);
			string value = string.Format("SharedKey {0}:{1}", AccountName, arg);
			headers.Add("authorization", value);
		}

		private static string GetHeader(string header, Dictionary<string, string> headers)
		{
			if (headers.ContainsKey(header))
			{
				return headers[header];
			}
			return string.Empty;
		}
	}
}
