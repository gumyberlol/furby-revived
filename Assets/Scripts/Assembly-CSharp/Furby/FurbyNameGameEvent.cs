using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurbyNameGameEvent
	{
		NamingScreenOpen = 0,
		NamingScreenClosed = 1,
		NamingScreenLeftScrollSnapCentre = 2,
		NamingScreenLeftScrollSpeed = 3,
		NamingScreenRightScrollSnapCentre = 4,
		NamingScreenRightScrollSpeed = 5,
		NamingScreenConfirmName = 6,
		NamingScreenFurbyConfirmsNameSent = 7,
		NamingScreenFurbyNameDidntSend = 8,
		NamingScreenSaveGameUpdated = 9
	}
}
