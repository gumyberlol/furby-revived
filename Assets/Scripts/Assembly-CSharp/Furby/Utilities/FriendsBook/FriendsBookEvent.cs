using Relentless;

namespace Furby.Utilities.FriendsBook
{
	[GameEventEnum]
	public enum FriendsBookEvent
	{
		Enter = 0,
		Exit = 1,
		ClickOnFurby = 2,
		ClickOnLockedFurby = 3,
		ClickOnEmptyFurby = 4,
		EggBought = 5,
		NotEnoughMoney = 6,
		EggCartonFull = 7
	}
}
