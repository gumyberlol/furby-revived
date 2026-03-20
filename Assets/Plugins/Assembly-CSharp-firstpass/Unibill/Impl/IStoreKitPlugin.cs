namespace Unibill.Impl
{
	public interface IStoreKitPlugin
	{
		void initialise(AppleAppStoreBillingService callback);

		bool storeKitPaymentsAvailable();

		void storeKitRequestProductData(string productIdentifiers, string[] productIds);

		void storeKitPurchaseProduct(string productId);

		void storeKitRestoreTransactions();
	}
}
