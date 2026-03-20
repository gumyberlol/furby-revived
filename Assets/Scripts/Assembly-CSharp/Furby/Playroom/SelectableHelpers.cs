using Relentless;

namespace Furby.Playroom
{
	public class SelectableHelpers
	{
		public static bool HaveGoldenBaby()
		{
			bool result = false;
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if ((allFurbling.Progress == FurbyBabyProgresss.P || allFurbling.Progress == FurbyBabyProgresss.N) && allFurbling.Tribe.TribeSet == Tribeset.Golden)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static bool HaveGoldenCrystalBaby()
		{
			bool result = false;
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if ((allFurbling.Progress == FurbyBabyProgresss.P || allFurbling.Progress == FurbyBabyProgresss.N) && allFurbling.Tribe.TribeSet == Tribeset.CrystalGolden)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static bool HaveOpenedGift(string giftname)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.HaveOpenedGift_ByName(giftname);
		}
	}
}
