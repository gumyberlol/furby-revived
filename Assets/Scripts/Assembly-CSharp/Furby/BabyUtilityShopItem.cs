using System;
using Furby.Utilities;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyUtilityShopItem : ShopPurchaseableItem
	{
		[SerializeField]
		private BabyUtilityItem m_utilityItem;

		public Type UtilityType
		{
			get
			{
				return m_utilityItem.GetType();
			}
		}

		public void SetIngredient(BabyUtilityItem item)
		{
			m_utilityItem = item;
		}

		public override int GetFurbucksCost()
		{
			return m_utilityItem.Cost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_utilityItem.Name);
		}

		public override bool IsPurchased()
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_utilityItem);
		}

		public override string GetSpriteName()
		{
			return m_utilityItem.Graphic;
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_utilityItem);
		}

		public override UIAtlas GetAtlas()
		{
			return m_utilityItem.GraphicAtlas;
		}
	}
}
