using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypeInApp : ShopType
	{
		[SerializeField]
		public InAppItemData m_ItemData;

		[SerializeField]
		public float m_ItemSpriteScale = 0.5f;

		public ShopTypeInApp()
		{
			m_Category = ShopTypeCategory.PurchaseWithRealMoney;
		}

		public override int GetNumItems()
		{
			return 1;
		}

		public override void SetUpItem_InApp(ShopItemUI_InApp item, int index)
		{
			DebugUtils.Log_InCyan("ShopTypeInApp::SetUpItem_InApp");
			InAppShopItem inAppShopItem = item.gameObject.AddComponent<InAppShopItem>();
			inAppShopItem.m_ItemData = m_ItemData;
			item.SetUp(m_ItemSpriteScale);
			if (Helper_HasBeenPurchasedAndCrystalUnlocked())
			{
				DebugUtils.Log_InCyan("ShopTypeInApp::SetUpItem_InApp - Already have Crystal");
				item.AlreadyPurchasedHierarchy.SetActive(true);
				item.PurchaseableHierarchy.SetActive(false);
				item.ShopUnavailableHierarchy.SetActive(false);
				inAppShopItem.m_Status = InAppItemStatus.AlreadyPurchased;
				return;
			}
			GameConfiguration instance = SingletonInstance<GameConfiguration>.Instance;
			string crystalLookBundleID = instance.GetGameConfigBlob().m_CrystalLookBundleID;
			bool flag = SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable();
			bool flag2 = SingletonInstance<RsStoreMediator>.Instance.IsBillingSystemIsAvailable();
			bool flag3 = SingletonInstance<RsStoreMediator>.Instance.IsItemAvailable(crystalLookBundleID);
			bool flag4 = SingletonInstance<RsStoreMediator>.Instance.CanItemBePurchased(crystalLookBundleID);
			if (flag2 && flag && flag3 && flag4)
			{
				DebugUtils.Log_InCyan("ShopTypeInApp::SetUpItem_InApp - Offering IAP");
				item.AlreadyPurchasedHierarchy.SetActive(false);
				item.PurchaseableHierarchy.SetActive(true);
				item.ShopUnavailableHierarchy.SetActive(false);
				inAppShopItem.m_Status = InAppItemStatus.Buyable;
			}
			else
			{
				DebugUtils.Log_InCyan("ShopTypeInApp::SetUpItem_InApp - IAPS Allowed? " + flag);
				DebugUtils.Log_InCyan("ShopTypeInApp::SetUpItem_InApp - IAP Item Exists? " + flag3);
				item.AlreadyPurchasedHierarchy.SetActive(false);
				item.PurchaseableHierarchy.SetActive(false);
				item.ShopUnavailableHierarchy.SetActive(true);
				inAppShopItem.m_Status = InAppItemStatus.Unavailable;
			}
		}

		public override bool IsUnlocked(int index)
		{
			return Helper_HasBeenPurchasedAndCrystalUnlocked();
		}

		public override string DEBUG_GetItemName(int index)
		{
			return m_ItemData.m_DisplayName;
		}

		public override void DEBUG_UnlockItem(int index)
		{
		}

		public bool Helper_HasBeenPurchasedAndCrystalUnlocked()
		{
			GameConfiguration instance = SingletonInstance<GameConfiguration>.Instance;
			string crystalLookBundleID = instance.GetGameConfigBlob().m_CrystalLookBundleID;
			bool crystalUnlocked = Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked;
			bool flag = SingletonInstance<RsStoreMediator>.Instance.AmAbleToMediatePurchases() && SingletonInstance<RsStoreMediator>.Instance.HasItemBeenPurchased(crystalLookBundleID);
			return crystalUnlocked || flag;
		}
	}
}
