using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurbyStatEvent
	{
		UpdateFurbySatietedness = 0,
		UpdateFurbyCleanliness = 1,
		UpdateFurbyHappiness = 2,
		UpdateFurbyBowelEmptiness = 3,
		UpdateFurbyBabySatiatedness = 16,
		UpdateFurbyBabyCleanliness = 17,
		UpdateFurbyBabyAttention = 18
	}
}
