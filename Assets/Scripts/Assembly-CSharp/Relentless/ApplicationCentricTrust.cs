using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Relentless
{
	public static class ApplicationCentricTrust
	{
		private static object Lock = new object();

		private static readonly Dictionary<string, bool> s_serverCheckStore = new Dictionary<string, bool>();

		private static readonly List<X509Certificate2> s_clientCertificateStore = new List<X509Certificate2>();

		public static X509CertificateCollection ClientCertificates
		{
			get
			{
				X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
				foreach (X509Certificate2 item in s_clientCertificateStore)
				{
					x509Certificate2Collection.Add(item);
				}
				return x509Certificate2Collection;
			}
		}

		public static X509Certificate SelectClientCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			lock (Lock)
			{
				Logging.Log("SelectClientCertificate");
				if (acceptableIssuers != null && acceptableIssuers.Length > 0)
				{
					if (localCertificates != null && localCertificates.Count > 0)
					{
						foreach (X509Certificate localCertificate in localCertificates)
						{
							string issuer = localCertificate.Issuer;
							if (Array.IndexOf(acceptableIssuers, issuer) != -1)
							{
								Logging.Log("Found a valid local client certificate : issuer = " + issuer);
								return localCertificate;
							}
						}
					}
					foreach (X509Certificate2 item in s_clientCertificateStore)
					{
						string issuer2 = item.Issuer;
						if (Array.IndexOf(acceptableIssuers, issuer2) != -1)
						{
							Logging.Log("Found a valid cached client certificate : issuer = " + issuer2);
							return item;
						}
					}
				}
				if (s_clientCertificateStore != null && s_clientCertificateStore.Count > 0)
				{
					Logging.Log("Found a valid client certificate (in store)");
					return s_clientCertificateStore[0];
				}
				if (localCertificates != null && localCertificates.Count > 0)
				{
					Logging.Log("Found a valid client certificate (in localCertificates)");
					return localCertificates[0];
				}
				Logging.LogError("Failed to find a valid client certificate");
				return null;
			}
		}

		public static bool IgnoreServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Logging.LogError("IgnoreServerCertificate");
			return true;
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			lock (Lock)
			{
				if (sslPolicyErrors == SslPolicyErrors.None)
				{
					return true;
				}
				string certHashString = certificate.GetCertHashString();
				if (ValidateThumbPrint(certHashString))
				{
					return true;
				}
				string text = Convert.ToBase64String(Encoding.ASCII.GetBytes(certHashString));
				string text2 = string.Format("ThumbPrint: {0} ({1})\nExpiration Date:{2}\nIssuer:{3}\nSubject:{4}", certHashString, text, certificate.GetExpirationDateString(), certificate.Issuer, certificate.Subject);
				Logging.LogError("Failed to find thumbprint for Server certificate\n" + text2);
				Logging.LogError("Failed to validate server certificate");
				return false;
			}
		}

		public static void CacheThumbPrint(string thumbprint, bool shouldTrust)
		{
			try
			{
				lock (Lock)
				{
					string key = thumbprint.ToLower();
					if (!s_serverCheckStore.ContainsKey(key))
					{
						s_serverCheckStore.Add(key, shouldTrust);
					}
					else
					{
						s_serverCheckStore[key] = shouldTrust;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to cache thumbprint:" + ex.ToString());
			}
		}

		public static void AddClientCertificate(byte[] rawCertificateData)
		{
			try
			{
				lock (Lock)
				{
					X509Certificate2 item = new X509Certificate2(rawCertificateData, "Passw0rd3");
					s_clientCertificateStore.Add(item);
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to add certificate:" + ex.ToString());
			}
		}

		private static bool ValidateThumbPrint(string thumbprint)
		{
			lock (Lock)
			{
				string key = thumbprint.ToLower();
				if (s_serverCheckStore.ContainsKey(key))
				{
					return s_serverCheckStore[key];
				}
				return false;
			}
		}
	}
}
