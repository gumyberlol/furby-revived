using Uniject;

namespace Unibill.Impl
{
	public interface IRawBillingPlatformProvider
	{
		IRawGooglePlayInterface getGooglePlay();

		IRawAmazonAppStoreBillingInterface getAmazon();

		IStoreKitPlugin getStorekit();

		IRawSamsungAppsBillingService getSamsung();

		IHTTPClient getHTTPClient();

		ILevelLoadListener getLevelLoadListener();
	}
}
