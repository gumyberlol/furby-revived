using Relentless;

namespace Furby
{
	public class SelectableFeatureShopItem : ShopPurchaseableItem
	{
		private IPlayroomSelectable m_feature;

		public IPlayroomSelectable Feature
		{
			get
			{
				return m_feature;
			}
		}

		public void SetFeature(IPlayroomSelectable feature)
		{
			m_feature = feature;
		}

		public override int GetFurbucksCost()
		{
			return m_feature.GetCost();
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_feature.GetName());
		}

		public override bool IsPurchased()
		{
			return WholeGameShopHelpers.IsItemUnlocked(m_feature);
		}

		public override string GetSpriteName()
		{
			return m_feature.GetSpriteName();
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_feature);
		}

		public override UIAtlas GetAtlas()
		{
			return m_feature.GetUIAtlas();
		}
	}
}
