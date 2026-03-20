using Furby.Utilities.Blender;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopTypeBlender : ShopType
	{
		[SerializeField]
		private IngredientList m_ingredientList;

		public override int GetNumItems()
		{
			return m_ingredientList.Items.Count;
		}

		public override void SetUpItem_Virtual(ShopItemUI item, int index)
		{
			BabyUtilityShopItem babyUtilityShopItem = item.gameObject.AddComponent<BabyUtilityShopItem>();
			babyUtilityShopItem.SetIngredient(m_ingredientList.Items[index]);
			item.SetUp(0.6f);
		}

		public override bool IsUnlocked(int index)
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_ingredientList.Items[index]);
		}

		public override string DEBUG_GetItemName(int index)
		{
			return Singleton<Localisation>.Instance.GetText(m_ingredientList.Items[index].Name);
		}

		public override void DEBUG_UnlockItem(int index)
		{
			WholeGameShopHelpers.PurchaseItem(m_ingredientList.Items[index]);
		}
	}
}
