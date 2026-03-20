using System.Collections.Generic;
using Uniject;

namespace Unibill.Impl
{
	public class GooglePlayBillingService : IBillingService
	{
		private string publicKey;

		private IRawGooglePlayInterface rawInterface;

		private IBillingServiceCallback callback;

		private ProductIdRemapper remapper;

		private UnibillConfiguration db;

		private ILogger logger;

		private HashSet<string> unknownAmazonProducts = new HashSet<string>();

		public GooglePlayBillingService(IRawGooglePlayInterface rawInterface, UnibillConfiguration config, ProductIdRemapper remapper, ILogger logger)
		{
			this.rawInterface = rawInterface;
			publicKey = config.GooglePlayPublicKey;
			this.remapper = remapper;
			db = config;
			this.logger = logger;
		}

		public void initialise(IBillingServiceCallback callback)
		{
			this.callback = callback;
			if (publicKey == null || publicKey.Equals("[Your key]"))
			{
				callback.logError(UnibillError.GOOGLEPLAY_PUBLICKEY_NOTCONFIGURED, publicKey);
				callback.onSetupComplete(false);
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("publicKey", publicKey);
			List<string> list = new List<string>();
			List<object> list2 = new List<object>();
			foreach (PurchasableItem allPurchasableItem in db.AllPurchasableItems)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				string text = remapper.mapItemIdToPlatformSpecificId(allPurchasableItem);
				list.Add(text);
				dictionary2.Add("productId", text);
				dictionary2.Add("consumable", allPurchasableItem.PurchaseType == PurchaseType.Consumable);
				list2.Add(dictionary2);
			}
			dictionary.Add("products", list2);
			string text2 = dictionary.toJson();
			rawInterface.initialise(this, text2, list.ToArray());
		}

		public void restoreTransactions()
		{
			rawInterface.restoreTransactions();
		}

		public void purchase(string item)
		{
			if (unknownAmazonProducts.Contains(item))
			{
				callback.logError(UnibillError.GOOGLEPLAY_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_GOOGLEPLAY, item);
				callback.onPurchaseFailedEvent(item);
			}
			else
			{
				rawInterface.purchase(item);
			}
		}

		public void onBillingNotSupported()
		{
			callback.logError(UnibillError.GOOGLEPLAY_BILLING_UNAVAILABLE);
			callback.onSetupComplete(false);
		}

		public void onPurchaseSucceeded(string json)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(json);
			callback.onPurchaseSucceeded((string)dictionary["productId"], (string)dictionary["signature"]);
		}

		public void onPurchaseCancelled(string item)
		{
			callback.onPurchaseCancelledEvent(item);
		}

		public void onPurchaseRefunded(string item)
		{
			callback.onPurchaseRefundedEvent(item);
		}

		public void onPurchaseFailed(string item)
		{
			callback.onPurchaseFailedEvent(item);
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

		public void onInvalidPublicKey(string key)
		{
			callback.logError(UnibillError.GOOGLEPLAY_PUBLICKEY_INVALID, key);
			callback.onSetupComplete(false);
		}

		public void onProductListReceived(string productListString)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(productListString);
			if (dictionary.Count == 0)
			{
				callback.logError(UnibillError.GOOGLEPLAY_NO_PRODUCTS_RETURNED);
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
					long value = dictionary2.getLong("priceInMicros");
					decimal amount = new decimal(value) / 1000000m;
					PurchasableItem.Writer.setPriceInLocalCurrency(purchasableItemFromPlatformSpecificId, amount);
					hashSet.Add(purchasableItemFromPlatformSpecificId);
				}
				else
				{
					logger.LogError("Warning: Unknown product identifier: {0}", key.ToString());
				}
			}
			HashSet<PurchasableItem> hashSet2 = new HashSet<PurchasableItem>(db.AllPurchasableItems);
			hashSet2.ExceptWith(hashSet);
			if (hashSet2.Count > 0)
			{
				foreach (PurchasableItem item in hashSet2)
				{
					unknownAmazonProducts.Add(remapper.mapItemIdToPlatformSpecificId(item));
					callback.logError(UnibillError.GOOGLEPLAY_MISSING_PRODUCT, item.Id, remapper.mapItemIdToPlatformSpecificId(item));
				}
			}
			logger.Log("Received product list, polling for consumables...");
			rawInterface.pollForConsumables();
		}

		public void onPollForConsumablesFinished(string json)
		{
			logger.Log("Finished poll for consumables, completing init.");
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(json);
			if (dictionary != null)
			{
				List<string> stringList = dictionary.getStringList("ownedSubscriptions");
				if (stringList != null)
				{
					callback.onActiveSubscriptionsRetrieved(stringList);
				}
			}
			callback.onSetupComplete(true);
		}
	}
}
