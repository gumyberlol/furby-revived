using System;
using System.Collections;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Relentless.Network.HTTP;

namespace Relentless
{
	public class RsHttpRequest : IRsHttpRequest, IRsHttpRequestHeader
	{
		private IRsHttpRequest m_aggragateInstance;

		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
		{
			get
			{
				return m_aggragateInstance.RemoteCertificateValidationCallback;
			}
			set
			{
				m_aggragateInstance.RemoteCertificateValidationCallback = value;
			}
		}

		public LocalCertificateSelectionCallback LocalCertificateSelectionCallback
		{
			get
			{
				return m_aggragateInstance.LocalCertificateSelectionCallback;
			}
			set
			{
				m_aggragateInstance.LocalCertificateSelectionCallback = value;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return m_aggragateInstance.ClientCertificates;
			}
			set
			{
				m_aggragateInstance.ClientCertificates = value;
			}
		}

		public SslProtocols SslProtocols
		{
			get
			{
				return m_aggragateInstance.SslProtocols;
			}
			set
			{
				m_aggragateInstance.SslProtocols = value;
			}
		}

		public string RequestText
		{
			get
			{
				return m_aggragateInstance.RequestText;
			}
			set
			{
				m_aggragateInstance.RequestText = value;
			}
		}

		public bool isDone
		{
			get
			{
				return m_aggragateInstance.isDone;
			}
		}

		public string ResponseText
		{
			get
			{
				return m_aggragateInstance.ResponseText;
			}
		}

		public bool HasErrorOccured
		{
			get
			{
				return m_aggragateInstance.HasErrorOccured;
			}
		}

		public string ErrorDescription
		{
			get
			{
				return m_aggragateInstance.ErrorDescription;
			}
		}

		public string Verb
		{
			get
			{
				return m_aggragateInstance.Verb;
			}
		}

		public Uri Uri
		{
			get
			{
				return m_aggragateInstance.Uri;
			}
		}

		public RsHttpRequest(string verb, string uri)
		{
			m_aggragateInstance = new WWWHttpRequest(verb, uri);
		}

		public void AddHeader(string name, string value)
		{
			m_aggragateInstance.AddHeader(name, value);
		}

		public string GetHeader(string name)
		{
			return m_aggragateInstance.GetHeader(name);
		}

		public IEnumerator BlockingSendForCoroutine()
		{
			return m_aggragateInstance.BlockingSendForCoroutine();
		}

		public void BlockingSendForNetThread()
		{
			m_aggragateInstance.BlockingSendForNetThread();
		}
	}
}
