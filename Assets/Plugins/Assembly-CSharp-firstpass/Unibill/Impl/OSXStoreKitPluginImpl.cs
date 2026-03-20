using System;

namespace Unibill.Impl
{
	public class OSXStoreKitPluginImpl : IStoreKitPlugin
	{
		private static AppleAppStoreBillingService callback;

		public void initialise(AppleAppStoreBillingService callback)
		{
			OSXStoreKitPluginImpl.callback = callback;
		}

		public bool storeKitPaymentsAvailable()
		{
			throw new NotImplementedException();
		}

		public void storeKitRequestProductData(string productIdentifiers, string[] productIds)
		{
			throw new NotImplementedException();
		}

		public void storeKitPurchaseProduct(string productId)
		{
			throw new NotImplementedException();
		}

		public void storeKitRestoreTransactions()
		{
			throw new NotImplementedException();
		}

		public static void UnibillSendMessage(string method, string argument)
		{
			switch (method)
			{
			case "onProductListReceived":
				onProductListReceived(argument);
				break;
			case "onProductPurchaseSuccess":
				onProductPurchaseSuccess(argument);
				break;
			case "onProductPurchaseCancelled":
				onProductPurchaseCancelled(argument);
				break;
			case "onProductPurchaseFailed":
				onProductPurchaseFailed(argument);
				break;
			case "onTransactionsRestoredSuccess":
				onTransactionsRestoredSuccess(argument);
				break;
			case "onTransactionsRestoredFail":
				onTransactionsRestoredFail(argument);
				break;
			case "onFailedToRetrieveProductList":
				onFailedToRetrieveProductList(argument);
				break;
			}
		}

		public static void onProductListReceived(string productList)
		{
			callback.onProductListReceived(productList);
		}

		public static void onProductPurchaseSuccess(string productId)
		{
			callback.onPurchaseSucceeded(productId);
		}

		public static void onProductPurchaseCancelled(string productId)
		{
			callback.onPurchaseCancelled(productId);
		}

		public static void onProductPurchaseFailed(string productId)
		{
			callback.onPurchaseFailed(productId);
		}

		public static void onTransactionsRestoredSuccess(string empty)
		{
			callback.onTransactionsRestoredSuccess();
		}

		public static void onTransactionsRestoredFail(string error)
		{
			callback.onTransactionsRestoredFail(error);
		}

		public static void onFailedToRetrieveProductList(string nop)
		{
			callback.onFailedToRetrieveProductList();
		}
	}
}
