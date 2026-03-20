using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypePromoEgg : ShopType
	{
		[SerializeField]
		private PromoEggData m_eggData;

		public override void RefreshList()
		{
		}

		public override int GetNumItems()
		{
			return m_eggData.m_eggsAvailable.Length;
		}

		public override void SetUpItem_Virtual(ShopItemUI item, int index)
		{
			PromoEggShopItem promoEggShopItem = item.gameObject.AddComponent<PromoEggShopItem>();
			promoEggShopItem.m_purchasableEggInfo = m_eggData.m_eggsAvailable[index];
			item.SetUp(1f);
		}

		public override bool IsUnlocked(int index)
		{
			bool result = false;
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if (allFurbling.Type.Equals(m_eggData.m_eggsAvailable[index].m_babyInfo.TypeID))
				{
					result = true;
				}
			}
			return result;
		}

		public override string DEBUG_GetItemName(int index)
		{
			return Singleton<Localisation>.Instance.GetText("PROMO_EGG_" + m_eggData.m_eggsAvailable[index].m_babyInfo.Code);
		}

		public override void DEBUG_UnlockItem(int index)
		{
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_eggData.m_eggsAvailable[index].m_babyInfo.TypeID);
			furbyBaby.Progress = FurbyBabyProgresss.E;
		}
	}
}
