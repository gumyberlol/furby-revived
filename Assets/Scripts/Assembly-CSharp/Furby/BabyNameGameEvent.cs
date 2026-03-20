using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum BabyNameGameEvent
	{
		NamingScreenOpen = 0,
		NamingScreenClosed = 1,
		NamingScreenLeftScrollSnapCentre = 2,
		NamingScreenLeftScrollSpeed = 3,
		NamingScreenRightScrollSnapCentre = 4,
		NamingScreenRightScrollSpeed = 5,
		NamingScreenConfirmName = 6
	}
}
