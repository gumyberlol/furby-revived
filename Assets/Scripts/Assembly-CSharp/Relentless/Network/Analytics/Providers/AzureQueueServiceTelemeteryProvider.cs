using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace Relentless.Network.Analytics.Providers
{
	public class AzureQueueServiceTelemeteryProvider : CloudTelemeteryProvider
	{
		public string ServerUrl;

		public string QueueName;

		protected override string ProviderName
		{
			get
			{
				return "AzureQueueServiceTelemetry";
			}
		}

		protected override bool ReallyLogEventToServer(QueuedTelemetryEvent telemetryEvent)
		{
			try
			{
				if (!base.ReallyLogEventToServer(telemetryEvent))
				{
					return false;
				}
				string text = ServerUrl + "/" + QueueName + "/messages";
				string serverTimeFormattedForHttpHeader = SetupNetworking.ServerTimeFormattedForHttpHeader;
				string s = JSONSerialiser.AsString(telemetryEvent);
				string arg = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
				string text2 = string.Format("<QueueMessage><MessageText>{0}</MessageText></QueueMessage>", arg);
				Uri uri = new Uri(text);
				byte[] bytes = Encoding.UTF8.GetBytes(text2);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("date", string.Empty);
				dictionary.Add("x-ms-date", serverTimeFormattedForHttpHeader);
				dictionary.Add("x-ms-version", "2012-02-12");
				dictionary.Add("content-type", "application/xml");
				dictionary.Add("host", uri.Host);
				dictionary.Add("content-length", bytes.Length.ToString());
				Dictionary<string, string> headers = dictionary;
				RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
				requestDetails.Headers = headers;
				requestDetails.SendClientCertificate = false;
				requestDetails.ValidateServerCertificate = ValidateServerCertificate.Ignore;
				requestDetails.SslProtocol = SslProtocols.Ssl3;
				requestDetails.ContentType = ContentType.RAW;
				RESTfulApi.RequestDetails requestDetails2 = requestDetails;
				AzureRequest.AddAuthorizationHeader("POST", headers, "bluetoadtelemetry/messages", "relentlessbt01miap", "KmTyOHvyYwxNRWBECmIVaEZej/s1T/CJ9U/hgav8SJssRfu/Kl9F7FswrlTgH4Nd91Hq+aNxbF3RKmoOVUgEwQ==");
				string text3 = RESTfulApi.PostItem<string, string>(text, text2, requestDetails2);
				return text3 != null;
			}
			catch (Exception ex)
			{
				Logging.LogError("AzureCloudQueueTelemetery: Failed to post telemetry to server. Caught exception " + ex);
				return false;
			}
		}
	}
}
