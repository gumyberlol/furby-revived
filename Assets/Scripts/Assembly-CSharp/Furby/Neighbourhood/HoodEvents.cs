using Relentless;

namespace Furby.Neighbourhood
{
	[GameEventEnum]
	public enum HoodEvents
	{
		Hood_NewResident_BeingPlaced = 0,
		Hood_NewResident_Placed = 1,
		Hood_Opened = 2,
		Hood_Closed = 3,
		Hood_MainTribePopulationCount = 4,
		Hood_PromoPopulationCount = 5,
		Hood_TransitionSequencer = 6,
		Hood_NewResident_PlacedFully = 7,
		Hood_BabySelected = 8,
		Hood_BabyPlayroomConfirmed = 9,
		Hood_BabyDeleteConfirmed = 10,
		Hood_BabyDeleteReload = 11,
		Hood_FlagRisesAtStart = 12,
		Hood_FlagRisesAfterVanDelivery = 13,
		Hood_GenericAccept = 14,
		Hood_GenericDecline = 15,
		Hood_BabyDeleteCompleted = 16,
		Hood_GoldFurblingWon = 17,
		Hood_NewResident_BeingPlacedGold = 18,
		Hood_GoldFurblingPresent = 19,
		Hood_FirstOfATribe = 20,
		Hood_SpringGemFurblingWon = 21,
		Hood_SpringGemFurblingPresent = 22,
		Hood_NewResident_BeingPlacedSpringGem = 23,
		Hood_BlimpCropDust_Begin = 24,
		Hood_BlimpCropDust_End = 25,
		Hood_GoldenTowers_Begin = 26,
		Hood_GoldenTowers_End = 27,
		Hood_GoldenTowerSequenceScheduled = 28,
		Hood_CrystalGemFurblingWon = 29,
		Hood_CrystalGemFurblingPresent = 30,
		Hood_NewResident_BeingPlacedHood_CrystalGem = 31,
		Hood_CrystalUFO_Leaves = 32
	}
}
