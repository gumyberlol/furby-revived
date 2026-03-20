using System.Collections.Generic;
using System.Linq;

namespace Unibill.Impl
{
	public class AppleAppStoreBillingService : IBillingService
	{
		private IBillingServiceCallback biller;

		private ProductIdRemapper remapper;

		private HashSet<PurchasableItem> products;

		private HashSet<string> productsNotReturnedByStorekit = new HashSet<string>();

		public IStoreKitPlugin storekit { get; private set; }

		public AppleAppStoreBillingService(UnibillConfiguration db, ProductIdRemapper mapper, IStoreKitPlugin storekit)
		{
			this.storekit = storekit;
			remapper = mapper;
			storekit.initialise(this);
			products = new HashSet<PurchasableItem>(db.AllPurchasableItems);
		}

		public void initialise(IBillingServiceCallback biller)
		{
			this.biller = biller;
			if (storekit.storeKitPaymentsAvailable())
			{
				string[] allPlatformSpecificProductIds = remapper.getAllPlatformSpecificProductIds();
				storekit.storeKitRequestProductData(string.Join(",", allPlatformSpecificProductIds), allPlatformSpecificProductIds);
			}
			else
			{
				biller.logError(UnibillError.STOREKIT_BILLING_UNAVAILABLE);
				biller.onSetupComplete(false);
			}
		}

		public void purchase(string item)
		{
			if (productsNotReturnedByStorekit.Contains(item))
			{
				biller.logError(UnibillError.STOREKIT_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_STOREKIT, item);
				biller.onPurchaseFailedEvent(item);
			}
			else
			{
				storekit.storeKitPurchaseProduct(item);
			}
		}

		public void restoreTransactions()
		{
			storekit.storeKitRestoreTransactions();
		}

		public void onProductListReceived(string productListString)
		{
			if (productListString.Length == 0)
			{
				biller.logError(UnibillError.STOREKIT_RETURNED_NO_PRODUCTS);
				biller.onSetupComplete(false);
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(productListString);
			HashSet<PurchasableItem> hashSet = new HashSet<PurchasableItem>();
			foreach (string key in dictionary.Keys)
			{
				if (!remapper.canMapProductSpecificId(key.ToString()))
				{
					biller.logError(UnibillError.UNIBILL_UNKNOWN_PRODUCTID, key.ToString());
					continue;
				}
				PurchasableItem purchasableItemFromPlatformSpecificId = remapper.getPurchasableItemFromPlatformSpecificId(key.ToString());
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[key];
				PurchasableItem.Writer.setLocalizedPrice(purchasableItemFromPlatformSpecificId, dictionary2["price"].ToString());
				PurchasableItem.Writer.setLocalizedTitle(purchasableItemFromPlatformSpecificId, dictionary2["localizedTitle"].ToString());
				PurchasableItem.Writer.setLocalizedDescription(purchasableItemFromPlatformSpecificId, dictionary2["localizedDescription"].ToString());
				if (dictionary2.ContainsKey("isoCurrencyCode"))
				{
					PurchasableItem.Writer.setISOCurrencySymbol(purchasableItemFromPlatformSpecificId, dictionary2["isoCurrencyCode"].ToString());
				}
				if (dictionary2.ContainsKey("priceDecimal"))
				{
					PurchasableItem.Writer.setPriceInLocalCurrency(purchasableItemFromPlatformSpecificId, decimal.Parse(dictionary2["priceDecimal"].ToString()));
				}
				hashSet.Add(purchasableItemFromPlatformSpecificId);
			}
			HashSet<PurchasableItem> hashSet2 = new HashSet<PurchasableItem>(products);
			hashSet2.ExceptWith(hashSet);
			if (hashSet2.Count > 0)
			{
				foreach (PurchasableItem item in hashSet2)
				{
					biller.logError(UnibillError.STOREKIT_REQUESTPRODUCTS_MISSING_PRODUCT, item.Id, remapper.mapItemIdToPlatformSpecificId(item));
				}
			}
			productsNotReturnedByStorekit = new HashSet<string>(hashSet2.Select((PurchasableItem x) => remapper.mapItemIdToPlatformSpecificId(x)));
			biller.onSetupComplete(true);
		}

		public void onPurchaseSucceeded(string data)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJSON.jsonDecode(data);
			biller.onPurchaseSucceeded((string)dictionary["productId"], (string)dictionary["receipt"]);
		}

		public void onPurchaseCancelled(string productId)
		{
			biller.onPurchaseCancelledEvent(productId);
		}

		public void onPurchaseFailed(string productId)
		{
			biller.onPurchaseFailedEvent(productId);
		}

		public void onTransactionsRestoredSuccess()
		{
			biller.onTransactionsRestoredSuccess();
		}

		public void onTransactionsRestoredFail(string error)
		{
			biller.onTransactionsRestoredFail(error);
		}

		public void onFailedToRetrieveProductList()
		{
			biller.logError(UnibillError.STOREKIT_FAILED_TO_RETRIEVE_PRODUCT_DATA);
			biller.onSetupComplete(true);
		}
	}
}
