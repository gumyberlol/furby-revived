using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class FurbyBaby
	{
		private const int ITEM_SCORE_DISLIKE = 0;

		private const int ITEM_SCORE_LIKE = 5;

		[SerializeField]
		public FurbyBabyPersistantData m_persistantData = new FurbyBabyPersistantData();

		private static readonly int[] s_IterationLevel = new int[9] { 0, 0, 1, 1, 2, 2, 2, 3, 4 };

		public FurbyTribeType TribeAll
		{
			get
			{
				if (m_persistantData.tribe == null)
				{
					return null;
				}
				if (Singleton<FurbyGlobals>.Exists)
				{
					return FurbyGlobals.BabyLibrary.GetTribeByName(m_persistantData.tribe, true);
				}
				return null;
			}
		}

		public FurbyTribeType Tribe
		{
			get
			{
				if (m_persistantData.tribe == null)
				{
					return null;
				}
				if (Singleton<FurbyGlobals>.Exists)
				{
					return FurbyGlobals.BabyLibrary.GetTribeByName(m_persistantData.tribe);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					m_persistantData.tribe = null;
				}
				else
				{
					m_persistantData.tribe = value.Name;
				}
			}
		}

		public FurbyBabyTypeID Type
		{
			get
			{
				return new FurbyBabyTypeID(Tribe, m_persistantData.iter);
			}
		}

		public FurbyBabyTypeID TypeAll
		{
			get
			{
				return new FurbyBabyTypeID(TribeAll, m_persistantData.iter);
			}
		}

		public string Name
		{
			get
			{
				return string.Format("{0}-{1}", m_persistantData.nameL, m_persistantData.nameR);
			}
		}

		public string NameLeft
		{
			get
			{
				return m_persistantData.nameL;
			}
			set
			{
				m_persistantData.nameL = value;
			}
		}

		public string NameRight
		{
			get
			{
				return m_persistantData.nameR;
			}
			set
			{
				m_persistantData.nameR = value;
			}
		}

		public FurbyBabyProgresss Progress
		{
			get
			{
				return m_persistantData.prog;
			}
			set
			{
				m_persistantData.prog = value;
			}
		}

		public bool HasBeenNamed
		{
			get
			{
				return m_persistantData.hasBeenNamed;
			}
			set
			{
				m_persistantData.hasBeenNamed = value;
			}
		}

		public FurbyBabyPersonality Personality
		{
			get
			{
				return m_persistantData.personality;
			}
		}

		public bool CanBeGifted
		{
			get
			{
				return m_persistantData.CanBeGifted;
			}
			set
			{
				m_persistantData.CanBeGifted = value;
			}
		}

		public bool FixedIncubationTime
		{
			get
			{
				return m_persistantData.FixedIncubationTime;
			}
		}

		public bool PreAllocatedPersonality
		{
			get
			{
				return m_persistantData.PreAllocatedPersonality;
			}
		}

		public float Attention
		{
			get
			{
				return m_persistantData.Attention;
			}
		}

		public float Cleanliness
		{
			get
			{
				return m_persistantData.Cleanliness;
			}
		}

		public float Satiatedness
		{
			get
			{
				return m_persistantData.Satiatedness;
			}
		}

		public float NewAttention
		{
			get
			{
				return m_persistantData.NewAttention;
			}
			set
			{
				m_persistantData.NewAttention = Mathf.Clamp01(value);
			}
		}

		public float NewCleanliness
		{
			get
			{
				return m_persistantData.NewCleanliness;
			}
			set
			{
				m_persistantData.NewCleanliness = Mathf.Clamp01(value);
			}
		}

		public float NewSatiatedness
		{
			get
			{
				return m_persistantData.NewSatiatedness;
			}
			set
			{
				m_persistantData.NewSatiatedness = Mathf.Clamp01(value);
			}
		}

		public DateTime TimeOfLastStatUpdate
		{
			get
			{
				return new DateTime(m_persistantData.TimeOfLastStatUpdate);
			}
			set
			{
				m_persistantData.TimeOfLastStatUpdate = value.Ticks;
			}
		}

		public int LevelAll
		{
			get
			{
				if (TribeAll.TribeSet == Tribeset.Promo)
				{
					return 0;
				}
				return GetLevelForIteration(Iteration);
			}
		}

		public int Level
		{
			get
			{
				if (Tribe.TribeSet == Tribeset.Promo)
				{
					return 0;
				}
				return GetLevelForIteration(Iteration);
			}
		}

		public int NeighbourhoodIndex
		{
			get
			{
				return m_persistantData.neighbourhoodIndex;
			}
			set
			{
				m_persistantData.neighbourhoodIndex = value;
			}
		}

		public int Iteration
		{
			get
			{
				return m_persistantData.iter;
			}
			set
			{
				m_persistantData.iter = value;
			}
		}

		public int IncubationSeed
		{
			get
			{
				return m_persistantData.IncubationSeed;
			}
			set
			{
				m_persistantData.IncubationSeed = value;
			}
		}

		public DateTime LastIncubationTime
		{
			get
			{
				return new DateTime(m_persistantData.IncubationTime);
			}
			set
			{
				m_persistantData.IncubationTime = value.Ticks;
			}
		}

		public DateTime NextAttentionPointTime
		{
			get
			{
				return new DateTime(m_persistantData.NextAttentionPointTime);
			}
			set
			{
				m_persistantData.PlayerNotifiedOfAttentionPoint = false;
				m_persistantData.NextAttentionPointTime = value.Ticks;
			}
		}

		public float IncubationDuration
		{
			get
			{
				return m_persistantData.IncubationDuration;
			}
			set
			{
				m_persistantData.IncubationDuration = value;
			}
		}

		public float IncubationProgress
		{
			get
			{
				return m_persistantData.IncubationProgress;
			}
			set
			{
				m_persistantData.IncubationProgress = value;
			}
		}

		public float IncubationAttentionProbability
		{
			get
			{
				return m_persistantData.IncubationAttentionProbability;
			}
			set
			{
				m_persistantData.IncubationAttentionProbability = value;
			}
		}

		public FurbyPersonality[] IncubationPersonalities
		{
			get
			{
				return m_persistantData.IncubationPersonalities;
			}
			set
			{
				m_persistantData.IncubationPersonalities = value;
			}
		}

		public bool IncubationPersonalityOverriden
		{
			get
			{
				return m_persistantData.IncubationPersonalityOverriden;
			}
			set
			{
				m_persistantData.IncubationPersonalityOverriden = value;
			}
		}

		public bool IncubationFastForwarded
		{
			get
			{
				return m_persistantData.IncubationFastForwarded;
			}
			set
			{
				m_persistantData.IncubationFastForwarded = value;
			}
		}

		public bool PlayerNotifiedOfAttentionPoint
		{
			get
			{
				return m_persistantData.PlayerNotifiedOfAttentionPoint;
			}
			set
			{
				m_persistantData.PlayerNotifiedOfAttentionPoint = value;
			}
		}

		public PlayroomCustomizationSettings PlayroomCustomizations
		{
			get
			{
				return m_persistantData.PlayroomCustomizations;
			}
			set
			{
				m_persistantData.PlayroomCustomizations = value;
			}
		}

		public string[] FoodLikes
		{
			get
			{
				if (m_persistantData.foodLikes == null)
				{
					m_persistantData.foodLikes = new string[3]
					{
						string.Empty,
						string.Empty,
						string.Empty
					};
				}
				return m_persistantData.foodLikes;
			}
		}

		public string[] FoodDislikes
		{
			get
			{
				if (m_persistantData.foodDislikes == null)
				{
					m_persistantData.foodDislikes = new string[3]
					{
						string.Empty,
						string.Empty,
						string.Empty
					};
				}
				return m_persistantData.foodDislikes;
			}
		}

		public string[] StyleLikes
		{
			get
			{
				if (m_persistantData.styleLikes == null)
				{
					m_persistantData.styleLikes = new string[3]
					{
						string.Empty,
						string.Empty,
						string.Empty
					};
				}
				return m_persistantData.styleLikes;
			}
		}

		public string[] StyleDislikes
		{
			get
			{
				if (m_persistantData.styleDislikes == null)
				{
					m_persistantData.styleDislikes = new string[3]
					{
						string.Empty,
						string.Empty,
						string.Empty
					};
				}
				return m_persistantData.styleDislikes;
			}
		}

		public string FoodLikesString
		{
			get
			{
				return LikeOrDislikeAsString(FoodLikes);
			}
		}

		public string FoodDislikesString
		{
			get
			{
				return LikeOrDislikeAsString(FoodDislikes);
			}
		}

		public string StyleLikesString
		{
			get
			{
				return LikeOrDislikeAsString(StyleLikes);
			}
		}

		public string StyleDislikesString
		{
			get
			{
				return LikeOrDislikeAsString(StyleDislikes);
			}
		}

		public int EarnedXP
		{
			get
			{
				return m_persistantData.EarnedXP;
			}
			set
			{
				m_persistantData.EarnedXP = value;
			}
		}

		public int XP
		{
			get
			{
				return m_persistantData.XP;
			}
		}

		public static int GetLevelForIteration(int iteration)
		{
			return s_IterationLevel[Mathf.Clamp(iteration, 1, s_IterationLevel.Length) - 1];
		}

		public void SetPersonality(FurbyBabyPersonality personality, int level)
		{
			m_persistantData.personality = personality;
			m_persistantData.flairs = new string[0];
			List<string> list = new List<string>();
			if (Singleton<FurbyGlobals>.Exists && FurbyGlobals.PersonalityLibrary != null)
			{
				PersonalityLibrary.PersonalityData personalityData = FurbyGlobals.PersonalityLibrary.GetPersonalityData(personality);
				if (personalityData != null)
				{
					list.AddRange(personalityData.flairs);
				}
			}
			if (level >= 3)
			{
				list.Add(FurbyGlobals.PersonalityLibrary.GetRandomAdditionalFlair());
			}
			m_persistantData.flairs = list.ToArray();
		}

		private string LikeOrDislikeAsString(string[] items)
		{
			string[] array = new string[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				string text = items[i];
				array[i] = ((!string.IsNullOrEmpty(text)) ? Singleton<Localisation>.Instance.GetText(text) : "???");
			}
			return string.Join(", ", array);
		}

		public static FurbyBaby Create(FurbyBabyTypeID type, FurbyNamingData nameData)
		{
			return Create(type, nameData, false);
		}

		public static FurbyBaby Create(FurbyBabyTypeID type, FurbyNamingData nameData, bool listAll)
		{
			FurbyBaby furbyBaby = new FurbyBaby();
			furbyBaby.Tribe = type.Tribe;
			furbyBaby.m_persistantData.iter = type.Iteration;
			if (nameData != null)
			{
				furbyBaby.NameLeft = nameData.m_leftNames[UnityEngine.Random.Range(0, nameData.m_leftNames.Length)];
				furbyBaby.NameRight = nameData.m_rightNames[UnityEngine.Random.Range(0, nameData.m_rightNames.Length)];
			}
			else
			{
				furbyBaby.NameLeft = "BAY";
				furbyBaby.NameRight = "BEE";
			}
			furbyBaby.HasBeenNamed = false;
			furbyBaby.Progress = FurbyBabyProgresss.E;
			furbyBaby.SetPersonality(FurbyBabyPersonality.None, (!listAll) ? furbyBaby.Level : furbyBaby.LevelAll);
			furbyBaby.m_persistantData.flairs = new string[0];
			furbyBaby.m_persistantData.newToCarton = true;
			furbyBaby.m_persistantData.Attention = 1f;
			furbyBaby.m_persistantData.Cleanliness = 1f;
			furbyBaby.m_persistantData.Satiatedness = 1f;
			furbyBaby.m_persistantData.NewAttention = 1f;
			furbyBaby.m_persistantData.NewCleanliness = 1f;
			furbyBaby.m_persistantData.NewSatiatedness = 1f;
			furbyBaby.m_persistantData.TimeOfLastStatUpdate = 0L;
			furbyBaby.m_persistantData.IncubationTime = 0L;
			furbyBaby.m_persistantData.IncubationProgress = 0f;
			if (furbyBaby.Tribe.TribeSet == Tribeset.Promo || furbyBaby.Tribe.TribeSet == Tribeset.Golden || furbyBaby.Tribe.TribeSet == Tribeset.CrystalGolden)
			{
				furbyBaby.CanBeGifted = false;
			}
			return furbyBaby;
		}

		public void SetLatestPersonality(FurbyPersonality personality)
		{
			FurbyPersonality[] array = IncubationPersonalities;
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = personality;
			IncubationPersonalities = array;
		}

		public FurbyPersonality? GetLatestPersonality()
		{
			FurbyPersonality? result = null;
			if (IncubationPersonalities.Length > 0)
			{
				result = IncubationPersonalities[IncubationPersonalities.Length - 1];
			}
			return result;
		}

		public FurbyBabyPersonality CombinePersonalities()
		{
			Type typeFromHandle = typeof(FurbyBabyPersonality);
			FurbyBabyPersonality furbyBabyPersonality = FurbyBabyPersonality.None;
			FurbyPersonality[] incubationPersonalities = IncubationPersonalities;
			foreach (FurbyPersonality furbyPersonality in incubationPersonalities)
			{
				furbyBabyPersonality |= MapPersonality(furbyPersonality);
			}
			if (Enum.IsDefined(typeFromHandle, furbyBabyPersonality))
			{
				return furbyBabyPersonality;
			}
			FurbyPersonality? latestPersonality = GetLatestPersonality();
			return MapPersonality((!latestPersonality.HasValue) ? FurbyPersonality.Gobbler : latestPersonality.Value);
		}

		public static FurbyBabyPersonality MapPersonality(FurbyPersonality furbyPersonality)
		{
			switch (furbyPersonality)
			{
			case FurbyPersonality.Gobbler:
				return FurbyBabyPersonality.Gobbler;
			case FurbyPersonality.Kooky:
				return FurbyBabyPersonality.Kooky;
			case FurbyPersonality.Base:
				return FurbyBabyPersonality.RockStar;
			case FurbyPersonality.SweetBelle:
				return FurbyBabyPersonality.SweetBelle;
			default:
				return FurbyBabyPersonality.ToughGirl;
			}
		}

		public int GetEffectiveLevel()
		{
			if (FurbyGlobals.Player.IncubatorEggsHatched == 0)
			{
				return 999;
			}
			switch (Tribe.TribeSet)
			{
			case Tribeset.Promo:
				return 0;
			case Tribeset.Golden:
			case Tribeset.CrystalGolden:
				return 4;
			default:
				return Level;
			}
		}

		public FurbyBabyPersistantData Save()
		{
			return m_persistantData;
		}

		public bool UpdateXP()
		{
			bool result = false;
			if (m_persistantData.EarnedXP > 0)
			{
				m_persistantData.XP += m_persistantData.EarnedXP;
				GameEventRouter.SendEvent(PlayerFurbyEvent.BabyGainedXP, null, m_persistantData.XP, m_persistantData.EarnedXP);
				m_persistantData.EarnedXP = 0;
				if (Progress != FurbyBabyProgresss.N && m_persistantData.XP >= FurbyGlobals.BabyLibrary.GetBabyFurby(Type).XpToLevelUp)
				{
					GameEventRouter.SendEvent(PlayerFurbyEvent.BabyGraduated, null, m_persistantData.XP, m_persistantData.EarnedXP);
					result = true;
				}
			}
			Singleton<GameDataStoreObject>.Instance.Save();
			return result;
		}

		public void OnFoodFeedback(string ingredientName, int score)
		{
			switch (score)
			{
			case 0:
				UpdateLikeOrDislike(FoodDislikes, ingredientName);
				break;
			case 5:
				UpdateLikeOrDislike(FoodLikes, ingredientName);
				break;
			}
		}

		public void OnStyleFeedback(string styleName, int score)
		{
			switch (score)
			{
			case 0:
				UpdateLikeOrDislike(StyleDislikes, styleName);
				break;
			case 5:
				UpdateLikeOrDislike(StyleLikes, styleName);
				break;
			}
		}

		private void UpdateLikeOrDislike(string[] list, string itemName)
		{
			int num = -1;
			for (int i = 0; i < list.Length; i++)
			{
				if (string.Compare(list[i], itemName) == 0)
				{
					return;
				}
				if (num < 0 && string.IsNullOrEmpty(list[i]))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				list[num] = itemName;
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public bool ShouldProgressToNeighbourhood()
		{
			if (Progress != FurbyBabyProgresss.N && m_persistantData.XP >= FurbyGlobals.BabyLibrary.GetBabyFurby(Type).XpToLevelUp)
			{
				GameEventRouter.SendEvent(PlayerFurbyEvent.BabyGraduated, null, m_persistantData.XP, m_persistantData.EarnedXP);
				return true;
			}
			return false;
		}

		private float CalculateUpdatedStat(float oldStat, float newStat, float reductionRate)
		{
			float result = oldStat;
			if (oldStat != newStat)
			{
				result = newStat;
			}
			else if (m_persistantData.TimeOfLastStatUpdate != 0L)
			{
				double totalSeconds = (DateTime.Now - TimeOfLastStatUpdate).TotalSeconds;
				float num = Mathf.Clamp01((float)(totalSeconds / (double)reductionRate));
				result = Mathf.Clamp01(oldStat - num);
			}
			return result;
		}

		public void UpdateStats()
		{
			if (m_persistantData.TimeOfLastStatUpdate != 0L)
			{
				FurbyBabyStatRates furbyBabyStatRates = ((!(TimeOfLastStatUpdate < DateTime.Now.AddSeconds(0f - Time.realtimeSinceStartup))) ? FurbyGlobals.BabyLibrary.InGameStatRates : FurbyGlobals.BabyLibrary.OutOfGameStatRates);
				m_persistantData.Satiatedness = CalculateUpdatedStat(m_persistantData.Satiatedness, m_persistantData.NewSatiatedness, furbyBabyStatRates.SatiatednessSecondsToZero);
				m_persistantData.NewSatiatedness = m_persistantData.Satiatedness;
				m_persistantData.Cleanliness = CalculateUpdatedStat(m_persistantData.Cleanliness, m_persistantData.NewCleanliness, furbyBabyStatRates.CleanlinessSecondsToZero);
				m_persistantData.NewCleanliness = m_persistantData.Cleanliness;
				m_persistantData.Attention = CalculateUpdatedStat(m_persistantData.Attention, m_persistantData.NewAttention, furbyBabyStatRates.AttentionSecondsToZero);
				m_persistantData.NewAttention = m_persistantData.Attention;
			}
			TimeOfLastStatUpdate = DateTime.Now;
		}
	}
}
