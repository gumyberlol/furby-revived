using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum BabyLifecycleEvent
	{
		FromOwnFurbyToy = 0,
		FromVirtualFurby = 1,
		FromSpecialFriend = 2,
		FromFriendsFurbyToy = 3,
		FromQRCode = 4,
		FromShop = 5,
		FromFriendsDevice = 6,
		BabyHatched = 256,
		BabyNamed = 257,
		BabyGraduated = 512
	}
}
