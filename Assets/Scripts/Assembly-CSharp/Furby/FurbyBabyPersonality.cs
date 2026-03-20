using System;

namespace Furby
{
	[Flags]
	public enum FurbyBabyPersonality
	{
		None = 0,
		ToughGirl = 1,
		Kooky = 2,
		RockStar = 4,
		SweetBelle = 8,
		Gobbler = 0x10,
		Kooky_SweetBelle = 0xA,
		RockStar_Gobbler = 0x14,
		RockStar_Kooky = 6,
		RockStar_ToughGirl = 5,
		ToughGirl_Gobbler = 0x11,
		ToughGirl_Kooky = 3,
		ToughGirl_SweetBelle = 9,
		RockStar_ToughGirl_Gobbler = 0x15,
		RockStar_ToughGirl_Kooky = 7,
		ToughGirl_Kooky_SweetBelle = 0xB
	}
}
