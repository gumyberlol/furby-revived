using System.Collections.Generic;
using Furby.Utilities;
using Furby.Utilities.Pantry;
using Relentless;

namespace Furby
{
	public static class WholeGameShopHelpers
	{
		public static HashSet<string> PurchasedItems
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.m_purchasedItems;
			}
		}

		public static bool IsItemUnlocked(BabyUtilityItem shopItem)
		{
			if (shopItem.Unlocked)
			{
				return true;
			}
			if (PurchasedItems.Contains("BabyUtilityItem_" + shopItem.Name))
			{
				return true;
			}
			return false;
		}

		public static void PurchaseItem(BabyUtilityItem shopItem)
		{
			PurchasedItems.Add("BabyUtilityItem_" + shopItem.Name);
		}

		public static bool IsItemUnlocked(IPlayroomSelectable shopItem)
		{
			bool flag = shopItem.IsAvailableForPurchase();
			bool flag2 = PurchasedItems.Contains("SelectableFeature_" + shopItem.GetName());
			if (flag && !flag2)
			{
				return false;
			}
			if (shopItem.IsUnlockedByScannedQRCode())
			{
				string qRCode = shopItem.GetQRCode();
				string variantCode = shopItem.GetVariantCode();
				bool flag3 = Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Contains(qRCode + variantCode);
				bool flag4 = PurchasedItems.Contains("SelectableFeature_" + shopItem.GetName());
				return flag3 || flag4;
			}
			if (shopItem.IsUnlockedByComAirTone())
			{
				bool flag5 = Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(shopItem.GetComAirTone());
				bool flag6 = PurchasedItems.Contains("SelectableFeature_" + shopItem.GetName());
				return flag5 || flag6;
			}
			return true;
		}

		public static void PurchaseItem(IPlayroomSelectable shopItem)
		{
			PurchasedItems.Add("SelectableFeature_" + shopItem.GetName());
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		public static bool IsItemUnlocked(PantryFoodData foodData)
		{
			if (foodData.UnlockedByQRCode)
			{
				return PurchasedItems.Contains("PantryFoodData_" + foodData.DisplayName);
			}
			return false;
		}

		public static void PurchaseItem(PantryFoodData foodData)
		{
			PurchasedItems.Add("PantryFoodData_" + foodData.DisplayName);
			Singleton<GameDataStoreObject>.Instance.Save();
		}
	}
}
