using System.Collections.Generic;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypeSalon : ShopType
	{
		[SerializeField]
		private SalonItemList[] m_salonItemLists;

		private List<SalonItem> m_combinedList = new List<SalonItem>();

		private void Awake()
		{
			SalonItemList[] salonItemLists = m_salonItemLists;
			foreach (SalonItemList salonItemList in salonItemLists)
			{
				foreach (SalonItem item in salonItemList.Items)
				{
					m_combinedList.Add(item);
				}
			}
		}

		public override int GetNumItems()
		{
			return m_combinedList.Count;
		}

		public override void SetUpItem_Virtual(ShopItemUI item, int index)
		{
			BabyUtilityShopItem babyUtilityShopItem = item.gameObject.AddComponent<BabyUtilityShopItem>();
			babyUtilityShopItem.SetIngredient(m_combinedList[index]);
			item.SetUp(0.6f);
		}

		public override bool IsUnlocked(int index)
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_combinedList[index]);
		}

		public override string DEBUG_GetItemName(int index)
		{
			return Singleton<Localisation>.Instance.GetText(m_combinedList[index].Name);
		}

		public override void DEBUG_UnlockItem(int index)
		{
			WholeGameShopHelpers.PurchaseItem(m_combinedList[index]);
		}
	}
}
