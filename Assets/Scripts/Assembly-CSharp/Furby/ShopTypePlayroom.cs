using System.Collections.Generic;
using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypePlayroom : ShopType
	{
		[SerializeField]
		private SelectableFeatureList[] m_featureLists;

		[SerializeField]
		private SelectableThemeList[] m_themeLists;

		private List<IPlayroomSelectable> m_allFeatures = new List<IPlayroomSelectable>();

		private void Awake()
		{
			SelectableFeatureList[] featureLists = m_featureLists;
			foreach (SelectableFeatureList selectableFeatureList in featureLists)
			{
				foreach (SelectableFeature feature in selectableFeatureList.Features)
				{
					bool flag = ((IPlayroomSelectable)feature).IsAvailableForPurchase();
					bool flag2 = ((IPlayroomSelectable)feature).IsAvailableFromTheStart();
					bool flag3 = ((IPlayroomSelectable)feature).IsGoldenItem() && SelectableHelpers.HaveGoldenBaby();
					bool flag4 = ((IPlayroomSelectable)feature).IsUnlockedBySeason() && ((IPlayroomSelectable)feature).IsTheActualSeasonUnlocked();
					bool flag5 = ((IPlayroomSelectable)feature).IsUnlockedAsGift() && SelectableHelpers.HaveOpenedGift(((IPlayroomSelectable)feature).GetName());
					bool flag6 = ((IPlayroomSelectable)feature).IsUnlockedByCrystal() && Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
					bool flag7 = ((IPlayroomSelectable)feature).IsGoldenCrystalItem() && SelectableHelpers.HaveGoldenCrystalBaby();
					if (flag6 || flag || flag2 || flag4 || flag3 || flag5 || flag7)
					{
						m_allFeatures.Add(feature);
					}
				}
			}
			SelectableThemeList[] themeLists = m_themeLists;
			foreach (SelectableThemeList selectableThemeList in themeLists)
			{
				foreach (SelectableTheme theme in selectableThemeList.Themes)
				{
					bool flag8 = ((IPlayroomSelectable)theme).IsAvailableForPurchase();
					bool flag9 = ((IPlayroomSelectable)theme).IsAvailableFromTheStart();
					bool flag10 = ((IPlayroomSelectable)theme).IsGoldenItem() && SelectableHelpers.HaveGoldenBaby();
					bool flag11 = ((IPlayroomSelectable)theme).IsUnlockedBySeason() && ((IPlayroomSelectable)theme).IsTheActualSeasonUnlocked();
					bool flag12 = ((IPlayroomSelectable)theme).IsUnlockedAsGift() && SelectableHelpers.HaveOpenedGift(((IPlayroomSelectable)theme).GetName());
					bool flag13 = ((IPlayroomSelectable)theme).IsUnlockedByCrystal() && Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
					bool flag14 = ((IPlayroomSelectable)theme).IsGoldenCrystalItem() && SelectableHelpers.HaveGoldenCrystalBaby();
					if (flag13 || flag8 || flag9 || flag11 || flag10 || flag12 || flag14)
					{
						m_allFeatures.Add(theme);
					}
				}
			}
		}

		public override int GetNumItems()
		{
			return m_allFeatures.Count;
		}

		public override void SetUpItem_Virtual(ShopItemUI item, int index)
		{
			SelectableFeatureShopItem selectableFeatureShopItem = item.gameObject.AddComponent<SelectableFeatureShopItem>();
			selectableFeatureShopItem.SetFeature(m_allFeatures[index]);
			item.SetUp(1f);
		}

		public override bool IsUnlocked(int index)
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_allFeatures[index]);
		}

		public override string DEBUG_GetItemName(int index)
		{
			return Singleton<Localisation>.Instance.GetText(m_allFeatures[index].GetName());
		}

		public override void DEBUG_UnlockItem(int index)
		{
			WholeGameShopHelpers.PurchaseItem(m_allFeatures[index]);
		}
	}
}
