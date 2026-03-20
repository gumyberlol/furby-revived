using System.Collections.Generic;
using Uniject;

namespace Unibill.Impl
{
	public class AmazonAppStoreBillingService : IBillingService
	{
		private IBillingServiceCallback callback;

		private ProductIdRemapper remapper;

		private UnibillConfiguration db;

		private ILogger logger;

		private IRawAmazonAppStoreBillingInterface amazon;

		private HashSet<string> unknownAmazonProducts = new HashSet<string>();

		private TransactionDatabase tDb;

		public AmazonAppStoreBillingService(IRawAmazonAppStoreBillingInterface amazon, ProductIdRemapper remapper, UnibillConfiguration db, TransactionDatabase tDb, ILogger logger)
		{
			this.remapper = remapper;
			this.db = db;
			this.logger = logger;
			logger.prefix = "UnibillAmazonBillingService";
			this.amazon = amazon;
			this.tDb = tDb;
		}

		public void initialise(IBillingServiceCallback biller)
		{
			callback = biller;
			amazon.initialise(this);
			amazon.initiateItemDataRequest(remapper.getAllPlatformSpecificProductIds());
		}

		public void purchase(string item)
		{
			if (unknownAmazonProducts.Contains(item))
			{
				callback.logError(UnibillError.AMAZONAPPSTORE_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_AMAZON, item);
				callback.onPurchaseFailedEvent(item);
			}
			else
			{
				amazon.initiatePurchaseRequest(item);
			}
		}

		public void restoreTransactions()
		{
			amazon.restoreTransactions();
		}

		public void onSDKAvailable(string isSandbox)
		{
			bool flag = bool.Parse(isSandbox);
			logger.Log("Running against {0} Amazon environment", (!flag) ? "PRODUCTION" : "SANDBOX");
		}

		public void onGetItemDataFailed()
		{
			callback.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_FAILED);
			callback.onSetupComplete(true);
		}

		public void onProductListReceived(string productListString)
		{
			Dictionary<string, object> dic = (Dictionary<string, object>)MiniJSON.jsonDecode(productListString);
			onUserIdRetrieved(dic.getString("userId", string.Empty));
			Dictionary<string, object> hash = dic.getHash("products");
			if (hash.Count == 0)
			{
				callback.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_NO_PRODUCTS_RETURNED);
				callback.onSetupComplete(false);
				return;
			}
			HashSet<PurchasableItem> hashSet = new HashSet<PurchasableItem>();
			foreach (string key in hash.Keys)
			{
				PurchasableItem purchasableItemFromPlatformSpecificId = remapper.getPurchasableItemFromPlatformSpecificId(key.ToString());
				Dictionary<string, object> dictionary = (Dictionary<string, object>)hash[key];
				PurchasableItem.Writer.setLocalizedPrice(purchasableItemFromPlatformSpecificId, dictionary["price"].ToString());
				PurchasableItem.Writer.setLocalizedTitle(purchasableItemFromPlatformSpecificId, (string)dictionary["localizedTitle"]);
				PurchasableItem.Writer.setLocalizedDescription(purchasableItemFromPlatformSpecificId, (string)dictionary["localizedDescription"]);
				PurchasableItem.Writer.setISOCurrencySymbol(purchasableItemFromPlatformSpecificId, dictionary.getString("isoCurrencyCode", string.Empty));
				PurchasableItem.Writer.setPriceInLocalCurrency(purchasableItemFromPlatformSpecificId, decimal.Parse(dictionary.getString("priceDecimal", string.Empty)));
				hashSet.Add(purchasableItemFromPlatformSpecificId);
			}
			HashSet<PurchasableItem> hashSet2 = new HashSet<PurchasableItem>(db.AllPurchasableItems);
			hashSet2.ExceptWith(hashSet);
			if (hashSet2.Count > 0)
			{
				foreach (PurchasableItem item in hashSet2)
				{
					unknownAmazonProducts.Add(remapper.mapItemIdToPlatformSpecificId(item));
					callback.logError(UnibillError.AMAZONAPPSTORE_GETITEMDATAREQUEST_MISSING_PRODUCT, item.Id, remapper.mapItemIdToPlatformSpecificId(item));
				}
			}
			callback.onSetupComplete(true);
		}

		private void onUserIdRetrieved(string userId)
		{
			tDb.UserId = userId;
		}

		public void onTransactionsRestored(string successString)
		{
			if (bool.Parse(successString))
			{
				callback.onTransactionsRestoredSuccess();
			}
			else
			{
				callback.onTransactionsRestoredFail(string.Empty);
			}
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
			string platformSpecificId = (string)dictionary["productId"];
			string receipt = (string)dictionary["purchaseToken"];
			callback.onPurchaseSucceeded(platformSpecificId, receipt);
		}

		public void onPurchaseUpdateFailed()
		{
			logger.LogWarning("AmazonAppStoreBillingService: onPurchaseUpdate() failed.");
		}

		public void onPurchaseUpdateSuccess(string json)
		{
			Dictionary<string, object> dic = (Dictionary<string, object>)MiniJSON.jsonDecode(json);
			List<object> list = dic.get<List<object>>("restored");
			foreach (Dictionary<string, object> item2 in list)
			{
				callback.onPurchaseSucceeded(item2.getString("sku", string.Empty), item2.getString("receipt", string.Empty));
			}
			List<object> list2 = dic.get<List<object>>("revoked");
			foreach (string item3 in list2)
			{
				callback.onPurchaseRefundedEvent(item3);
			}
		}
	}
}
