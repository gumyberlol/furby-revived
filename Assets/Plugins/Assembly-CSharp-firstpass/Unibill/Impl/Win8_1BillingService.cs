using Uniject;
using unibill.Dummy;

namespace Unibill.Impl
{
	internal class Win8_1BillingService : IWindowsIAPCallback, IBillingService
	{
		private static int count;

		public Win8_1BillingService(IWindowsIAP wp8, UnibillConfiguration config, ProductIdRemapper remapper, TransactionDatabase tDb, ILogger logger)
		{
		}

		public void initialise(IBillingServiceCallback biller)
		{
		}

		private void init(int delay)
		{
		}

		public void purchase(string item)
		{
		}

		public void restoreTransactions()
		{
		}

		public void enumerateLicenses()
		{
		}

		public void logError(string error)
		{
		}

		public void OnProductListReceived(Product[] products)
		{
		}

		public void log(string message)
		{
		}

		public void OnPurchaseFailed(string productId, string error)
		{
		}

		public void OnPurchaseCancelled(string productId)
		{
		}

		public void OnPurchaseSucceeded(string productId, string receipt)
		{
		}

		public void OnPurchaseSucceeded(string productId)
		{
			OnPurchaseSucceeded(productId, string.Empty);
		}

		public void OnProductListError(string message)
		{
		}
	}
}
