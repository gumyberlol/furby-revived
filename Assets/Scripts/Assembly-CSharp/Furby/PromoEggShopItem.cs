using System.Linq;
using Furby.EggTransfer;
using Relentless;

namespace Furby
{
	public class PromoEggShopItem : ShopPurchaseableItem
	{
		public PromoEggData.PurchaseableEgg m_purchasableEggInfo;

		public override int GetFurbucksCost()
		{
			return m_purchasableEggInfo.m_furbucksCost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText("PROMO_EGG_" + m_purchasableEggInfo.m_babyInfo.Code);
		}

		public override bool IsPurchased()
		{
			bool result = false;
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if (allFurbling.Type.Equals(m_purchasableEggInfo.m_babyInfo.TypeID))
				{
					result = true;
				}
			}
			return result;
		}

		public override bool AttemptPurchaseIsValid()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				GameEventRouter.SendEvent(EggTransferGameEvent.EggCartonFull);
				return false;
			}
			return true;
		}

		public override string GetSpriteName()
		{
			return m_purchasableEggInfo.m_spriteName;
		}

		public override void Purchase()
		{
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_purchasableEggInfo.m_babyInfo.TypeID);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			GameEventRouter.SendEvent(BabyLifecycleEvent.FromShop, null, furbyBaby);
			GameEventRouter.SendEvent(ShopGameEvents.EggBought);
		}

		public override UIAtlas GetAtlas()
		{
			return m_purchasableEggInfo.m_atlas;
		}
	}
}
