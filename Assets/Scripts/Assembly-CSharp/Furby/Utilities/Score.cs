using System;

namespace Furby.Utilities
{
	[Serializable]
	public class Score
	{
		public int ToughGirl;

		public int Kooky;

		public int RockStar;

		public int SweetBelle;

		public int Gobbler;

		public int Kooky_SweetBelle;

		public int RockStar_Gobbler;

		public int RockStar_Kooky;

		public int RockStar_ToughGirl;

		public int ToughGirl_Gobbler;

		public int ToughGirl_Kooky;

		public int ToughGirl_SweetBelle;

		public int RockStar_ToughGirl_Gobbler;

		public int RockStar_ToughGirl_Kooky;

		public int ToughGirl_Kooky_SweetBelle;

		public int this[FurbyBabyPersonality babyPersonality]
		{
			get
			{
				switch (babyPersonality)
				{
				case FurbyBabyPersonality.Gobbler:
					return Gobbler;
				case FurbyBabyPersonality.Kooky:
					return Kooky;
				case FurbyBabyPersonality.Kooky_SweetBelle:
					return Kooky_SweetBelle;
				case FurbyBabyPersonality.RockStar:
					return RockStar;
				case FurbyBabyPersonality.RockStar_Gobbler:
					return RockStar_Gobbler;
				case FurbyBabyPersonality.RockStar_Kooky:
					return RockStar_Kooky;
				case FurbyBabyPersonality.RockStar_ToughGirl:
					return RockStar_ToughGirl;
				case FurbyBabyPersonality.RockStar_ToughGirl_Gobbler:
					return RockStar_ToughGirl_Gobbler;
				case FurbyBabyPersonality.RockStar_ToughGirl_Kooky:
					return RockStar_ToughGirl_Kooky;
				case FurbyBabyPersonality.SweetBelle:
					return SweetBelle;
				case FurbyBabyPersonality.ToughGirl:
					return ToughGirl;
				case FurbyBabyPersonality.ToughGirl_Gobbler:
					return ToughGirl_Gobbler;
				case FurbyBabyPersonality.ToughGirl_Kooky:
					return ToughGirl_Kooky;
				case FurbyBabyPersonality.ToughGirl_Kooky_SweetBelle:
					return ToughGirl_Kooky_SweetBelle;
				case FurbyBabyPersonality.ToughGirl_SweetBelle:
					return ToughGirl_SweetBelle;
				default:
					return 0;
				}
			}
		}
	}
}
