using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Relentless.Core.DesignPatterns.ProviderManager;
using Relentless.Network;
using Relentless.Network.Analytics;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless
{
	public class InAppPurchaseServer : SingletonInstance<InAppPurchaseServer>
	{
		private class ValidateProductParameters
		{
			public ValidateProductHandler Callback;

			public string UserId { get; set; }

			public string ProductId { get; set; }

			public string StoreSpecificProductId { get; set; }

			public string ReceiptBase64 { get; set; }

			public bool? Result { get; set; }
		}

		public delegate void ProductListUpdatedHandler();

		public delegate void ProcessProductHandler(ProductDetails product);

		public delegate void ValidateProductHandler(string productId, bool? result);

		private static object m_lock = new object();

		private static bool m_isReady = false;

		private List<ProductDetails> m_validProducts = new List<ProductDetails>();

		private Thread m_downloadProductListThread;

		private bool m_productListDownloaded;

		private Thread m_validateProductThread;

		private string m_osVer;

		private Platforms m_serverPlatform;

		private ProfileFriendlyName m_friendlyProfileName;

		private ProfileType m_profileType;

		private Resolution m_resolution;

		private string m_gameVersion;

		private string m_sysInfo;

		private SystemLanguage m_language;

		private string m_languageCode;

		private string m_currentLocale;

		private string m_coutryCode;

		private StoreNames m_storeName = StoreNames.DummyStore;

		[SerializeField]
		private Servers m_server = Servers.None;

		[SerializeField]
		private StoreNames m_editorStore = StoreNames.DummyStore;

		private string m_bundleIdentifier = string.Empty;

		[SerializeField]
		private bool m_automaticDownloadOfProductList = true;

		private bool m_useSandbox = true;

		public static bool IsReady
		{
			get
			{
				lock (m_lock)
				{
					return m_isReady;
				}
			}
			private set
			{
				lock (m_lock)
				{
					m_isReady = value;
				}
			}
		}

		public bool ProductListDownloaded
		{
			get
			{
				lock (m_lock)
				{
					return m_productListDownloaded;
				}
			}
		}

		public StoreNames EditorStore
		{
			get
			{
				lock (m_lock)
				{
					return m_editorStore;
				}
			}
		}

		public event ProductListUpdatedHandler ProductListUpdated;

		private void FireProductListUpdated()
		{
			ProductListUpdatedHandler productListUpdated = this.ProductListUpdated;
			if (productListUpdated != null)
			{
				productListUpdated();
			}
		}

		public override void Awake()
		{
			base.Awake();
			StartCoroutine(InitialiseCoroutine());
		}

		public void StartDownloadingProductList()
		{
			StartCoroutine(DownloadProductListCoroutine());
		}

		public void IterateProducts(ProcessProductHandler productHandler)
		{
			if (productHandler == null)
			{
				Logging.LogError("productHandler == null");
				return;
			}
			if (!m_productListDownloaded)
			{
				Logging.LogError("InAppPurchaseServer: Product list not yet downloaded", this);
				return;
			}
			lock (m_lock)
			{
				foreach (ProductDetails validProduct in m_validProducts)
				{
					productHandler(validProduct);
				}
			}
		}

		public bool? IsProductAvaliable(string productId)
		{
			try
			{
				lock (m_lock)
				{
					foreach (ProductDetails validProduct in m_validProducts)
					{
						bool flag = string.Compare(validProduct.Name, productId, StringComparison.OrdinalIgnoreCase) == 0;
						if (!flag && validProduct.ListOfBundledProducts != null)
						{
							foreach (string listOfBundledProduct in validProduct.ListOfBundledProducts)
							{
								if (string.Compare(listOfBundledProduct, productId, StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							if (validProduct.IsComingSoon)
							{
								return false;
							}
							if (!validProduct.IsAvailable)
							{
								return false;
							}
							return true;
						}
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("InAppPurchaseServer: IsProductAvaliable failed: " + ex.ToString());
			}
			return null;
		}

		public ProductDetails GetProduct(string productId)
		{
			try
			{
				lock (m_lock)
				{
					foreach (ProductDetails validProduct in m_validProducts)
					{
						bool flag = string.Compare(validProduct.Name, productId, StringComparison.OrdinalIgnoreCase) == 0;
						if (!flag && validProduct.ListOfBundledProducts != null)
						{
							foreach (string listOfBundledProduct in validProduct.ListOfBundledProducts)
							{
								if (string.Compare(listOfBundledProduct, productId, StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							return validProduct;
						}
					}
					return null;
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("InAppPurchaseServer: GetProduct failed: " + ex.ToString());
			}
			return null;
		}

		private string GetStoreSpecificProductId(string productId)
		{
			try
			{
				lock (m_lock)
				{
					foreach (ProductDetails validProduct in m_validProducts)
					{
						bool flag = string.Compare(validProduct.Name, productId, StringComparison.OrdinalIgnoreCase) == 0;
						if (!flag && validProduct.ListOfBundledProducts != null)
						{
							foreach (string listOfBundledProduct in validProduct.ListOfBundledProducts)
							{
								if (string.Compare(listOfBundledProduct, productId, StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag || validProduct.IsComingSoon || !validProduct.IsAvailable)
						{
							continue;
						}
						return validProduct.StoreSpecificProductId;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("InAppPurchaseServer: GetStoreSpecificProductId failed: " + ex.ToString());
			}
			return string.Empty;
		}

		public void StartValidatingProduct(string userId, string productId, string receiptBase64, ValidateProductHandler callback)
		{
			string storeSpecificProductId = GetStoreSpecificProductId(productId);
			ValidateProductParameters validateProductParameters = new ValidateProductParameters();
			validateProductParameters.UserId = userId;
			validateProductParameters.ProductId = productId;
			validateProductParameters.StoreSpecificProductId = storeSpecificProductId;
			validateProductParameters.ReceiptBase64 = receiptBase64;
			validateProductParameters.Result = null;
			validateProductParameters.Callback = callback;
			ValidateProductParameters value = validateProductParameters;
			StartCoroutine("ValidateProductCoroutine", value);
		}

		public float GetPriceValue_FromDatabase(ProductDetails product, string currencyCode)
		{
			if (product == null || string.IsNullOrEmpty(currencyCode))
			{
				return 0f;
			}
			float num = 0f;
			switch (currencyCode)
			{
			case "USD":
				return product.PriceDetails.USD;
			case "EUR":
				return product.PriceDetails.EUR;
			case "CAD":
				return product.PriceDetails.CAD;
			case "SEK":
				return product.PriceDetails.SEK;
			case "NOK":
				return product.PriceDetails.NOK;
			case "DKK":
				return product.PriceDetails.DKK;
			case "JPY":
				return product.PriceDetails.JPY;
			case "HKD":
				return product.PriceDetails.HKD;
			case "AUD":
				return product.PriceDetails.AUD;
			default:
				return product.PriceDetails.GBP;
			}
		}

		public string SelectCurrencyCode()
		{
			return "USD";
		}

		public string GetLocalizedPrice_FromProvider(string productID)
		{
			string text = SingletonInstance<RsStoreMediator>.Instance.GetPurchaseItemPriceForDisplay(productID);
			if ((text == null) | (text == string.Empty))
			{
				text = string.Empty;
			}
			return text;
		}

		public bool HasItemBeenPurchased(string productID)
		{
			return SingletonInstance<RsStoreMediator>.Instance.HasItemBeenPurchased(productID);
		}

		private IEnumerator InitialiseCoroutine()
		{
			m_productListDownloaded = false;
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			SetupNetworking.GetServerDetails(Servers.Kepler);
			m_osVer = NetworkHelper.OSVer;
			m_serverPlatform = NetworkHelper.ServerPlatform;
			m_friendlyProfileName = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName;
			m_profileType = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType;
			m_resolution = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution;
			m_gameVersion = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleVersion;
			m_sysInfo = NetworkHelper.GetSysInfo();
			m_bundleIdentifier = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.bundleIdentifier;
			m_useSandbox = SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.StoreSandboxEnabled;
			m_coutryCode = Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode;
			ProviderDevices currentDevice = ProviderDevicesHelper.GetDevice();
			if (currentDevice == ProviderDevices.Editor)
			{
				m_storeName = m_editorStore;
			}
			else
			{
				switch (Application.platform)
				{
				case RuntimePlatform.Android:
					if (currentDevice != ProviderDevices.Kindle)
					{
						m_storeName = StoreNames.GooglePlay;
					}
					else
					{
						m_storeName = StoreNames.AmazonStore;
					}
					break;
				case RuntimePlatform.IPhonePlayer:
					m_storeName = StoreNames.AppStore;
					break;
				}
			}
			if (m_automaticDownloadOfProductList)
			{
				StartDownloadingProductList();
			}
			else
			{
				IsReady = true;
			}
		}

		private IEnumerator DownloadProductListCoroutine()
		{
			m_productListDownloaded = false;
			m_language = NetworkHelper.GetSystemLanguage(Singleton<Localisation>.Instance.CurrentLocale);
			m_languageCode = NetworkHelper.GetLanguageCode_FromLocale(Singleton<Localisation>.Instance.CurrentLocale);
			m_currentLocale = Singleton<Localisation>.Instance.CurrentLocale.ToString();
			Logging.Log("InAppPurchaseServer: DownloadProductList started");
			if (SetupNetworking.IsNetworkingEnabled)
			{
				while (m_downloadProductListThread != null)
				{
					yield return new WaitForEndOfFrame();
				}
				m_downloadProductListThread = new Thread(DownloadProductListThread);
				m_downloadProductListThread.Start();
				yield return new WaitForEndOfFrame();
				while (m_downloadProductListThread.IsAlive)
				{
					yield return new WaitForEndOfFrame();
				}
				m_downloadProductListThread = null;
			}
			else
			{
				Logging.LogWarning("InAppPurchaseServer: Skipping download of ProductList from cloud since networking is disabled", this);
			}
			if (!m_productListDownloaded)
			{
				Logging.LogError("InAppPurchaseServer: Failed to download ProductList from cloud.", this);
				DebugNotifications.AddNotification("Failed to download ProductList from the cloud.", TimeSpan.FromSeconds(15.0));
			}
			else
			{
				FireProductListUpdated();
			}
			Logging.Log("InAppPurchaseServer: DownloadProductList complete", this);
			IsReady = true;
			yield return null;
		}

		private void DownloadProductListThread()
		{
			try
			{
				m_productListDownloaded = DownloadProductList();
			}
			catch (Exception ex)
			{
				m_productListDownloaded = false;
				Logging.LogError("InAppPurchaseServer: Failed to download ProductList : " + ex.ToString(), this);
			}
		}

		private bool DownloadProductList()
		{
			StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_server);
			if (serverDetails == null)
			{
				return false;
			}
			ProductDetailsRequestBuilder productDetailsRequestBuilder = new ProductDetailsRequestBuilder();
			productDetailsRequestBuilder.StaticRequestDetails = serverDetails;
			productDetailsRequestBuilder.OS = m_serverPlatform;
			productDetailsRequestBuilder.OSVer = m_osVer;
			productDetailsRequestBuilder.StoreName = m_storeName;
			ProductDetailsRequestBuilder productDetailsRequestBuilder2 = productDetailsRequestBuilder;
			string requestUri = productDetailsRequestBuilder2.ToString();
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
			ProductDetailsRequest productDetailsRequest = new ProductDetailsRequest();
			productDetailsRequest.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
			productDetailsRequest.SessionId = TelemetryManager.SessionId;
			productDetailsRequest.DeviceId = DeviceIdManager.DeviceId;
			productDetailsRequest.ProfileFriendlyName = m_friendlyProfileName;
			productDetailsRequest.ProfileType = m_profileType;
			productDetailsRequest.Resolution = m_resolution;
			productDetailsRequest.SysInfo = m_sysInfo;
			productDetailsRequest.VersionNumber = m_gameVersion;
			productDetailsRequest.Language = m_language;
			productDetailsRequest.LanguageCode = m_languageCode;
			productDetailsRequest.Locale = m_currentLocale;
			productDetailsRequest.CoutryCode = m_coutryCode;
			ProductDetailsRequest productDetailsRequest2 = productDetailsRequest;
			ProductDetailsResponse item = RESTfulApi.GetItem<ProductDetailsResponse, ProductDetailsRequest>(HttpVerb.POST, requestUri, productDetailsRequest2, requestDetails2);
			if (item == null)
			{
				Logging.LogError("InAppPurchaseServer:  Did not receive a response from server.", this);
				return false;
			}
			if (string.IsNullOrEmpty(item.RequestId))
			{
				Logging.LogError("InAppPurchaseServer:  Did not receive a RequestId back from server. Not trusting this response.", this);
				return false;
			}
			if (string.CompareOrdinal(item.RequestId, productDetailsRequest2.RequestId) != 0)
			{
				Logging.LogError("InAppPurchaseServer:  Received a different RequestId back from server than the one we sent. Not trusting this response.", this);
				return false;
			}
			if (item.ProductDetails == null)
			{
				Logging.LogError("InAppPurchaseServer:  Received an invalid response object. Not trusting this response.", this);
				return false;
			}
			RegisterProducts(item.ProductDetails);
			Logging.Log("InAppPurchaseServer:  Got " + item.ProductDetails.Count + " products", this);
			return true;
		}

		private void RegisterProducts(IEnumerable<ProductDetails> productsReceived)
		{
			lock (m_lock)
			{
				m_validProducts.Clear();
				foreach (ProductDetails item in productsReceived)
				{
					if (!IsStoreItemIntendedForThisApp(item.StoreSpecificProductId))
					{
						Logging.LogWarning(string.Format("InAppPurchaseServer: Received an item that is intended for a different application (item {0} not for app {1}).", item.StoreSpecificProductId, m_bundleIdentifier), this);
						continue;
					}
					Logging.Log("InAppPurchaseServer: name = " + item.Name + ", StoreSpecificProductId = " + item.StoreSpecificProductId, this);
					m_validProducts.Add(item);
				}
			}
		}

		private bool IsStoreItemIntendedForThisApp(string storeSpecificProductId)
		{
			if (string.IsNullOrEmpty(m_bundleIdentifier) || string.IsNullOrEmpty(storeSpecificProductId))
			{
				return true;
			}
			if (!storeSpecificProductId.StartsWith(m_bundleIdentifier, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			string text = storeSpecificProductId.Substring(m_bundleIdentifier.Length);
			if (!text.StartsWith("."))
			{
				return false;
			}
			int startIndex = 0;
			int num = 0;
			while ((startIndex = text.IndexOf(".", startIndex)) >= 0)
			{
				num++;
				startIndex++;
			}
			if (num > 1)
			{
				return false;
			}
			return true;
		}

		private IEnumerator ValidateProductCoroutine(object parameters)
		{
			ValidateProductParameters validateProductParameters = (ValidateProductParameters)parameters;
			Logging.Log("InAppPurchaseServer: Validate Product started");
			if (SetupNetworking.IsNetworkingEnabled)
			{
				while (m_validateProductThread != null)
				{
					yield return new WaitForEndOfFrame();
				}
				m_validateProductThread = new Thread(ValidateProductThread);
				m_validateProductThread.Start(validateProductParameters);
				yield return new WaitForEndOfFrame();
				while (m_validateProductThread.IsAlive)
				{
					yield return new WaitForEndOfFrame();
				}
				m_validateProductThread = null;
				Logging.Log("InAppPurchaseServer: Validate thread complete");
			}
			else
			{
				Logging.LogWarning("InAppPurchaseServer: Skipping Validate Product since networking is disabled", this);
			}
			if (validateProductParameters.Callback != null)
			{
				validateProductParameters.Callback(validateProductParameters.ProductId, validateProductParameters.Result);
			}
			yield return null;
		}

		private void ValidateProductThread(object parameters)
		{
			ValidateProductParameters validateProductParameters = (ValidateProductParameters)parameters;
			if (validateProductParameters == null)
			{
				Logging.LogError("InAppPurchaseServer: no params passed to ValidateProductThread");
				return;
			}
			try
			{
				if (ValidateProduct(validateProductParameters))
				{
				}
			}
			catch (Exception ex)
			{
				validateProductParameters.Result = null;
				Logging.LogError("InAppPurchaseServer: Failed to validate Product : " + ex.ToString(), this);
			}
		}

		private bool ValidateProduct(ValidateProductParameters validateProductParameters)
		{
			StaticRequestDetails serverDetails = SetupNetworking.GetServerDetails(m_server);
			if (serverDetails == null)
			{
				Logging.LogError("InAppPurchaseServer: ValidateProduct: No serverDetails", this);
				return false;
			}
			ValidateProductRequestBuilder validateProductRequestBuilder = new ValidateProductRequestBuilder();
			validateProductRequestBuilder.StaticRequestDetails = serverDetails;
			validateProductRequestBuilder.OS = m_serverPlatform;
			validateProductRequestBuilder.OSVer = m_osVer;
			validateProductRequestBuilder.StoreName = m_storeName;
			validateProductRequestBuilder.IsSandbox = m_useSandbox;
			ValidateProductRequestBuilder validateProductRequestBuilder2 = validateProductRequestBuilder;
			string requestUri = validateProductRequestBuilder2.ToString();
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
			string storeReceipt = validateProductParameters.ReceiptBase64;
			if (m_storeName == StoreNames.AmazonStore)
			{
				AmazonReceiptBody amazonReceiptBody = new AmazonReceiptBody();
				amazonReceiptBody.UserID = validateProductParameters.UserId;
				amazonReceiptBody.PurchaseTokenBase64 = validateProductParameters.ReceiptBase64;
				AmazonReceiptBody obj = amazonReceiptBody;
				storeReceipt = JSONSerialiser.AsString(obj);
				storeReceipt = Convert.ToBase64String(Encoding.UTF8.GetBytes(storeReceipt));
			}
			ValidateProductRequest validateProductRequest = new ValidateProductRequest();
			validateProductRequest.RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
			validateProductRequest.SessionId = TelemetryManager.SessionId;
			validateProductRequest.DeviceId = DeviceIdManager.DeviceId;
			validateProductRequest.ProfileFriendlyName = m_friendlyProfileName;
			validateProductRequest.ProfileType = m_profileType;
			validateProductRequest.Resolution = m_resolution;
			validateProductRequest.SysInfo = m_sysInfo;
			validateProductRequest.VersionNumber = m_gameVersion;
			validateProductRequest.Language = m_language;
			validateProductRequest.LanguageCode = m_languageCode;
			validateProductRequest.Locale = m_currentLocale;
			validateProductRequest.StoreReceipt = storeReceipt;
			validateProductRequest.ProductId = validateProductParameters.ProductId;
			validateProductRequest.StoreSpecificProductId = validateProductParameters.StoreSpecificProductId;
			ValidateProductRequest validateProductRequest2 = validateProductRequest;
			ValidateProductResponse item = RESTfulApi.GetItem<ValidateProductResponse, ValidateProductRequest>(HttpVerb.POST, requestUri, validateProductRequest2, requestDetails2);
			if (item == null)
			{
				Logging.LogError("InAppPurchaseServer:  Did not receive a response from server.", this);
				return false;
			}
			if (string.IsNullOrEmpty(item.RequestId))
			{
				validateProductParameters.Result = false;
				Logging.LogError("InAppPurchaseServer:  Did not receive a RequestId back from server. Not trusting this response.", this);
				return false;
			}
			if (string.CompareOrdinal(item.RequestId, validateProductRequest2.RequestId) != 0)
			{
				validateProductParameters.Result = false;
				Logging.LogError("InAppPurchaseServer:  Received a different RequestId back from server than the one we sent. Not trusting this response.", this);
				return false;
			}
			if (!item.IsValidReceipt)
			{
				validateProductParameters.Result = false;
				Logging.LogError("InAppPurchaseServer: ReceiptBase64 is not valid", this);
				return true;
			}
			if (item.ProductDetails == null)
			{
				validateProductParameters.Result = false;
				Logging.LogError("InAppPurchaseServer:  Received an invalid response object : No ProductDetails returned", this);
				return false;
			}
			bool flag = false;
			foreach (ProductDetails productDetail in item.ProductDetails)
			{
				if (!IsStoreItemIntendedForThisApp(productDetail.StoreSpecificProductId))
				{
					continue;
				}
				if (string.CompareOrdinal(productDetail.Name, validateProductParameters.ProductId) == 0)
				{
					flag = true;
					break;
				}
				if (productDetail.ListOfBundledProducts == null)
				{
					continue;
				}
				foreach (string listOfBundledProduct in productDetail.ListOfBundledProducts)
				{
					if (string.CompareOrdinal(listOfBundledProduct, validateProductParameters.ProductId) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				break;
			}
			validateProductParameters.Result = flag;
			Logging.Log("InAppPurchaseServer:  Got " + item.ProductDetails.Count + " products. Validation result : " + flag, this);
			return true;
		}
	}
}
