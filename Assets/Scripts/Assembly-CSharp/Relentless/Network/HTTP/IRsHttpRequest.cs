using System.Collections;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Relentless.Network.HTTP
{
	public interface IRsHttpRequest : IRsHttpRequestHeader
	{
		RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

		LocalCertificateSelectionCallback LocalCertificateSelectionCallback { get; set; }

		X509CertificateCollection ClientCertificates { get; set; }

		SslProtocols SslProtocols { get; set; }

		string RequestText { get; set; }

		string ResponseText { get; }

		bool isDone { get; }

		bool HasErrorOccured { get; }

		string ErrorDescription { get; }

		IEnumerator BlockingSendForCoroutine();

		void BlockingSendForNetThread();
	}
}
