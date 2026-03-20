using System;
using System.Text;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless.Network.Analytics.Providers
{
	public class AzureCloudServiceTelemeteryProvider : CloudTelemeteryProvider
	{
		[SerializeField]
		private Servers m_server = Servers.None;

		protected override string ProviderName
		{
			get
			{
				return "AzureCloudServiceTelemetry";
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
				StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_server);
				if (serverDetails == null)
				{
					Logging.LogError("AzureCloudServiceTelemetery: Failed to get server details");
					return false;
				}
				AzureTelemeteryRequestBuilder azureTelemeteryRequestBuilder = new AzureTelemeteryRequestBuilder();
				azureTelemeteryRequestBuilder.StaticRequestDetails = serverDetails;
				AzureTelemeteryRequestBuilder azureTelemeteryRequestBuilder2 = azureTelemeteryRequestBuilder;
				RESTfulApi.RequestDetails requestDetails = new RESTfulApi.RequestDetails();
				requestDetails.ContentType = ContentType.JSON;
				requestDetails.SendClientCertificate = false;
				RESTfulApi.RequestDetails requestDetails2 = requestDetails;
				if (serverDetails.Protocol == Protocol.https)
				{
					requestDetails2.ValidateServerCertificate = ValidateServerCertificate.Check;
					requestDetails2.SslProtocol = serverDetails.ServerProtocol;
				}
				else
				{
					requestDetails2.ValidateServerCertificate = ValidateServerCertificate.None;
				}
				string s = JSONSerialiser.AsString(telemetryEvent);
				string telemeteryDataBase = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
				AzureTelemetryServiceRequestContent azureTelemetryServiceRequestContent = new AzureTelemetryServiceRequestContent();
				azureTelemetryServiceRequestContent.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
				azureTelemetryServiceRequestContent.SessionId = TelemetryManager.SessionId;
				azureTelemetryServiceRequestContent.DeviceId = DeviceIdManager.DeviceId;
				azureTelemetryServiceRequestContent.TelemeteryDataBase64 = telemeteryDataBase;
				AzureTelemetryServiceRequestContent objToSendAsContent = azureTelemetryServiceRequestContent;
				string item = RESTfulApi.GetItem<string, AzureTelemetryServiceRequestContent>(HttpVerb.POST, azureTelemeteryRequestBuilder2.ToString(), objToSendAsContent, requestDetails2);
				if (item == null)
				{
					Logging.LogError("AzureCloudServiceTelemetery: Failed to send");
				}
				return item != null;
			}
			catch (Exception ex)
			{
				Logging.LogError("AzureCloudServiceTelemetery: Failed to post telemetry to server. Caught exception " + ex);
				return false;
			}
		}
	}
}
