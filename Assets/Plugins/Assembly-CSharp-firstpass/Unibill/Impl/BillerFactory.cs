using System.Collections.Generic;
using System.Linq;
using Tests;
using Uniject;
using UnityEngine;
using unibill.Dummy;

namespace Unibill.Impl
{
	public class BillerFactory
	{
		private IResourceLoader loader;

		private ILogger logger;

		private IStorage storage;

		private IRawBillingPlatformProvider platformProvider;

		private IUtil util;

		private UnibillConfiguration config;

		private CurrencyManager _currencyManager;

		private TransactionDatabase _tDb;

		private HelpCentre _helpCentre;

		private ProductIdRemapper _remapper;

		public BillerFactory(IResourceLoader resourceLoader, ILogger logger, IStorage storage, IRawBillingPlatformProvider platformProvider, UnibillConfiguration config, IUtil util)
		{
			loader = resourceLoader;
			this.logger = logger;
			this.storage = storage;
			this.platformProvider = platformProvider;
			this.config = config;
			this.util = util;
		}

		public Biller instantiate()
		{
			IBillingService billingSubsystem = instantiateBillingSubsystem();
			Biller biller = new Biller(config, getTransactionDatabase(), billingSubsystem, getLogger(), getHelp(), getMapper(), getCurrencyManager());
			instantiateAnalytics(biller);
			return biller;
		}

		public void instantiateAnalytics(Biller biller)
		{
			if (!string.IsNullOrEmpty(config.UnibillAnalyticsAppId))
			{
				new AnalyticsReporter(biller, config, platformProvider.getHTTPClient(), getStorage(), util, platformProvider.getLevelLoadListener());
			}
		}

		private IBillingService instantiateBillingSubsystem()
		{
			switch (config.CurrentPlatform)
			{
			case BillingPlatform.AppleAppStore:
				return new AppleAppStoreBillingService(config, getMapper(), platformProvider.getStorekit());
			case BillingPlatform.AmazonAppstore:
				return new AmazonAppStoreBillingService(platformProvider.getAmazon(), getMapper(), config, getTransactionDatabase(), getLogger());
			case BillingPlatform.GooglePlay:
				return new GooglePlayBillingService(platformProvider.getGooglePlay(), config, getMapper(), getLogger());
			case BillingPlatform.MacAppStore:
				return new AppleAppStoreBillingService(config, getMapper(), platformProvider.getStorekit());
			case BillingPlatform.WindowsPhone8:
			{
				WP8BillingService wP8BillingService = new WP8BillingService(Factory.Create(config.WP8SandboxEnabled, GetDummyProducts()), config, getMapper(), getTransactionDatabase(), getLogger());
				new GameObject().AddComponent<WP8Eventhook>().callback = wP8BillingService;
				return wP8BillingService;
			}
			case BillingPlatform.Windows8_1:
			{
				Win8_1BillingService win8_1BillingService = new Win8_1BillingService(Factory.Create(config.UseWin8_1Sandbox, GetDummyProducts()), config, getMapper(), getTransactionDatabase(), getLogger());
				new GameObject().AddComponent<Win8Eventhook>().callback = win8_1BillingService;
				return win8_1BillingService;
			}
			case BillingPlatform.SamsungApps:
				return new SamsungAppsBillingService(config, getMapper(), platformProvider.getSamsung(), getLogger());
			default:
				return new FakeBillingService(getMapper());
			}
		}

		private CurrencyManager getCurrencyManager()
		{
			if (_currencyManager == null)
			{
				_currencyManager = new CurrencyManager(config, getStorage());
			}
			return _currencyManager;
		}

		private Product[] GetDummyProducts()
		{
			IEnumerable<Product> source = from x in config.AllPurchasableItems
				where x.PurchaseType != PurchaseType.Subscription
				select new Product
				{
					Consumable = (x.PurchaseType == PurchaseType.Consumable),
					Description = x.description,
					Id = x.LocalId,
					Price = "$123.45",
					PriceDecimal = 123.45m,
					Title = x.name
				};
			return source.ToArray();
		}

		private TransactionDatabase getTransactionDatabase()
		{
			if (_tDb == null)
			{
				_tDb = new TransactionDatabase(getStorage(), getLogger());
			}
			return _tDb;
		}

		private IStorage getStorage()
		{
			return storage;
		}

		private HelpCentre getHelp()
		{
			if (_helpCentre == null)
			{
				_helpCentre = new HelpCentre(loader.openTextFile("unibillStrings.json").ReadToEnd());
			}
			return _helpCentre;
		}

		private ProductIdRemapper getMapper()
		{
			if (_remapper == null)
			{
				_remapper = new ProductIdRemapper(config);
			}
			return _remapper;
		}

		private ILogger getLogger()
		{
			return logger;
		}

		private IResourceLoader getResourceLoader()
		{
			return loader;
		}
	}
}
