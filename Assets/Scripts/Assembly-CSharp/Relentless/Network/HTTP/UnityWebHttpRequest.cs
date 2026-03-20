using System;
using System.Collections;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using HTTP;
using UnityEngine;

namespace Relentless.Network.HTTP
{
	public class UnityWebHttpRequest : IRsHttpRequest, IRsHttpRequestHeader
	{
		private Request m_internalRequest;

		private string m_url;

		public byte[] bytes
		{
			get
			{
				return m_internalRequest.bytes;
			}
			set
			{
				m_internalRequest.bytes = value;
			}
		}

		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
		{
			get
			{
				return m_internalRequest.RemoteCertificateValidationCallback;
			}
			set
			{
				m_internalRequest.RemoteCertificateValidationCallback = value;
			}
		}

		public LocalCertificateSelectionCallback LocalCertificateSelectionCallback
		{
			get
			{
				return m_internalRequest.LocalCertificateSelectionCallback;
			}
			set
			{
				m_internalRequest.LocalCertificateSelectionCallback = value;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return m_internalRequest.ClientCertificates;
			}
			set
			{
				m_internalRequest.ClientCertificates = value;
			}
		}

		public SslProtocols SslProtocols
		{
			get
			{
				return m_internalRequest.SslProtocols;
			}
			set
			{
				m_internalRequest.SslProtocols = value;
			}
		}

		public bool isDone
		{
			get
			{
				return m_internalRequest.isDone;
			}
		}

		public string RequestText
		{
			get
			{
				return m_internalRequest.Text;
			}
			set
			{
				m_internalRequest.Text = value;
			}
		}

		public string ResponseText
		{
			get
			{
				if (m_internalRequest.response == null)
				{
					return string.Empty;
				}
				return m_internalRequest.response.Text;
			}
		}

		public bool HasErrorOccured
		{
			get
			{
				if (m_internalRequest.exception != null)
				{
					return true;
				}
				if (m_internalRequest.response == null)
				{
					return true;
				}
				if (m_internalRequest.response.status >= 400)
				{
					return true;
				}
				return false;
			}
		}

		public string ErrorDescription
		{
			get
			{
				if (m_internalRequest.exception != null)
				{
					return string.Format("Failed HTTP request {0}\nexception = {1}\ninner = {2}", m_url, m_internalRequest.exception, (m_internalRequest.exception.InnerException == null) ? "(null)" : m_internalRequest.exception.InnerException.ToString());
				}
				if (m_internalRequest.response == null)
				{
					return string.Format("Failed HTTP request {0}\nNo content received.", m_url);
				}
				if (m_internalRequest.response.status >= 400)
				{
					return string.Format("Failed HTTP request {0}\nstatus={1}\nresponse={2}", m_url, m_internalRequest.response.status, (!string.IsNullOrEmpty(m_internalRequest.response.Text)) ? m_internalRequest.response.Text : "(empty)");
				}
				return string.Empty;
			}
		}

		public string Verb
		{
			get
			{
				return m_internalRequest.method;
			}
		}

		public Uri Uri
		{
			get
			{
				return new Uri(m_url);
			}
		}

		public UnityWebHttpRequest(string verb, string uri)
		{
			m_url = uri;
			m_internalRequest = new Request(verb, m_url);
		}

		public void AddHeader(string name, string value)
		{
			m_internalRequest.AddHeader(name, value);
		}

		public string GetHeader(string name)
		{
			return m_internalRequest.GetHeader(name);
		}

		public IEnumerator BlockingSendForCoroutine()
		{
			m_internalRequest.Send();
			while (!m_internalRequest.isDone)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		public void BlockingSendForNetThread()
		{
			m_internalRequest.Send();
			while (!m_internalRequest.isDone)
			{
				Thread.Sleep(100);
			}
		}
	}
}
