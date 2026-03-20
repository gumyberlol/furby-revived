using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum PlayerFurbyCommand
	{
		Unintialised = 0,
		AwardAdultXP = 1,
		UpdateAdultLevel = 2,
		TurnOnNoFurbyMode = 16,
		TurnOffNoFurbyMode = 17
	}
}
