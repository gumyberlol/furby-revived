using System;
using System.Text;

namespace Relentless
{
	public class SetupNetworkingFromEditor : SingletonInstance<SetupNetworkingFromEditor>
	{
		private bool m_isReady;

		public string[] ClientCertificates;

		public string[] ServerThumbprints;

		public bool IsReady
		{
			get
			{
				return m_isReady;
			}
		}

		public override void Awake()
		{
			base.Awake();
			try
			{
				string[] clientCertificates = ClientCertificates;
				foreach (string s in clientCertificates)
				{
					byte[] rawCertificateData = Convert.FromBase64String(s);
					ApplicationCentricTrust.AddClientCertificate(rawCertificateData);
				}
				string[] serverThumbprints = ServerThumbprints;
				foreach (string text in serverThumbprints)
				{
					string s2 = text;
					string text2 = Encoding.ASCII.GetString(Convert.FromBase64String(s2));
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
