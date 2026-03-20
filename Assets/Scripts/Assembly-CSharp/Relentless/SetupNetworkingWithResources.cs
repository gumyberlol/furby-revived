using System;
using System.Text;
using UnityEngine;

namespace Relentless
{
	public class SetupNetworkingWithResources : SingletonInstance<SetupNetworkingWithResources>
	{
		private bool m_isReady;

		public TextAsset[] ClientCertificates;

		public TextAsset[] ServerThumbprints;

		public bool IsReady
		{
			get
			{
				return m_isReady;
			}
		}

		public void Start()
		{
			try
			{
				TextAsset[] clientCertificates = ClientCertificates;
				foreach (TextAsset textAsset in clientCertificates)
				{
					byte[] rawCertificateData = Convert.FromBase64String(textAsset.text);
					ApplicationCentricTrust.AddClientCertificate(rawCertificateData);
				}
				TextAsset[] serverThumbprints = ServerThumbprints;
				foreach (TextAsset textAsset2 in serverThumbprints)
				{
					string text = textAsset2.text;
					string text2 = Encoding.ASCII.GetString(Convert.FromBase64String(text));
					Logging.Log("Adding server thumbprint " + text2);
					ApplicationCentricTrust.CacheThumbPrint(text2, true);
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("SetupNetworkingFromEditor: Failed to load certificates and thumbprints\n" + ex);
			}
			m_isReady = true;
		}
	}
}
