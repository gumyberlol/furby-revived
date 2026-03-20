using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using JsonFx.Serialization;
using Relentless.Network.Security;

namespace Relentless.Network
{
	public class RESTfulApi
	{
		public class RequestDetails
		{
			public ValidateServerCertificate ValidateServerCertificate { get; set; }

			public bool SendClientCertificate { get; set; }

			public Dictionary<string, string> Headers { get; set; }

			public SslProtocols SslProtocol { get; set; }

			public ContentType ContentType { get; set; }

			public RequestDetails()
			{
				SendClientCertificate = false;
				ValidateServerCertificate = ValidateServerCertificate.Check;
				ContentType = ContentType.JSON;
				SslProtocol = SslProtocols.Ssl3;
			}
		}

		private class ServerContent
		{
			public string ContentType;

			public string Content;
		}

		private static readonly RequestDetails DefaultRequestDetails = new RequestDetails();

		public static T GetItem<T>(string requestUri) where T : class
		{
			return GetItem<T>(requestUri, DefaultRequestDetails);
		}

		public static T GetItem<T>(string requestUri, RequestDetails requestDetails) where T : class
		{
			return GetItem<T, object>(HttpVerb.GET, requestUri, null, requestDetails);
		}

		public static T GetItem<T, U>(HttpVerb verb, string requestUri, U objToSendAsContent, RequestDetails requestDetails) where T : class where U : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return (T)null;
				}
				ServerContent content = SendRequest(verb, requestUri, objToSendAsContent, requestDetails);
				return ParseResult<T>(content, requestUri);
			}
			catch (Exception ex)
			{
				Logging.LogError(string.Format("RESTful: Get Item Failed {0}\r\nexception = {1}\r\ninner = {2}", requestUri, ex, (ex.InnerException == null) ? "(null)" : ex.InnerException.ToString()));
			}
			return (T)null;
		}

		public static List<T> GetItemList<T>(string requestUri) where T : class
		{
			return GetItemList<T>(requestUri, DefaultRequestDetails);
		}

		public static List<T> GetItemList<T>(string requestUri, RequestDetails requestDetails) where T : class
		{
			return GetItemListExtended<T, object>(HttpVerb.GET, requestUri, null, requestDetails);
		}

		public static List<T> GetItemListExtended<T, U>(HttpVerb verb, string requestUri, U objToSendAsContent, RequestDetails requestDetails) where T : class where U : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return null;
				}
				ServerContent content = SendRequest(verb, requestUri, objToSendAsContent, requestDetails);
				return ParseResult<List<T>>(content, requestUri);
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: GetItemList Failed :" + ex.ToString());
			}
			return null;
		}

		public static R PostItem<T, R>(string requestUri, T obj) where T : class where R : class
		{
			return PostItem<T, R>(requestUri, obj, DefaultRequestDetails);
		}

		public static R PostItem<T, R>(string requestUri, T obj, RequestDetails requestDetails) where T : class where R : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return (R)null;
				}
				ServerContent content = SendRequest(HttpVerb.POST, requestUri, obj, requestDetails);
				return ParseResult<R>(content, requestUri);
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: PostItem Failed :" + ex.ToString());
			}
			return (R)null;
		}

		public static bool PostItemList<T>(string requestUri, List<T> obj) where T : class
		{
			return PostItemList(requestUri, obj, DefaultRequestDetails);
		}

		public static bool PostItemList<T>(string requestUri, List<T> obj, RequestDetails requestDetails) where T : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return false;
				}
				ServerContent serverContent = SendRequest(HttpVerb.POST, requestUri, obj, requestDetails);
				return serverContent != null;
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: PostItemList Failed :" + ex.ToString());
			}
			return false;
		}

		public static bool PutItem<T>(string requestUri, T obj) where T : class
		{
			return PutItem(requestUri, obj, DefaultRequestDetails);
		}

		public static bool PutItem<T>(string requestUri, T obj, RequestDetails requestDetails) where T : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return false;
				}
				SendRequest(HttpVerb.PUT, requestUri, obj, requestDetails);
				return true;
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: PutItem Failed :" + ex.ToString());
			}
			return false;
		}

		public static bool PutItemList<T>(string requestUri, List<T> obj) where T : class
		{
			return PutItemList(requestUri, obj, DefaultRequestDetails);
		}

		public static bool PutItemList<T>(string requestUri, List<T> obj, RequestDetails requestDetails) where T : class
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					Logging.LogError("RESTful: empty url");
					return false;
				}
				SendRequest(HttpVerb.PUT, requestUri, obj, requestDetails);
				return true;
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: PutItemList Failed :" + ex.ToString());
			}
			return false;
		}

		public static bool NoOp(string requestUri)
		{
			try
			{
				if (string.IsNullOrEmpty(requestUri))
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Logging.LogError("RESTful: NoOp Failed :" + ex.ToString());
			}
			return false;
		}

		private static U ParseResult<U>(ServerContent content, string requestUri) where U : class
		{
			if (content == null || content.Content == null)
			{
				Logging.LogError("RESTful: Failed to download from " + requestUri + ": Null content so cannot parse result");
				return (U)null;
			}
			Type typeFromHandle = typeof(U);
			if (typeFromHandle == typeof(string))
			{
				return content.Content as U;
			}
			if (string.IsNullOrEmpty(content.Content))
			{
				Logging.LogError("RESTful: Failed to download from " + requestUri + ": Null content so cannot parse result (as an object)");
				return (U)null;
			}
			try
			{
				switch (content.ContentType)
				{
				case "application/json":
					return JSONSerialiser.Parse<U>(content.Content);
				case "application/xml":
					return ParseResultAsXml<U>(content.Content);
				case "application/text":
					Logging.LogError("RESTful: Failed to download from " + requestUri + ": Don't know how to interpret content type: " + content.ContentType + "\r\nReceived:\r\n" + content.Content);
					return (U)null;
				default:
					Logging.LogError("RESTful: Failed to download from " + requestUri + ": Unknown content type: " + content.ContentType + "\r\nReceived:\r\n" + content.Content);
					return (U)null;
				}
			}
			catch (DeserializationException ex)
			{
				string[] array = content.Content.Split('\n');
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Failed to download from " + requestUri + ": DeserializationException:");
				stringBuilder.AppendFormat("{0} (line {1}, column {2}, index {3})", ex.Message, ex.Line, ex.Column, ex.Index);
				if (array.Length > ex.Line)
				{
					stringBuilder.AppendFormat("\r\nline {0}: {1}", ex.Line, array[ex.Line]);
				}
				Logging.LogError("RESTful: " + stringBuilder.ToString());
				return (U)null;
			}
			catch (Exception ex2)
			{
				Logging.LogError("RESTful: Failed to download from " + requestUri + ": Failed to parse content:" + ex2.ToString());
				return (U)null;
			}
		}

		private static U ParseResultAsXml<U>(string content) where U : class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(U));
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreWhitespace = true;
			xmlReaderSettings.IgnoreComments = true;
			XmlReaderSettings settings = xmlReaderSettings;
			using (StringReader reader = new StringReader(content))
			{
				using (XmlReader xmlReader = XmlReader.Create(reader, settings))
				{
					if (!xmlSerializer.CanDeserialize(xmlReader))
					{
						return (U)null;
					}
					return (U)xmlSerializer.Deserialize(xmlReader);
				}
			}
		}

		private static string ContentAsXml<U>(U content) where U : class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(U));
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriterSettings settings = xmlWriterSettings;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
				{
					xmlSerializer.Serialize(xmlWriter, content);
				}
				return stringWriter.ToString();
			}
		}

		private static void UpdateRequestWithSecurityAndHeaders(RsHttpRequest request, RequestDetails requestDetails)
		{
			if (requestDetails == null)
			{
				return;
			}
			if (requestDetails.SendClientCertificate)
			{
				request.LocalCertificateSelectionCallback = ApplicationCentricTrust.SelectClientCertificate;
				request.ClientCertificates = ApplicationCentricTrust.ClientCertificates;
			}
			switch (requestDetails.ValidateServerCertificate)
			{
			case ValidateServerCertificate.Ignore:
				request.RemoteCertificateValidationCallback = ApplicationCentricTrust.IgnoreServerCertificate;
				break;
			case ValidateServerCertificate.Check:
				request.RemoteCertificateValidationCallback = ApplicationCentricTrust.ValidateServerCertificate;
				break;
			}
			if (requestDetails.Headers != null)
			{
				foreach (KeyValuePair<string, string> header in requestDetails.Headers)
				{
					request.AddHeader(header.Key, header.Value);
				}
			}
			if (!string.IsNullOrEmpty(request.RequestText) && string.IsNullOrEmpty(request.GetHeader("Content-Length")))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(request.RequestText);
				request.AddHeader("Content-Length", bytes.Length.ToString());
			}
			AddContentType(requestDetails, request);
			AddDate(requestDetails.Headers, request);
			AddRsAuth(request);
			request.SslProtocols = requestDetails.SslProtocol;
		}

		private static void AddDate(Dictionary<string, string> headers, RsHttpRequest request)
		{
			string serverTimeFormattedForHttpHeader = SetupNetworking.ServerTimeFormattedForHttpHeader;
			if (headers == null)
			{
				request.AddHeader("Date", serverTimeFormattedForHttpHeader);
				request.AddHeader("x-ms-date", serverTimeFormattedForHttpHeader);
				request.AddHeader("x-rs-date", serverTimeFormattedForHttpHeader);
				return;
			}
			if (!headers.ContainsKey("Date"))
			{
				request.AddHeader("Date", serverTimeFormattedForHttpHeader);
			}
			if (!headers.ContainsKey("x-ms-date"))
			{
				request.AddHeader("x-ms-date", serverTimeFormattedForHttpHeader);
			}
			if (!headers.ContainsKey("x-rs-date"))
			{
				request.AddHeader("x-rs-date", serverTimeFormattedForHttpHeader);
			}
		}

		private static void AddRsAuth(RsHttpRequest request)
		{
			if (!string.IsNullOrEmpty(request.Uri.Host) && string.IsNullOrEmpty(request.GetHeader("x-rs-authorization")))
			{
				string authorizationHeader = RSAuthHeader.GetAuthorizationHeader(request, "rsonlineservices", "KmTyOHvyYwxNRWBECmIVaEZej/s1T/CJ9U/hgav8SJssRfu/Kl9F7FswrlTgH4Nd91Hq+aNxbF3RKmoOVUgEwQ==");
				request.AddHeader("x-rs-authorization", authorizationHeader);
			}
		}

		private static void AddContentType(RequestDetails requestDetails, RsHttpRequest request)
		{
			if (requestDetails.Headers == null || !requestDetails.Headers.ContainsKey("Content-Type"))
			{
				switch (requestDetails.ContentType)
				{
				case ContentType.XML:
					request.AddHeader("Content-Type", "application/xml");
					break;
				case ContentType.RAW:
					break;
				default:
					request.AddHeader("Content-Type", "application/json");
					break;
				}
			}
		}

		private static ServerContent SendRequest<U>(HttpVerb verb, string requestUri, U contentToSend, RequestDetails requestDetails) where U : class
		{
			Logging.Log(string.Concat("RESTful: verb = ", verb, ", requestUri = ", requestUri));
			RsHttpRequest rsHttpRequest = new RsHttpRequest(verb.ToString(), requestUri);
			if (verb == HttpVerb.GET && contentToSend != null)
			{
				Logging.LogError("RESTful: Cannot send content using Http verb GET. Content will be ignored to prevent a server error.");
			}
			else if (contentToSend != null)
			{
				switch (requestDetails.ContentType)
				{
				case ContentType.XML:
					rsHttpRequest.RequestText = JSONSerialiser.AsString(contentToSend);
					break;
				case ContentType.RAW:
					if (contentToSend is string)
					{
						rsHttpRequest.RequestText = contentToSend as string;
					}
					else
					{
						rsHttpRequest.RequestText = contentToSend.ToString();
					}
					break;
				default:
					rsHttpRequest.RequestText = JSONSerialiser.AsString(contentToSend);
					break;
				}
			}
			UpdateRequestWithSecurityAndHeaders(rsHttpRequest, requestDetails);
			rsHttpRequest.BlockingSendForNetThread();
			if (rsHttpRequest.HasErrorOccured)
			{
				Logging.LogError("RESTful: Failed to download form " + requestUri + ": " + rsHttpRequest.ErrorDescription);
				return null;
			}
			string content = string.Empty;
			string contentType = "application/text";
			if (!string.IsNullOrEmpty(rsHttpRequest.ResponseText))
			{
				contentType = "application/json";
				content = rsHttpRequest.ResponseText;
			}
			ServerContent serverContent = new ServerContent();
			serverContent.ContentType = contentType;
			serverContent.Content = content;
			ServerContent serverContent2 = serverContent;
			Logging.Log(string.Format("RESTful: SendRequest {0}\r\n{1}", requestUri, serverContent2.Content));
			return serverContent2;
		}
	}
}
