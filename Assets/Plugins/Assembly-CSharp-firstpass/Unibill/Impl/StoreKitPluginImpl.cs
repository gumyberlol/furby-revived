using System;
using UnityEngine;

namespace Unibill.Impl
{
	public class StoreKitPluginImpl : IStoreKitPlugin
	{
		public void initialise(AppleAppStoreBillingService svc)
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<AppleAppStoreCallbackMonoBehaviour>().initialise(svc);
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
	}
}
