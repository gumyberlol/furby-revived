using System;

namespace Furby
{
	[Serializable]
	public class FurbyBabyPersistantData
	{
		public string tribe;

		public int iter;

		public string nameL;

		public string nameR;

		public bool hasBeenNamed;

		public FurbyBabyProgresss prog;

		public FurbyBabyPersonality personality;

		public string[] foodLikes = new string[3]
		{
			string.Empty,
			string.Empty,
			string.Empty
		};

		public string[] foodDislikes = new string[3]
		{
			string.Empty,
			string.Empty,
			string.Empty
		};

		public string[] styleLikes = new string[3]
		{
			string.Empty,
			string.Empty,
			string.Empty
		};

		public string[] styleDislikes = new string[3]
		{
			string.Empty,
			string.Empty,
			string.Empty
		};

		public int neighbourhoodIndex;

		public string[] flairs = new string[1] { "Top Hat" };

		public bool newToCarton;

		public bool cameFromFriendsBook;

		public int IncubationSeed;

		public float IncubationProgress;

		public float IncubationAttentionProbability = 1f;

		public FurbyPersonality[] IncubationPersonalities = new FurbyPersonality[0];

		public bool IncubationPersonalityOverriden;

		public bool IncubationFastForwarded;

		public float IncubationDuration;

		public long IncubationTime;

		public long NextAttentionPointTime = DateTime.MaxValue.Ticks;

		public bool PlayerNotifiedOfAttentionPoint;

		public float Attention = 1f;

		public float Cleanliness = 1f;

		public float Satiatedness = 1f;

		public float NewAttention = 1f;

		public float NewCleanliness = 1f;

		public float NewSatiatedness = 1f;

		public long TimeOfLastStatUpdate;

		public int XP;

		public int EarnedXP;

		public PlayroomCustomizationSettings PlayroomCustomizations = new PlayroomCustomizationSettings();

		public long LayingTime = DateTime.MinValue.Ticks;

		public long HatchingTime = DateTime.MinValue.Ticks;

		public long GraduationTime = DateTime.MinValue.Ticks;

		public bool CanBeGifted = true;

		public bool FixedIncubationTime;

		public bool PreAllocatedPersonality;
	}
}
