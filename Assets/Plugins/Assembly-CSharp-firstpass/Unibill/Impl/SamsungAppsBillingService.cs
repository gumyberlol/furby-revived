using System.Collections.Generic;
using Uniject;

namespace Unibill.Impl
{
	public class SamsungAppsBillingService : IBillingService
	{
		private IBillingServiceCallback callback;

		private ProductIdRemapper remapper;

		private UnibillConfiguration config;

		private IRawSamsungAppsBillingService rawSamsung;

		private ILogger logger;

		private HashSet<string> unknownSamsungProducts = new HashSet<string>();

		public SamsungAppsBillingService(UnibillConfiguration config, ProductIdRemapper remapper, IRawSamsungAppsBillingService rawSamsung, ILogger logger)
		{
			this.config = config;
			this.remapper = remapper;
			this.rawSamsung = rawSamsung;
			this.logger = logger;
		}

		public void initialise(IBillingServiceCallback biller)
		{
			callback = biller;
			rawSamsung.initialise(this);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("mode", config.SamsungAppsMode.ToString());
			dictionary.Add("itemGroupId", config.SamsungItemGroupId);
			rawSamsung.getProductList(dictionary.toJson());
		}

		public void purchase(string item)
		{
			if (unknownSamsungProducts.Contains(item))
			{
				callback.logError(UnibillError.SAMSUNG_APPS_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_SAMSUNG, item);
				callback.onPurchaseFailedEvent(item);
			}
			else
			{
				rawSamsung.initiatePurchaseRequest(item);
			}
		}

		public void restoreTransactions()
		{
			rawSamsung.restoreTransactions();
		}

		public void onProductListReceived(string productListString)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(productListString);
			if (dictionary.Count == 0)
			{
				callback.logError(UnibillError.SAMSUNG_APPS_NO_PRODUCTS_RETURNED);
				callback.onSetupComplete(false);
				return;
			}
			HashSet<PurchasableItem> hashSet = new HashSet<PurchasableItem>();
			foreach (string key in dictionary.Keys)
			{
				if (remapper.canMapProductSpecificId(key.ToString()))
				{
					PurchasableItem purchasableItemFromPlatformSpecificId = remapper.getPurchasableItemFromPlatformSpecificId(key.ToString());
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[key];
					PurchasableItem.Writer.setLocalizedPrice(purchasableItemFromPlatformSpecificId, dictionary2["price"].ToString());
					PurchasableItem.Writer.setLocalizedTitle(purchasableItemFromPlatformSpecificId, (string)dictionary2["localizedTitle"]);
					PurchasableItem.Writer.setLocalizedDescription(purchasableItemFromPlatformSpecificId, (string)dictionary2["localizedDescription"]);
					PurchasableItem.Writer.setISOCurrencySymbol(purchasableItemFromPlatformSpecificId, dictionary2.getString("isoCurrencyCode", string.Empty));
					PurchasableItem.Writer.setPriceInLocalCurrency(purchasableItemFromPlatformSpecificId, decimal.Parse(dictionary2.getString("priceDecimal", "0")));
					hashSet.Add(purchasableItemFromPlatformSpecificId);
				}
				else
				{
					logger.LogError("Warning: Unknown product identifier: {0}", key.ToString());
				}
			}
			HashSet<PurchasableItem> hashSet2 = new HashSet<PurchasableItem>(config.AllPurchasableItems);
			hashSet2.ExceptWith(hashSet);
			if (hashSet2.Count > 0)
			{
				foreach (PurchasableItem item in hashSet2)
				{
					unknownSamsungProducts.Add(remapper.mapItemIdToPlatformSpecificId(item));
					callback.logError(UnibillError.SAMSUNG_APPS_MISSING_PRODUCT, item.Id, remapper.mapItemIdToPlatformSpecificId(item));
				}
			}
			callback.onSetupComplete(true);
		}

		public void onPurchaseFailed(string item)
		{
			callback.onPurchaseFailedEvent(item);
		}

		public void onPurchaseCancelled(string item)
		{
			callback.onPurchaseCancelledEvent(item);
		}

		public void onPurchaseSucceeded(string json)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(json);
			callback.onPurchaseSucceeded((string)dictionary["productId"], (string)dictionary["signature"]);
		}

		public void onTransactionsRestored(string success)
		{
			if (bool.Parse(success))
			{
				callback.onTransactionsRestoredSuccess();
			}
			else
			{
				callback.onTransactionsRestoredFail(string.Empty);
			}
		}

		public void onInitFail()
		{
			callback.onSetupComplete(false);
		}
	}
}
