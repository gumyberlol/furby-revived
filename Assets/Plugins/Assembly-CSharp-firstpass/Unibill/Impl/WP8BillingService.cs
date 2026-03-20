using System;
using System.Collections.Generic;
using System.Linq;
using Uniject;
using unibill.Dummy;

namespace Unibill.Impl
{
	public class WP8BillingService : IWindowsIAPCallback, IBillingService
	{
		private IWindowsIAP wp8;

		private IBillingServiceCallback callback;

		private UnibillConfiguration db;

		private TransactionDatabase tDb;

		private ProductIdRemapper remapper;

		private ILogger logger;

		private HashSet<string> unknownProducts = new HashSet<string>();

		private static int count;

		public WP8BillingService(IWindowsIAP wp8, UnibillConfiguration config, ProductIdRemapper remapper, TransactionDatabase tDb, ILogger logger)
		{
			this.wp8 = wp8;
			db = config;
			this.tDb = tDb;
			this.remapper = remapper;
			this.logger = logger;
		}

		public void initialise(IBillingServiceCallback biller)
		{
			callback = biller;
			init(0);
		}

		private void init(int delay)
		{
			wp8.Initialise(this, delay);
		}

		public void log(string message)
		{
			logger.Log(message);
		}

		public void purchase(string item)
		{
			if (unknownProducts.Contains(item))
			{
				callback.logError(UnibillError.WP8_ATTEMPTING_TO_PURCHASE_PRODUCT_NOT_RETURNED_BY_MICROSOFT, item);
				callback.onPurchaseFailedEvent(item);
			}
			else
			{
				wp8.Purchase(item);
			}
		}

		public void restoreTransactions()
		{
			enumerateLicenses();
			callback.onTransactionsRestoredSuccess();
		}

		public void enumerateLicenses()
		{
			wp8.EnumerateLicenses();
		}

		public void logError(string error)
		{
			logger.LogError(error);
		}

		public void OnProductListReceived(Product[] products)
		{
			if (products.Length == 0)
			{
				callback.logError(UnibillError.WP8_NO_PRODUCTS_RETURNED);
				callback.onSetupComplete(false);
				return;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Product product in products)
			{
				if (remapper.canMapProductSpecificId(product.Id))
				{
					hashSet.Add(product.Id);
					PurchasableItem purchasableItemFromPlatformSpecificId = remapper.getPurchasableItemFromPlatformSpecificId(product.Id);
					PurchasableItem.Writer.setLocalizedPrice(purchasableItemFromPlatformSpecificId, product.Price);
					PurchasableItem.Writer.setLocalizedTitle(purchasableItemFromPlatformSpecificId, product.Title);
					PurchasableItem.Writer.setLocalizedDescription(purchasableItemFromPlatformSpecificId, product.Description);
					PurchasableItem.Writer.setISOCurrencySymbol(purchasableItemFromPlatformSpecificId, product.IsoCurrencyCode);
					PurchasableItem.Writer.setPriceInLocalCurrency(purchasableItemFromPlatformSpecificId, product.PriceDecimal);
				}
				else
				{
					logger.LogError("Warning: Unknown product identifier: {0}", product.Id);
				}
			}
			unknownProducts = new HashSet<string>(db.AllNonSubscriptionPurchasableItems.Select((PurchasableItem x) => remapper.mapItemIdToPlatformSpecificId(x)));
			unknownProducts.ExceptWith(hashSet);
			if (unknownProducts.Count > 0)
			{
				foreach (string unknownProduct in unknownProducts)
				{
					callback.logError(UnibillError.WP8_MISSING_PRODUCT, unknownProduct, remapper.getPurchasableItemFromPlatformSpecificId(unknownProduct).Id);
				}
			}
			enumerateLicenses();
			callback.onSetupComplete(true);
		}

		public void RunOnUIThread(Action<int> act)
		{
			throw new NotImplementedException();
		}

		public void OnPurchaseFailed(string productId, string error)
		{
			logger.LogError("Purchase failed: {0}, {1}", productId, error);
			callback.onPurchaseFailedEvent(productId);
		}

		public void OnPurchaseCancelled(string productId)
		{
			callback.onPurchaseCancelledEvent(productId);
		}

		public void OnPurchaseSucceeded(string productId, string receipt)
		{
			logger.LogError("PURCHASE SUCCEEDED!:{0}", count++);
			if (!remapper.canMapProductSpecificId(productId))
			{
				logger.LogError("Purchased unknown product: {0}. Ignoring!", productId);
				return;
			}
			PurchasableItem purchasableItemFromPlatformSpecificId = remapper.getPurchasableItemFromPlatformSpecificId(productId);
			switch (purchasableItemFromPlatformSpecificId.PurchaseType)
			{
			case PurchaseType.Consumable:
				callback.onPurchaseSucceeded(productId, receipt);
				break;
			case PurchaseType.NonConsumable:
			case PurchaseType.Subscription:
			{
				PurchasableItem purchasableItemFromPlatformSpecificId2 = remapper.getPurchasableItemFromPlatformSpecificId(productId);
				if (tDb.getPurchaseHistory(purchasableItemFromPlatformSpecificId2) == 0)
				{
					callback.onPurchaseSucceeded(productId, receipt);
				}
				break;
			}
			}
		}

		public void OnPurchaseSucceeded(string productId)
		{
			OnPurchaseSucceeded(productId, string.Empty);
		}

		public void OnProductListError(string message)
		{
			if (message.Contains("0x805A0194"))
			{
				callback.logError(UnibillError.WP8_APP_ID_NOT_KNOWN);
				callback.onSetupComplete(false);
			}
			else
			{
				logError("Unable to retrieve product listings. Unibill will automatically retry...");
				logError(message);
				init(3000);
			}
		}
	}
}
