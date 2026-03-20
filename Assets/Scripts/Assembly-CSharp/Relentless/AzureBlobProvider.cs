using System;
using UnityEngine;

namespace Relentless
{
	public abstract class AzureBlobProvider<T> : CloudProvider<T> where T : class, new()
	{
		public string ServerUrl;

		public bool UseHttps = true;

		public bool PrependPlatformToFilename;

		public string AccountName;

		public string AccountKey;

		protected override string ProviderName
		{
			get
			{
				return "AzureBlobProvider";
			}
		}

		public override bool DownloadBlobData()
		{
			try
			{
				AzureRequest azureRequest = new AzureRequest(ServerUrl, UseHttps, AccountName, AccountKey);
				string text = Filename;
				if (PrependPlatformToFilename)
				{
					text = string.Concat(Application.platform, text);
				}
				base.BlobData = azureRequest.GetItem<T>(ContainerName, text);
				if (base.BlobData == null)
				{
					UseDefaultData();
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Logging.LogError("Failed to download ServerGameData BLOB: " + ex.ToString());
			}
			UseDefaultData();
			return false;
		}
	}
}
