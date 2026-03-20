using Uniject;
using Uniject.Impl;
using UnityEngine;

namespace Unibill.Impl
{
	internal class RawBillingPlatformProvider : IRawBillingPlatformProvider
	{
		private UnibillConfiguration config;

		public RawBillingPlatformProvider(UnibillConfiguration config)
		{
			this.config = config;
		}

		public IRawGooglePlayInterface getGooglePlay()
		{
			return new RawGooglePlayInterface();
		}

		public IRawAmazonAppStoreBillingInterface getAmazon()
		{
			return new RawAmazonAppStoreBillingInterface(config);
		}

		public IStoreKitPlugin getStorekit()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new StoreKitPluginImpl();
			}
			return new OSXStoreKitPluginImpl();
		}

		public IRawSamsungAppsBillingService getSamsung()
		{
			return new RawSamsungAppsBillingInterface();
		}

		public IHTTPClient getHTTPClient()
		{
			GameObject gameObject = new GameObject();
			return gameObject.AddComponent<HTTPClient>();
		}

		public ILevelLoadListener getLevelLoadListener()
		{
			return new GameObject().AddComponent<UnityLevelLoadListener>();
		}
	}
}
