using System.Collections;
using Relentless;

namespace Furby
{
	public class InAppShopItem : ShopPurchaseableItem_InApp
	{
		public InAppItemData m_ItemData;

		public InAppItemStatus m_Status;

		private bool m_PurchaseInProgress;

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_ItemData.m_DisplayName);
		}

		public override bool IsPurchased()
		{
			return false;
		}

		public override bool AttemptPurchaseIsValid()
		{
			return true;
		}

		public override string GetSpriteName()
		{
			return m_ItemData.m_SpriteName;
		}

		public override UIAtlas GetAtlas()
		{
			return m_ItemData.m_Atlas;
		}

		private IEnumerator OnClick()
		{
			if (!m_PurchaseInProgress)
			{
				switch (m_Status)
				{
				case InAppItemStatus.Buyable:
					yield return StartCoroutine(DoInAppPurchaseSequence());
					break;
				}
			}
		}

		public override void Purchase()
		{
			DebugUtils.Assert(false, string.Empty);
		}

		public IEnumerator DoInAppPurchaseSequence()
		{
			m_PurchaseInProgress = true;
			Logging.Log("InAppShopItem::DoInAppPurchase");
			GameConfiguration gameConfig = SingletonInstance<GameConfiguration>.Instance;
			if (gameConfig != null && gameConfig.IsIAPAvailable())
			{
				RsStoreMediator storeMediator = SingletonInstance<RsStoreMediator>.Instance;
				if (storeMediator.IsBillingSystemIsAvailable())
				{
					string bundleId = gameConfig.GetGameConfigBlob().m_CrystalLookBundleID;
					if (storeMediator.IsItemAvailable(bundleId))
					{
						Logging.LogWarning("InAppShopItem:DoInAppPurchase Starting flow...");
						GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ProgressMode);
						GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, "GENERIC_CONNECTING_MESSAGE");
						DebugUtils.Log_InGreen("InAppShopItem:DoInAppPurchase Testing if item can be restored...");
						if (storeMediator.CanItemBeRestored(bundleId))
						{
							DebugUtils.Log_InGreen("InAppShopItem:DoInAppPurchase It can! -> abandoning and switching to restore view");
							GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ModalityMode);
							WaitForGameEvent waiter = new WaitForGameEvent();
							GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, "SETTINGS_RESTORE_SUCCESS");
							yield return StartCoroutine(waiter.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
							GameEventRouter.SendEvent(ShopGameEvents.ShopItemsUpdated);
							Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
							GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
						}
						else
						{
							DebugUtils.Log_InGreen("InAppShopItem:DoInAppPurchase ConductingPurchaseFlow...");
							yield return StartCoroutine(storeMediator.ConductPurchaseFlow(bundleId));
							GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ModalityMode);
							WaitForGameEvent waiter2 = new WaitForGameEvent();
							DebugUtils.Log_InGreen("InAppShopItem:DoInAppPurchase Flow complete, response: " + storeMediator.LastStoreResponse);
							RsStoreMediator.StoreResponse? lastStoreResponse = storeMediator.LastStoreResponse;
							if (lastStoreResponse.HasValue)
							{
								switch (lastStoreResponse.Value)
								{
								case RsStoreMediator.StoreResponse.Failed:
									GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, "SHOP_IAP_TRANSFAILED");
									yield return StartCoroutine(waiter2.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
									GameEventRouter.SendEvent(InAppShopPurchaseState.InAppItem_PurchaseFailed, null, bundleId);
									GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseFailed, null, bundleId);
									break;
								case RsStoreMediator.StoreResponse.Succeeded:
									GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, "SHOP_IAP_SUCCESS");
									yield return StartCoroutine(waiter2.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
									GameEventRouter.SendEvent(InAppShopPurchaseState.InAppItem_PurchaseSuccess, null, bundleId);
									GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseComplete, null, bundleId);
									GameEventRouter.SendEvent(CrystalUnlockTelemetryEvents.CrystalUnlocked_Purchased);
									GameEventRouter.SendEvent(ShopGameEvents.ShopItemsUpdated);
									Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
									break;
								case RsStoreMediator.StoreResponse.Cancelled:
									GameEventRouter.SendEvent(InAppShopPurchaseState.InAppItem_PurchaseCancelled, null, bundleId);
									GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseCancelled, null, bundleId);
									break;
								case RsStoreMediator.StoreResponse.CouldntValidate:
									GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, "SHOP_IAP_TRANSFAILED");
									yield return StartCoroutine(waiter2.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
									break;
								}
							}
							GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
						}
					}
					else
					{
						Logging.Log("InAppShopItem::Purchase - FAIL - Item unavailable");
					}
				}
				else
				{
					Logging.Log("InAppShopItem::Purchase - FAIL - Billing System Unavailable");
				}
			}
			else
			{
				Logging.Log("InAppShopItem::Purchase - FAIL - IAP Excluded");
			}
			m_PurchaseInProgress = false;
		}

		public override string GetRealPrice()
		{
			RsStoreMediator instance = SingletonInstance<RsStoreMediator>.Instance;
			if (instance != null)
			{
				GameConfiguration instance2 = SingletonInstance<GameConfiguration>.Instance;
				if (instance2 != null)
				{
					string crystalLookBundleID = instance2.GetGameConfigBlob().m_CrystalLookBundleID;
					if (crystalLookBundleID != string.Empty && (bool)instance && instance.IsItemAvailable(crystalLookBundleID))
					{
						return instance.GetPurchaseItemPriceForDisplay(crystalLookBundleID);
					}
				}
			}
			return "0.00";
		}
	}
}
