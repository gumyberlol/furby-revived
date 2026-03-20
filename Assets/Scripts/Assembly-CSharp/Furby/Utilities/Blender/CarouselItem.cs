using Relentless;

namespace Furby.Utilities.Blender
{
	public class CarouselItem : InGamePurchaseableItem
	{
		public Ingredient m_ItemIngredient;

		public override void OnClickAlreadyPurchased()
		{
			Singleton<Blender>.Instance.OnInsertItem(this);
		}

		public override int GetFurbucksCost()
		{
			return m_ItemIngredient.Cost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_ItemIngredient.Name);
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_ItemIngredient);
		}

		public override bool ShouldUseAfterPurchase()
		{
			return true;
		}
	}
}
