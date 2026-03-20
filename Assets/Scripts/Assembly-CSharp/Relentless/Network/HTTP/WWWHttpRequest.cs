using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Relentless.Core;
using UnityEngine;

namespace Relentless.Network.HTTP
{
	public class WWWHttpRequest : IRsHttpRequest, IRsHttpRequestHeader
	{
		private WWW m_internalRequest;

		private string m_url;

		private string m_verb;

		private string m_error;

		private Dictionary<string, string> m_headers = new Dictionary<string, string>();

		private byte[] m_requestBytes;

		private byte[] m_responseBytes;

		private string m_responseText;

		public byte[] RequestBytes
		{
			get
			{
				return m_requestBytes;
			}
			set
			{
				m_requestBytes = value;
			}
		}

		public byte[] ResponseBytes
		{
			get
			{
				return m_responseBytes;
			}
		}

		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

		public LocalCertificateSelectionCallback LocalCertificateSelectionCallback { get; set; }

		public X509CertificateCollection ClientCertificates { get; set; }

		public SslProtocols SslProtocols { get; set; }

		public bool isDone
		{
			get
			{
				if (m_internalRequest == null)
				{
					return false;
				}
				return m_internalRequest.isDone;
			}
		}

		public string RequestText
		{
			get
			{
				if (m_requestBytes == null)
				{
					return string.Empty;
				}
				return Encoding.UTF8.GetString(m_requestBytes);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					m_requestBytes = null;
				}
				else
				{
					m_requestBytes = Encoding.UTF8.GetBytes(value);
				}
			}
		}

		public string ResponseText
		{
			get
			{
				return m_responseText;
			}
		}

		public bool HasErrorOccured
		{
			get
			{
				if (m_internalRequest == null)
				{
					return true;
				}
				if (!string.IsNullOrEmpty(m_error))
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
				return m_error;
			}
		}

		public string Verb
		{
			get
			{
				return m_verb;
			}
		}

		public Uri Uri
		{
			get
			{
				return new Uri(m_url);
			}
		}

		public WWWHttpRequest(string verb, string uri)
		{
			SslProtocols = SslProtocols.Default;
			m_url = uri;
			m_verb = verb.ToUpper();
		}

		public void AddHeader(string name, string value)
		{
			name = name.Trim();
			string key = name.ToLower();
			value = value.Trim();
			if (m_headers.ContainsKey(name))
			{
				m_headers[name] = value;
			}
			else if (m_headers.ContainsKey(key))
			{
				m_headers[key] = value;
			}
			else
			{
				m_headers.Add(name, value);
			}
		}

		public string GetHeader(string name)
		{
			name = name.Trim();
			string key = name.ToLower();
			if (m_headers.ContainsKey(name))
			{
				return m_headers[name];
			}
			if (m_headers.ContainsKey(key))
			{
				return m_headers[key];
			}
			return string.Empty;
		}

		public IEnumerator BlockingSendForCoroutine()
		{
			return BlockingCoroutine();
		}

		public void BlockingSendForNetThread()
		{
			CoroutineMethod coroutine = BlockingCoroutine;
			CoroutineManager.Add(coroutine);
			while (CoroutineManager.StillWaitingFor(coroutine))
			{
				Thread.Sleep(100);
			}
		}

		public IEnumerator BlockingCoroutine()
{
    ServicePointManager.ServerCertificateValidationCallback = ApplicationCentricTrust.IgnoreServerCertificate;
    switch (m_verb)
    {
    case "GET":
        m_internalRequest = new WWW(m_url);
        break;
    case "POST":
        m_internalRequest = new WWW(m_url, m_requestBytes, m_headers);
        break;
    default:
        throw new Exception("HTTP verb " + m_verb + " not supported!");
    }
    yield return m_internalRequest;
    m_error = m_internalRequest.error;
    if (string.IsNullOrEmpty(m_error))
    {
        m_responseBytes = m_internalRequest.bytes;
        m_responseText = m_internalRequest.text;
    }
}
	}
}
