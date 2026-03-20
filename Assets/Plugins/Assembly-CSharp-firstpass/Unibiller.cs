using System;
using Unibill;
using Unibill.Impl;
using Uniject.Impl;
using UnityEngine;

public class Unibiller
{
	private static Biller biller;

	public static BillingPlatform BillingPlatform
	{
		get
		{
			if (biller != null)
			{
				return biller.InventoryDatabase.CurrentPlatform;
			}
			return BillingPlatform.UnityEditor;
		}
	}

	public static bool Initialised
	{
		get
		{
			if (biller != null)
			{
				return biller.State == BillerState.INITIALISED || biller.State == BillerState.INITIALISED_WITH_ERROR;
			}
			return false;
		}
	}

	public static UnibillError[] Errors
	{
		get
		{
			if (biller != null)
			{
				return biller.Errors.ToArray();
			}
			return new UnibillError[0];
		}
	}

	public static string UserId
	{
		get
		{
			if (biller != null)
			{
				return biller.getUserId();
			}
			return string.Empty;
		}
	}

	public static PurchasableItem[] AllPurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllPurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllNonConsumablePurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllNonConsumablePurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllConsumablePurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllConsumablePurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllSubscriptions
	{
		get
		{
			return biller.InventoryDatabase.AllSubscriptions.ToArray();
		}
	}

	public static string[] AllCurrencies
	{
		get
		{
			return biller.CurrencyIdentifiers;
		}
	}

	public static event Action<UnibillState> onBillerReady;

	public static event Action<PurchasableItem> onPurchaseCancelled;

	public static event Action<PurchaseEvent> onPurchaseCompleteEvent;

	public static event Action<PurchasableItem> onPurchaseComplete;

	public static event Action<PurchasableItem> onPurchaseFailed;

	public static event Action<PurchasableItem> onPurchaseRefunded;

	public static event Action<bool> onTransactionsRestored;

	public static void Initialise()
	{
		if (biller == null)
		{
			RemoteConfigManager remoteConfigManager = new RemoteConfigManager(new UnityResourceLoader(), new UnityPlayerPrefsStorage(), new UnityLogger(), Application.platform);
			UnibillConfiguration config = remoteConfigManager.Config;
			_internal_doInitialise(new BillerFactory(new UnityResourceLoader(), new UnityLogger(), new UnityPlayerPrefsStorage(), new RawBillingPlatformProvider(config), config, new UnityUtil()).instantiate());
		}
	}

	public static PurchasableItem GetPurchasableItemById(string unibillPurchasableId)
	{
		if (biller != null)
		{
			return biller.InventoryDatabase.getItemById(unibillPurchasableId);
		}
		return null;
	}

	public static string[] GetAllPurchaseReceipts(PurchasableItem forItem)
	{
		if (biller != null)
		{
			return biller.getReceiptsForPurchasable(forItem);
		}
		return new string[0];
	}

	public static void initiatePurchase(PurchasableItem purchasable)
	{
		if (biller != null)
		{
			biller.purchase(purchasable);
		}
	}

	public static void initiatePurchase(string purchasableId)
	{
		if (biller != null)
		{
			biller.purchase(purchasableId);
		}
	}

	public static int GetPurchaseCount(PurchasableItem item)
	{
		if (biller != null)
		{
			return biller.getPurchaseHistory(item);
		}
		return 0;
	}

	public static int GetPurchaseCount(string purchasableId)
	{
		if (biller != null)
		{
			return biller.getPurchaseHistory(purchasableId);
		}
		return 0;
	}

	public static decimal GetCurrencyBalance(string currencyIdentifier)
	{
		if (biller != null)
		{
			return biller.getCurrencyBalance(currencyIdentifier);
		}
		return 0m;
	}

	public static void CreditBalance(string currencyIdentifier, decimal amount)
	{
		if (biller != null)
		{
			biller.creditCurrencyBalance(currencyIdentifier, amount);
		}
	}

	public static bool DebitBalance(string currencyIdentifier, decimal amount)
	{
		if (biller != null)
		{
			return biller.debitCurrencyBalance(currencyIdentifier, amount);
		}
		return false;
	}

	public static void restoreTransactions()
	{
		if (biller != null)
		{
			biller.restoreTransactions();
		}
	}

	public static void clearTransactions()
	{
		if (biller != null)
		{
			biller.ClearPurchases();
		}
	}

	public static void _internal_doInitialise(Biller biller)
	{
		Unibiller.biller = biller;
		biller.onBillerReady += delegate(bool success)
		{
			if (Unibiller.onBillerReady != null)
			{
				if (success)
				{
					Unibiller.onBillerReady((biller.State != BillerState.INITIALISED) ? UnibillState.SUCCESS_WITH_ERRORS : UnibillState.SUCCESS);
				}
				else
				{
					Unibiller.onBillerReady(UnibillState.CRITICAL_ERROR);
				}
			}
		};
		biller.onPurchaseCancelled += _onPurchaseCancelled;
		biller.onPurchaseComplete += _onPurchaseComplete;
		biller.onPurchaseFailed += _onPurchaseFailed;
		biller.onPurchaseRefunded += _onPurchaseRefunded;
		biller.onTransactionsRestored += _onTransactionsRestored;
		biller.Initialise();
	}

	private static void _onPurchaseCancelled(PurchasableItem item)
	{
		if (Unibiller.onPurchaseCancelled != null)
		{
			Unibiller.onPurchaseCancelled(item);
		}
	}

	private static void _onPurchaseComplete(PurchaseEvent e)
	{
		if (Unibiller.onPurchaseComplete != null)
		{
			Unibiller.onPurchaseComplete(e.PurchasedItem);
		}
		if (Unibiller.onPurchaseCompleteEvent != null)
		{
			Unibiller.onPurchaseCompleteEvent(e);
		}
	}

	private static void _onPurchaseFailed(PurchasableItem item)
	{
		if (Unibiller.onPurchaseFailed != null)
		{
			Unibiller.onPurchaseFailed(item);
		}
	}

	private static void _onPurchaseRefunded(PurchasableItem item)
	{
		if (Unibiller.onPurchaseRefunded != null)
		{
			Unibiller.onPurchaseRefunded(item);
		}
	}

	private static void _onTransactionsRestored(bool success)
	{
		if (Unibiller.onTransactionsRestored != null)
		{
			Unibiller.onTransactionsRestored(success);
		}
	}
}
