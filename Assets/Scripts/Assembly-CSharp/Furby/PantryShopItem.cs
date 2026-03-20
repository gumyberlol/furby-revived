using Furby.Utilities.Pantry;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PantryShopItem : ShopPurchaseableItem
	{
		[SerializeField]
		private PantryFoodData m_FoodData;

		public void SetFoodData(PantryFoodData food)
		{
			m_FoodData = food;
		}

		public override int GetFurbucksCost()
		{
			return m_FoodData.m_FurbuckCost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_FoodData.DisplayName);
		}

		public override bool IsPurchased()
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_FoodData);
		}

		public override string GetSpriteName()
		{
			return m_FoodData.GraphicName;
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_FoodData);
		}

		public override UIAtlas GetAtlas()
		{
			return m_FoodData.GraphicAtlas;
		}
	}
}
