namespace Furby
{
	public interface IPlayroomSelectable
	{
		int GetCost();

		string GetName();

		UIAtlas GetUIAtlas();

		string GetSpriteName();

		bool IsGoldenItem();

		bool IsGoldenCrystalItem();

		bool IsAvailableForPurchase();

		bool IsAvailableFromTheStart();

		bool IsUnlockedByScannedQRCode();

		bool IsUnlockedByComAirTone();

		bool IsUnlockedBySeason();

		bool IsUnlockedByCrystal();

		bool IsTheActualSeasonUnlocked();

		bool IsUnlockedAsGift();

		string GetQRCode();

		string GetVariantCode();

		int GetComAirTone();
	}
}
