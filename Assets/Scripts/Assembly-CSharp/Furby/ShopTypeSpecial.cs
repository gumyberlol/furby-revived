using System.Collections.Generic;
using Furby.Playroom;
using Furby.Utilities.Pantry;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypeSpecial : ShopType
	{
		public enum SpecialItemShopItemIndexType
		{
			PromoEgg = 0,
			PlayroomItem = 1,
			PantryItem = 2
		}

		public class SpecialItemShopItemIndex
		{
			public int m_Index;

			public SpecialItemShopItemIndexType m_Type;

			public SpecialItemShopItemIndex(int index, SpecialItemShopItemIndexType type)
			{
				m_Index = index;
				m_Type = type;
			}
		}

		[SerializeField]
		private PromoEggData m_PromoEggData;

		[SerializeField]
		private SelectableFeatureList[] m_PlayroomFeatureLists;

		[SerializeField]
		private SelectableThemeList[] m_PlayroomThemeLists;

		private List<IPlayroomSelectable> m_AllPlayroomItems = new List<IPlayroomSelectable>();

		public List<PantryFoodData> m_PurchaseablePantryFoodData = new List<PantryFoodData>();

		[SerializeField]
		private PantryFoodDataList m_AllPantryFoodData;

		[SerializeField]
		public List<SpecialItemShopItemIndex> m_ItemIndices = new List<SpecialItemShopItemIndex>();

		public override void RefreshList()
		{
		}

		private void Awake()
		{
			ExtractPlayroomItemsFromSource();
			ExtractPurchaseablePantryData();
			ExtractPurchaseablePromoEggs();
		}

		private void ExtractPlayroomItemsFromSource()
		{
			SelectableFeatureList[] playroomFeatureLists = m_PlayroomFeatureLists;
			foreach (SelectableFeatureList selectableFeatureList in playroomFeatureLists)
			{
				foreach (SelectableFeature feature in selectableFeatureList.Features)
				{
					if (((IPlayroomSelectable)feature).IsUnlockedByScannedQRCode() || ((IPlayroomSelectable)feature).IsUnlockedByComAirTone())
					{
						m_AllPlayroomItems.Add(feature);
						m_ItemIndices.Add(new SpecialItemShopItemIndex(m_AllPlayroomItems.Count - 1, SpecialItemShopItemIndexType.PlayroomItem));
					}
				}
			}
			SelectableThemeList[] playroomThemeLists = m_PlayroomThemeLists;
			foreach (SelectableThemeList selectableThemeList in playroomThemeLists)
			{
				foreach (SelectableTheme theme in selectableThemeList.Themes)
				{
					if (((IPlayroomSelectable)theme).IsUnlockedByScannedQRCode() || ((IPlayroomSelectable)theme).IsUnlockedByComAirTone())
					{
						m_AllPlayroomItems.Add(theme);
						m_ItemIndices.Add(new SpecialItemShopItemIndex(m_AllPlayroomItems.Count - 1, SpecialItemShopItemIndexType.PlayroomItem));
					}
				}
			}
		}

		private void ExtractPurchaseablePantryData()
		{
			foreach (PantryFoodData allPantryFoodDatum in m_AllPantryFoodData)
			{
				if (allPantryFoodDatum.UnlockedByQRCode)
				{
					m_PurchaseablePantryFoodData.Add(allPantryFoodDatum);
					m_ItemIndices.Add(new SpecialItemShopItemIndex(m_PurchaseablePantryFoodData.Count - 1, SpecialItemShopItemIndexType.PantryItem));
				}
			}
		}

		private void ExtractPurchaseablePromoEggs()
		{
			int num = 0;
			PromoEggData.PurchaseableEgg[] eggsAvailable = m_PromoEggData.m_eggsAvailable;
			foreach (PromoEggData.PurchaseableEgg purchaseableEgg in eggsAvailable)
			{
				Logging.Log(purchaseableEgg.m_spriteName);
				m_ItemIndices.Add(new SpecialItemShopItemIndex(num++, SpecialItemShopItemIndexType.PromoEgg));
			}
		}

		public override int GetNumItems()
		{
			return m_ItemIndices.Count;
		}

		public override void SetUpItem_Virtual(ShopItemUI item, int index)
		{
			SpecialItemShopItemIndex specialItemShopItemIndex = m_ItemIndices[index];
			switch (specialItemShopItemIndex.m_Type)
			{
			case SpecialItemShopItemIndexType.PromoEgg:
				SetupItem_AsPromoEgg(item, specialItemShopItemIndex.m_Index);
				break;
			case SpecialItemShopItemIndexType.PlayroomItem:
				SetupItem_AsPlayroomItem(item, specialItemShopItemIndex.m_Index);
				break;
			case SpecialItemShopItemIndexType.PantryItem:
				SetUpItem_AsPantryItem(item, specialItemShopItemIndex.m_Index);
				break;
			}
		}

		private void SetupItem_AsPromoEgg(ShopItemUI item, int index)
		{
			PromoEggShopItem promoEggShopItem = item.gameObject.AddComponent<PromoEggShopItem>();
			promoEggShopItem.m_purchasableEggInfo = m_PromoEggData.m_eggsAvailable[index];
			item.SetUp(1f);
		}

		private void SetupItem_AsPlayroomItem(ShopItemUI item, int index)
		{
			SelectableFeatureShopItem selectableFeatureShopItem = item.gameObject.AddComponent<SelectableFeatureShopItem>();
			selectableFeatureShopItem.SetFeature(m_AllPlayroomItems[index]);
			item.SetUp(1f);
		}

		private void SetUpItem_AsPantryItem(ShopItemUI item, int index)
		{
			PantryShopItem pantryShopItem = item.gameObject.AddComponent<PantryShopItem>();
			pantryShopItem.SetFoodData(m_PurchaseablePantryFoodData[index]);
			item.SetUp(0.6f);
		}

		public override bool IsUnlocked(int index)
		{
			SpecialItemShopItemIndex specialItemShopItemIndex = m_ItemIndices[index];
			switch (specialItemShopItemIndex.m_Type)
			{
			case SpecialItemShopItemIndexType.PromoEgg:
				return IsUnlocked_AsPromoEgg(specialItemShopItemIndex.m_Index);
			case SpecialItemShopItemIndexType.PlayroomItem:
				return IsUnlocked_AsPlayroomItem(specialItemShopItemIndex.m_Index);
			case SpecialItemShopItemIndexType.PantryItem:
				return IsUnlocked_AsPantryItem(specialItemShopItemIndex.m_Index);
			default:
				return false;
			}
		}

		private bool IsUnlocked_AsPlayroomItem(int index)
		{
			IPlayroomSelectable shopItem = m_AllPlayroomItems[index];
			return WholeGameShopHelpers.IsItemUnlocked(shopItem);
		}

		private bool IsUnlocked_AsPromoEgg(int index)
		{
			bool result = false;
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if (allFurbling.Type.Equals(m_PromoEggData.m_eggsAvailable[index].m_babyInfo.TypeID))
				{
					result = true;
				}
			}
			return result;
		}

		private bool IsUnlocked_AsPantryItem(int index)
		{
			PantryFoodData foodData = m_PurchaseablePantryFoodData[index];
			return WholeGameShopHelpers.IsItemUnlocked(foodData);
		}

		public override string DEBUG_GetItemName(int index)
		{
			SpecialItemShopItemIndex specialItemShopItemIndex = m_ItemIndices[index];
			string key = string.Empty;
			switch (specialItemShopItemIndex.m_Type)
			{
			case SpecialItemShopItemIndexType.PromoEgg:
				key = m_PromoEggData.m_eggsAvailable[specialItemShopItemIndex.m_Index].m_spriteName;
				break;
			case SpecialItemShopItemIndexType.PlayroomItem:
				key = m_AllPlayroomItems[specialItemShopItemIndex.m_Index].GetName();
				break;
			case SpecialItemShopItemIndexType.PantryItem:
				key = m_PurchaseablePantryFoodData[specialItemShopItemIndex.m_Index].DisplayName;
				break;
			}
			return Singleton<Localisation>.Instance.GetText(key);
		}

		public override void DEBUG_UnlockItem(int index)
		{
			SpecialItemShopItemIndex specialItemShopItemIndex = m_ItemIndices[index];
			switch (specialItemShopItemIndex.m_Type)
			{
			case SpecialItemShopItemIndexType.PromoEgg:
			{
				FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_PromoEggData.m_eggsAvailable[specialItemShopItemIndex.m_Index].m_babyInfo.TypeID);
				furbyBaby.Progress = FurbyBabyProgresss.E;
				break;
			}
			case SpecialItemShopItemIndexType.PlayroomItem:
				WholeGameShopHelpers.PurchaseItem(m_AllPlayroomItems[specialItemShopItemIndex.m_Index]);
				break;
			case SpecialItemShopItemIndexType.PantryItem:
				WholeGameShopHelpers.PurchaseItem(m_PurchaseablePantryFoodData[specialItemShopItemIndex.m_Index]);
				break;
			}
		}
	}
}
