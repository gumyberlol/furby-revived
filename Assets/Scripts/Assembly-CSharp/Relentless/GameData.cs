using System;
using System.Collections.Generic;
using Furby;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class GameData
	{
		public class SceneCount
		{
			public string scene;

			public uint count;

			public SceneCount()
			{
			}

			public SceneCount(string s)
			{
				scene = s;
				count = 0u;
			}
		}

		[Serializable]
		public class IAPReceipt
		{
			[SerializeField]
			public string m_ReceiptBase64;

			[SerializeField]
			public int m_ValidationCount;

			[SerializeField]
			public DateTime m_LastAttempt;

			public IAPReceipt(string receiptBase64)
			{
				m_ReceiptBase64 = receiptBase64;
				m_LastAttempt = DateTime.Now;
				m_ValidationCount = 1;
			}
		}

		public class IAPBundleStats
		{
			public string m_BundleID;

			public int m_PurchaseCount;
		}

		public float AudioVolume = 0.5f;

		public int Xp;

		private int m_earnedXP;

		public int Level;

		public int IncubatorFastForwards = 1;

		public int IncubatorEggsHatched;

		public int NumEggsAvailable;

		public int HideAndSeekLevel;

		public int FurbucksBalance = 2000;

		public HashSet<string> m_purchasedItems = new HashSet<string>();

		public List<FurbyBaby> BabyInstancesDontAccessDirectly = new List<FurbyBaby>();

		public int InProgressBabyIndex = -1;

		private int SelectedBabyIndex = -1;

		public bool HasCompletedFirstTimeFlow;

		public AdultFurbyType FurbyType = AdultFurbyType.Unknown;

		public AdultFurbyType NoFurbyUnlockType;

		public string FurbyNameLeft = "DEE";

		public string FurbyNameRight = "DOH";

		public string DefaultFurbyName = "DEE-DOH";

		private float m_happiness = 1f;

		public float Satiatedness = 1f;

		public float Cleanliness = 0.15f;

		public float BowelEmptiness = 0.75f;

		public float NewHappiness = 1f;

		public float NewSatiatedness = 1f;

		public float NewCleanliness = 0.15f;

		public float NewBowelEmptiness = 0.75f;

		public long TimeOfLastStatUpdate;

		public long TimeOfLastSave;

		public long TimeOfLastLoad;

		public long TimeSpentPlaying;

		public long TimeOfCreation = DateTime.UtcNow.Ticks;

		public int DashboardVisitCount;

		public FlowStage FlowStage = FlowStage.Normal;

		public List<AdultFurbyType> MetFurbies = new List<AdultFurbyType>();

		public List<string> RecognizedQRCodes = new List<string>();

		public List<int> RecognizedComAirTones = new List<int>();

		public List<string> m_videosPlayed = new List<string>();

		public List<string> m_helpPanelsShown = new List<string>();

		public long m_timeNextFriendEggTransferAllowed = DateTime.MinValue.Ticks;

		public List<string> m_unlockedSlangs = new List<string>();

		public List<string> m_unconfirmedSlangs = new List<string>();

		public int m_slangLastLevelSeen = 1;

		public string m_themeName = string.Empty;

		public long m_themeChoiceTicks;

		public bool m_unlocked;

		public List<SceneCount> m_sceneCounts = new List<SceneCount>();

		public int m_numFurblingsEverHatched;

		public int m_numFurblingsWhenGiftingFeatureAdded;

		public List<int> m_UnopenedGiftIndices = new List<int>();

		public List<int> m_OpenedGiftIndices = new List<int>();

		public uint m_numGiftsForWhichEligable;

		public int m_numNoFurbySessions;

		public int m_numFurbySessions;

		public bool m_numSessionsIsCountingFromStart = true;

		public GameConfigBlob m_LastGameConfig;

		public bool m_HaveStoredADownloadedGameConfig;

		public Dictionary<string, IAPReceipt> m_UnresolvedReceipts = new Dictionary<string, IAPReceipt>();

		public List<IAPBundleStats> m_BundleStats = new List<IAPBundleStats>();

		public bool m_HaveSentConversionTelemetry;

		public List<string> QRItemsUnlocked = new List<string>();

		public int EarnedXP
		{
			get
			{
				return m_earnedXP;
			}
			set
			{
				if (value < 0 || value > 1000)
				{
					Logging.LogError(string.Format("Requested EarnedXP value of {0} is probably a bug.", value));
				}
				m_earnedXP = value;
			}
		}

		public bool NoFurbyMode
		{
			get
			{
				return FurbyType == AdultFurbyType.NoFurby;
			}
		}

		public bool IsFurbyMode
		{
			get
			{
				return !NoFurbyMode;
			}
		}

		public bool IsActiveGame
		{
			get
			{
				return FurbyType != AdultFurbyType.Unknown;
			}
		}

		public float Happiness
		{
			get
			{
				return m_happiness;
			}
			set
			{
				m_happiness = value;
			}
		}

		public string ThemePeriod
		{
			get
			{
				return m_themeName;
			}
		}

		public DateTime ThemePeriodDateTime
		{
			get
			{
				return new DateTime(m_themeChoiceTicks);
			}
		}

		public AppLookAndFeel AppLookAndFeel { get; set; }

		public bool CanPlayAppChangeAnimation { get; set; }

		public int NumFurblingsSinceGifting
		{
			get
			{
				int numFurblingsEverHatched = m_numFurblingsEverHatched;
				int numFurblingsWhenGiftingFeatureAdded = m_numFurblingsWhenGiftingFeatureAdded;
				int num = numFurblingsEverHatched - numFurblingsWhenGiftingFeatureAdded;
				if (num < 0)
				{
					throw new ApplicationException(string.Format("Impossible NumFurblingsSinceGifting: now={0}, then={1}, result={2}", numFurblingsEverHatched, numFurblingsWhenGiftingFeatureAdded, num));
				}
				return num;
			}
		}

		public int NumGiftsAwarded
		{
			get
			{
				return m_UnopenedGiftIndices.Count + m_OpenedGiftIndices.Count;
			}
		}

		public bool HasEligibilityForGift
		{
			get
			{
				return m_numGiftsForWhichEligable != 0;
			}
		}

		public event EventHandler FurblingAdded;

		public event Action EggHatched;

		public int GetNumFurbyBabies()
		{
			return BabyInstancesDontAccessDirectly.Count;
		}

		public void AddNewFurbyBaby(FurbyBaby toAdd)
		{
			BabyInstancesDontAccessDirectly.Add(toAdd);
		}

		public void RemoveFurbyBaby(FurbyBaby toRemove)
		{
			FurbyBaby inProgressFurbyBaby = GetInProgressFurbyBaby();
			FurbyBaby selectedFurbyBaby = GetSelectedFurbyBaby();
			BabyInstancesDontAccessDirectly.Remove(toRemove);
			SetInProgressFurbyBaby(inProgressFurbyBaby);
			SetSelectedFurbyBaby(selectedFurbyBaby);
		}

		public FurbyBaby GetInProgressFurbyBaby()
		{
			return GetFurbyBabyByIndex(InProgressBabyIndex);
		}

		public FurbyBaby GetSelectedFurbyBaby()
		{
			if (SelectedBabyIndex == -1)
			{
				return GetFurbyBabyByIndex(InProgressBabyIndex);
			}
			return GetFurbyBabyByIndex(SelectedBabyIndex);
		}

		public void SetInProgressFurbyBaby(FurbyBaby toSet)
		{
			InProgressBabyIndex = -1;
			int count = BabyInstancesDontAccessDirectly.Count;
			for (int i = 0; i < count; i++)
			{
				if (toSet == BabyInstancesDontAccessDirectly[i])
				{
					InProgressBabyIndex = i;
					break;
				}
			}
		}

		public void SetSelectedFurbyBaby(FurbyBaby toSet)
		{
			SelectedBabyIndex = -1;
			int count = BabyInstancesDontAccessDirectly.Count;
			for (int i = 0; i < count; i++)
			{
				if (toSet == BabyInstancesDontAccessDirectly[i])
				{
					SelectedBabyIndex = i;
					break;
				}
			}
		}

		public FurbyBaby GetFurbyBabyByIndex(int index)
		{
			if (index == -1)
			{
				return null;
			}
			return BabyInstancesDontAccessDirectly[index];
		}

		public void SetThemePeriod(string name)
		{
			m_themeName = name;
			m_themeChoiceTicks = DateTime.Now.Ticks;
		}

		public void Loaded()
		{
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked && !m_unlocked)
			{
				UnlockAppLookAndFeel(AppLookAndFeel.Crystal);
			}
		}

		internal void UnlockAppLookAndFeel(AppLookAndFeel lookAndFeel)
		{
			if (!m_unlocked)
			{
				if (AppLookAndFeel == AppLookAndFeel.Normal)
				{
					CanPlayAppChangeAnimation = true;
				}
				AppLookAndFeel = lookAndFeel;
				m_unlocked = true;
			}
		}

		public uint GetSceneCount(string scene)
		{
			SceneCount sceneCount = m_sceneCounts.Find((SceneCount x) => x.scene == scene);
			return (sceneCount != null) ? sceneCount.count : 0u;
		}

		public uint IncrementSceneCount(string scene)
		{
			SceneCount sceneCount = m_sceneCounts.Find((SceneCount x) => x.scene == scene);
			if (sceneCount == null)
			{
				sceneCount = new SceneCount(scene);
				m_sceneCounts.Add(sceneCount);
			}
			sceneCount.count++;
			return sceneCount.count;
		}

		public void MarkEggHatched()
		{
			m_numFurblingsEverHatched++;
			if (this.EggHatched != null)
			{
				this.EggHatched();
			}
		}

		public bool GiftHasBeenAwardedButIsUnopened(int giftIndex)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Contains(giftIndex);
		}

		public bool GiftHasBeenAwardedAndOpened(int giftIndex)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices.Contains(giftIndex);
		}

		public bool HaveOpenedGift_ByName(string giftName)
		{
			int indexOfGiftByName = FurbyGlobals.GiftList.GetIndexOfGiftByName(giftName);
			return GiftHasBeenAwardedAndOpened(indexOfGiftByName);
		}

		public bool GiftExists(int giftIndex)
		{
			return giftIndex >= 0 && giftIndex < FurbyGlobals.GiftList.m_GiftItemData.Count;
		}

		public void MarkEligibilityForGift()
		{
			m_numGiftsForWhichEligable++;
		}

		public void MarkGiftAwarded()
		{
			m_numGiftsForWhichEligable--;
		}

		public void CountSessionWithFurby()
		{
			m_numFurbySessions++;
		}

		public void CountSessionWithoutFurby()
		{
			m_numNoFurbySessions++;
		}

		public void CountSession()
		{
			if (IsFurbyMode)
			{
				CountSessionWithFurby();
			}
			else
			{
				CountSessionWithoutFurby();
			}
		}

		public void SetReceiptAsResolved(string bundleID)
		{
			if (HaveUnresolvedReceipt(bundleID))
			{
				m_UnresolvedReceipts.Remove(bundleID);
			}
		}

		public void UpdateResolvedReceipt(string bundleID)
		{
			IAPReceipt unresolvedReceipt = GetUnresolvedReceipt(bundleID);
			unresolvedReceipt.m_LastAttempt = DateTime.Now;
			unresolvedReceipt.m_ValidationCount++;
		}

		public void RememberUnresolvedReceipt(string bundleID, string receiptBase64)
		{
			if (!HaveUnresolvedReceipt(bundleID))
			{
				if (m_UnresolvedReceipts == null)
				{
					m_UnresolvedReceipts = new Dictionary<string, IAPReceipt>();
				}
				IAPReceipt value = new IAPReceipt(receiptBase64);
				m_UnresolvedReceipts.Add(bundleID, value);
			}
		}

		public bool HaveUnresolvedReceipt(string bundleID)
		{
			if (m_UnresolvedReceipts == null)
			{
				m_UnresolvedReceipts = new Dictionary<string, IAPReceipt>();
				return false;
			}
			IAPReceipt value = null;
			return m_UnresolvedReceipts.TryGetValue(bundleID, out value);
		}

		public IAPReceipt GetUnresolvedReceipt(string bundleID)
		{
			IAPReceipt value = null;
			if (HaveUnresolvedReceipt(bundleID))
			{
				m_UnresolvedReceipts.TryGetValue(bundleID, out value);
			}
			return value;
		}

		public void IncrementSuccessfulIAPBundlePurchase(string bundleID)
		{
			if (m_BundleStats != null)
			{
				m_BundleStats = new List<IAPBundleStats>();
			}
			IAPBundleStats iAPBundleStats = m_BundleStats.Find((IAPBundleStats r) => r.m_BundleID.Equals(bundleID));
			if (iAPBundleStats == null)
			{
				iAPBundleStats = new IAPBundleStats();
				iAPBundleStats.m_BundleID = bundleID;
				iAPBundleStats.m_PurchaseCount = 1;
				m_BundleStats.Add(iAPBundleStats);
			}
			else
			{
				iAPBundleStats.m_PurchaseCount++;
			}
		}
	}
}
