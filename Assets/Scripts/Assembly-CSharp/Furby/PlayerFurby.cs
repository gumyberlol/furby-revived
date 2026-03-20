using System;
using System.Collections.Generic;
using System.Linq;
using Furby.Scanner;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PlayerFurby : RelentlessMonoBehaviour
	{
		[SerializeField]
		private XpAwards m_xpAwards;

		[SerializeField]
		private FurbyAdultStatRates m_inGameStatRates;

		[SerializeField]
		private FurbyAdultStatRates m_outOfGameStatRates;

		[NonSerialized]
		private bool InNoFurbyMode;

		private bool m_hasScannedThisPlaythrough;

		private bool m_sickness;

		private GameEventSubscription m_furbyScanSubscription;

		private GameEventSubscription m_furbyPlayerCommandSubscription;

		private GameEventSubscription m_furbyXPAwardSubscription;

		private GameEventSubscription m_debugPanelSubscription;

		private GameEventSubscription m_substitutionSubscription;

		public FurbyBaby SelectedFurbyBaby
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.GetSelectedFurbyBaby();
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.SetSelectedFurbyBaby(value);
			}
		}

		public FurbyBaby InProgressFurbyBaby
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.GetInProgressFurbyBaby();
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.SetInProgressFurbyBaby(value);
				Singleton<GameDataStoreObject>.Instance.Data.SetSelectedFurbyBaby(value);
			}
		}

		public FurbyData Furby
		{
			get
			{
				AdultFurbyType adultFurbyType = Singleton<GameDataStoreObject>.Instance.Data.FurbyType;
				if (adultFurbyType == AdultFurbyType.Unknown)
				{
					adultFurbyType = AdultFurbyType.NoFurby;
				}
				return FurbyGlobals.AdultLibrary.GetAdultFurby(adultFurbyType);
			}
		}

		public FurbyData NoFurbyUnlock
		{
			get
			{
				return FurbyGlobals.AdultLibrary.GetAdultFurby(Singleton<GameDataStoreObject>.Instance.Data.NoFurbyUnlockType);
			}
		}

		public float Happiness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.Happiness;
			}
		}

		public float NewHappiness
		{
			get
			{
				if (Sickness)
				{
					return 0f;
				}
				return Singleton<GameDataStoreObject>.Instance.Data.NewHappiness;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.NewHappiness = value;
			}
		}

		public float BowelEmptiness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.BowelEmptiness;
			}
		}

		public float NewBowelEmptiness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.NewBowelEmptiness;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.NewBowelEmptiness = value;
			}
		}

		public float Cleanliness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.Cleanliness;
			}
		}

		public float NewCleanliness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.NewCleanliness;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.NewCleanliness = value;
			}
		}

		public float Satiatedness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.Satiatedness;
			}
		}

		public float NewSatiatedness
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.NewSatiatedness;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.NewSatiatedness = value;
			}
		}

		public bool Sickness
		{
			get
			{
				return m_sickness;
			}
			set
			{
				m_sickness = value;
			}
		}

		public int IncubatorFastForwards
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.IncubatorFastForwards;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.IncubatorFastForwards = value;
			}
		}

		public int IncubatorEggsHatched
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.IncubatorEggsHatched;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.IncubatorEggsHatched = value;
			}
		}

		public int NumEggsAvailable
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.NumEggsAvailable;
			}
			set
			{
				Singleton<GameDataStoreObject>.Instance.Data.NumEggsAvailable = value;
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public int XP
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.Xp;
			}
			private set
			{
				if (value != Singleton<GameDataStoreObject>.Instance.Data.Xp)
				{
					Singleton<GameDataStoreObject>.Instance.Data.Xp = value;
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
		}

		public int EarnedXP
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.EarnedXP;
			}
			set
			{
				if (value != Singleton<GameDataStoreObject>.Instance.Data.EarnedXP)
				{
					Singleton<GameDataStoreObject>.Instance.Data.EarnedXP = value;
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
		}

		public int Level
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.Level;
			}
			private set
			{
				if (value != Singleton<GameDataStoreObject>.Instance.Data.Level)
				{
					Singleton<GameDataStoreObject>.Instance.Data.Level = value;
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
		}

		public FlowStage FlowStage
		{
			get
			{
				return Singleton<GameDataStoreObject>.Instance.Data.FlowStage;
			}
			set
			{
				if (value != Singleton<GameDataStoreObject>.Instance.Data.FlowStage)
				{
					Singleton<GameDataStoreObject>.Instance.Data.FlowStage = value;
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
		}

		public string FullName
		{
			get
			{
				if (NoFurbyOnSaveGame())
				{
					return string.Format(Singleton<Localisation>.Instance.GetText("DASHBOARD_NOFURBY_NAME"), Singleton<GameDataStoreObject>.Instance.GetCurrentSlotIndex() + 1);
				}
				return string.Format("{0}-{1}", Singleton<GameDataStoreObject>.Instance.Data.FurbyNameLeft, Singleton<GameDataStoreObject>.Instance.Data.FurbyNameRight);
			}
		}

		public void SetScannedFlag(bool scanned)
		{
			m_hasScannedThisPlaythrough = scanned;
		}

		public bool HasScannedThisPlaythrough()
		{
			return m_hasScannedThisPlaythrough;
		}

		public bool NoFurbyForEitherReason()
		{
			return NoFurbyOnSaveGame() || HasFurbyButNotAtTheMoment();
		}

		public bool NoFurbyOnSaveGame()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode;
		}

		public bool HasFurbyOnSaveGame()
		{
			return !NoFurbyOnSaveGame();
		}

		public bool HasFurbyButNotAtTheMoment()
		{
			return InNoFurbyMode && !Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode;
		}

		public bool IsFirstTimeFlow()
		{
			return FlowStage != FlowStage.Normal;
		}

		public bool IsFurbyUnlocked(AdultFurbyType furbyType)
		{
			if (Singleton<GameDataStoreObject>.Instance.Data.MetFurbies.Contains(furbyType))
			{
				return true;
			}
			IEnumerable<AdultFurbyType> unlocksInOrder = FurbyGlobals.Player.Furby.UnlocksInOrder;
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				unlocksInOrder = FurbyGlobals.Player.NoFurbyUnlock.UnlocksInOrder;
			}
			int num = Array.IndexOf(unlocksInOrder.ToArray(), furbyType);
			IList<int> unlockLevels = FurbyGlobals.AdultLibrary.UnlockLevels;
			if (num == -1)
			{
				return false;
			}
			if (num >= unlockLevels.Count)
			{
				return false;
			}
			return unlockLevels[num] <= Level;
		}

		private void UpdateXpAndLevel()
		{
			if (NumEggsAvailable <= 0 && EarnedXP != 0)
			{
				if (FurbyGlobals.AdultLibrary.XpLevels.Count > Level + 1)
				{
					int num = FurbyGlobals.AdultLibrary.XpLevels[Level + 1];
					int a = num - XP;
					int num2 = Mathf.Min(a, EarnedXP);
					XP += num2;
					GameEventRouter.SendEvent(PlayerFurbyEvent.AdultGainedXP, base.gameObject, num2);
					EarnedXP -= num2;
				}
				else
				{
					XP += EarnedXP;
					GameEventRouter.SendEvent(PlayerFurbyEvent.AdultGainedXP, base.gameObject, EarnedXP);
					EarnedXP = 0;
				}
			}
			int levelForXP = GetLevelForXP(XP);
			if (Level != levelForXP)
			{
				if (Furby.AdultType != AdultFurbyType.NoFurby || Level == 0)
				{
					NumEggsAvailable = 1;
				}
				Level++;
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultLevelGained, base.gameObject, EarnedXP);
			}
		}

		private int GetLevelForXP(int xp)
		{
			int num = -1;
			foreach (int xpLevel in FurbyGlobals.AdultLibrary.XpLevels)
			{
				if (xp < xpLevel)
				{
					break;
				}
				num++;
			}
			return num;
		}

		private void Awake()
		{
			m_furbyScanSubscription = new GameEventSubscription(typeof(ScannerEvents), OnScanEvent);
			m_furbyPlayerCommandSubscription = new GameEventSubscription(typeof(PlayerFurbyCommand), OnPlayerFurbyCommand);
			m_furbyXPAwardSubscription = new GameEventSubscription(typeof(XpAwardEvent), OnXpAwardEvent);
			m_debugPanelSubscription = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
			m_substitutionSubscription = new GameEventSubscription(OnStringSubstitutionRequest, Localisation.LocalisationEvent.RequestSubstitution);
		}

		private void OnDestroy()
		{
			m_furbyScanSubscription.Dispose();
			m_furbyPlayerCommandSubscription.Dispose();
			m_furbyXPAwardSubscription.Dispose();
			m_debugPanelSubscription.Dispose();
			m_substitutionSubscription.Dispose();
		}

		public bool IsPlayersFurby(FurbyStatus status)
		{
			bool result = false;
			string value = Singleton<GameDataStoreObject>.Instance.Data.FurbyNameLeft + "_" + Singleton<GameDataStoreObject>.Instance.Data.FurbyNameRight;
			FurbyReceiveName furbyReceiveName = EnumExtensions.Parse<FurbyReceiveName>(value);
			if (status.Name == furbyReceiveName && Singleton<GameDataStoreObject>.Instance.Data.FurbyType == AdultFurbyLibrary.ConvertComAirPatternToAdultType(status.Pattern))
			{
				result = true;
			}
			return result;
		}

		private void ReceiveFurbyDataEvent(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			FurbyDataEvent furbyDataEvent = (FurbyDataEvent)(object)eventType;
			FurbyDataEvent furbyDataEvent2 = furbyDataEvent;
			if (furbyDataEvent2 != FurbyDataEvent.FurbyDataReceived)
			{
				return;
			}
			FurbyStatus status = (FurbyStatus)parameters[0];
			if (IsPlayersFurby(status))
			{
				NewHappiness = (float)status.Happyness / 100f;
				if (!m_hasScannedThisPlaythrough)
				{
					NewSatiatedness = (float)status.Fullness / 100f;
				}
				gameObject.SendGameEvent(PlayerFurbyEvent.StatusUpdated, this);
			}
		}

		private void OnScanEvent(Enum sysEvent, GameObject origin, params object[] paramList)
		{
			ScannerEvents scannerEvents = (ScannerEvents)(object)sysEvent;
			if (scannerEvents == ScannerEvents.ScanningSucceeded)
			{
				GameEventRouter.SendEvent(PlayerFurbyEvent.FurbyModeActivated);
			}
		}

		private void OnPlayerFurbyCommand(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			switch ((PlayerFurbyCommand)(object)eventType)
			{
			case PlayerFurbyCommand.TurnOffNoFurbyMode:
				InNoFurbyMode = false;
				GameEventRouter.SendEvent(PlayerFurbyEvent.FurbyModeActivated);
				break;
			case PlayerFurbyCommand.TurnOnNoFurbyMode:
				InNoFurbyMode = true;
				GameEventRouter.SendEvent(PlayerFurbyEvent.NoFurbyModeActivated);
				break;
			case PlayerFurbyCommand.AwardAdultXP:
				EarnedXP += (int)parameters[0];
				break;
			case PlayerFurbyCommand.UpdateAdultLevel:
				UpdateXpAndLevel();
				break;
			}
		}

		public void OnXpAwardEvent(Enum enumValue, GameObject gObj, params object[] parameters)
		{
			XpAwardEvent xpEvent = (XpAwardEvent)(object)enumValue;
			using (IEnumerator<XpAwardValue> enumerator = m_xpAwards.XpAwardValues.Where((XpAwardValue x) => xpEvent == x.XpEvent).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					XpAwardValue current = enumerator.Current;
					XP += current.Xp;
				}
			}
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Adult XP / Levelling"))
			{
				if (GUILayout.Button("Max out the Level"))
				{
					Level = FurbyGlobals.AdultLibrary.XpLevels.Count;
					EarnedXP = FurbyGlobals.AdultLibrary.XpLevels[Level];
					UpdateXpAndLevel();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("XP : " + XP);
				if (GUILayout.Button("-100"))
				{
					XP -= 100;
				}
				if (GUILayout.Button("+100"))
				{
					XP += 100;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("EarnedXP : " + EarnedXP);
				if (GUILayout.Button("-100"))
				{
					EarnedXP -= 100;
				}
				if (GUILayout.Button("+100"))
				{
					EarnedXP += 100;
				}
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Update Earned XP to Level"))
				{
					UpdateXpAndLevel();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Level : " + Level);
				if (Level + 1 < FurbyGlobals.AdultLibrary.XpLevels.Count)
				{
					GUILayout.Label("Next Level : " + FurbyGlobals.AdultLibrary.XpLevels[Level + 1]);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Prev Level") && Level > 0)
				{
					EarnedXP = FurbyGlobals.AdultLibrary.XpLevels[Level - 1] - XP;
					UpdateXpAndLevel();
				}
				if (GUILayout.Button("Next Level") && Level + 1 < FurbyGlobals.AdultLibrary.XpLevels.Count)
				{
					EarnedXP = FurbyGlobals.AdultLibrary.XpLevels[Level + 1] - XP;
					UpdateXpAndLevel();
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Number of eggs available:");
				GUILayout.Label(NumEggsAvailable.ToString(), GUI.skin.textField);
				GUI.enabled = NumEggsAvailable > 0;
				if (GUILayout.Button("[  -  ]"))
				{
					NumEggsAvailable = 0;
				}
				GUI.enabled = NumEggsAvailable <= 0;
				if (GUILayout.Button("[  +  ]"))
				{
					NumEggsAvailable = 1;
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Happiness");
				NewHappiness = GUILayout.HorizontalSlider(NewHappiness, 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Bowel Emptiness");
				NewBowelEmptiness = GUILayout.HorizontalSlider(NewBowelEmptiness, 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Cleanliness");
				NewCleanliness = GUILayout.HorizontalSlider(NewCleanliness, 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Satiatedness");
				NewSatiatedness = GUILayout.HorizontalSlider(NewSatiatedness, 0f, 1f);
				GUILayout.EndHorizontal();
				Sickness = GUILayout.Toggle(Sickness, "Sickness");
			}
			DebugPanel.EndSection();
			if (DebugPanel.StartSection("First Time Flow"))
			{
				if (GUILayout.Button("Start First-Time Flow"))
				{
					FurbyGlobals.Player.FlowStage = FlowStage.Dashboard_Initial;
				}
				if (GUILayout.Button("Turn Off First-Time Flow"))
				{
					FurbyGlobals.Player.FlowStage = FlowStage.Normal;
				}
				GUILayout.BeginVertical();
				GUILayout.Label("First Time Flow Section: ");
				FlowStage[] array = (FlowStage[])Enum.GetValues(typeof(FlowStage));
				int num = Array.IndexOf(array, FurbyGlobals.Player.FlowStage);
				int num2 = num;
				GUILayout.BeginHorizontal();
				GUI.enabled = num > 0;
				if (GUILayout.Button("[  <  ]"))
				{
					num2--;
				}
				GUI.enabled = num < array.Length - 1;
				if (GUILayout.Button("[   >  ]"))
				{
					num2++;
				}
				GUI.enabled = true;
				GUILayout.Label(FurbyGlobals.Player.FlowStage.ToString(), GUI.skin.textField);
				GUILayout.EndHorizontal();
				if (num2 != num)
				{
					FurbyGlobals.Player.FlowStage = array[num2];
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}

		private float CalculateUpdatedStat(float oldStat, float newStat, float reductionRate)
		{
			float result = oldStat;
			if (oldStat != newStat)
			{
				result = newStat;
			}
			else if (Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate != 0L)
			{
				double totalSeconds = (DateTime.Now - new DateTime(Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate)).TotalSeconds;
				float num = Mathf.Clamp01((float)(totalSeconds / (double)reductionRate));
				result = Mathf.Clamp01(oldStat - num);
			}
			return result;
		}

		public void UpdateStats()
		{
			DateTime dateTime = new DateTime(Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate);
			if (Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate != 0L)
			{
				FurbyAdultStatRates furbyAdultStatRates = ((!(dateTime < DateTime.Now.AddSeconds(0f - Time.realtimeSinceStartup))) ? m_inGameStatRates : m_outOfGameStatRates);
				Singleton<GameDataStoreObject>.Instance.Data.Satiatedness = CalculateUpdatedStat(Singleton<GameDataStoreObject>.Instance.Data.Satiatedness, Singleton<GameDataStoreObject>.Instance.Data.NewSatiatedness, furbyAdultStatRates.SatiatednessSecondsToZero);
				NewSatiatedness = Singleton<GameDataStoreObject>.Instance.Data.Satiatedness;
				Singleton<GameDataStoreObject>.Instance.Data.Cleanliness = CalculateUpdatedStat(Singleton<GameDataStoreObject>.Instance.Data.Cleanliness, Singleton<GameDataStoreObject>.Instance.Data.NewCleanliness, furbyAdultStatRates.CleanlinessSecondsToZero);
				NewCleanliness = Singleton<GameDataStoreObject>.Instance.Data.Cleanliness;
				Singleton<GameDataStoreObject>.Instance.Data.BowelEmptiness = CalculateUpdatedStat(Singleton<GameDataStoreObject>.Instance.Data.BowelEmptiness, Singleton<GameDataStoreObject>.Instance.Data.NewBowelEmptiness, furbyAdultStatRates.BowelEmptinessSecondsToZero);
				NewBowelEmptiness = Singleton<GameDataStoreObject>.Instance.Data.BowelEmptiness;
				if (Sickness)
				{
					Singleton<GameDataStoreObject>.Instance.Data.Happiness = CalculateUpdatedStat(Singleton<GameDataStoreObject>.Instance.Data.Happiness, 0f, furbyAdultStatRates.HappinessSecondsToZero);
				}
				else
				{
					Singleton<GameDataStoreObject>.Instance.Data.Happiness = CalculateUpdatedStat(Singleton<GameDataStoreObject>.Instance.Data.Happiness, Singleton<GameDataStoreObject>.Instance.Data.NewHappiness, furbyAdultStatRates.HappinessSecondsToZero);
				}
				NewHappiness = Singleton<GameDataStoreObject>.Instance.Data.Happiness;
			}
			Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate = DateTime.Now.Ticks;
		}

		private void OnStringSubstitutionRequest(Enum evt, GameObject obj, params object[] parameters)
		{
			if (evt.Equals(Localisation.LocalisationEvent.RequestSubstitution))
			{
				string text = parameters[0] as string;
				switch (text)
				{
				case "{adultname}":
					GameEventRouter.SendEvent(Localisation.LocalisationEvent.SetSubstitution, base.gameObject, text, FullName);
					break;
				case "{inprogressbabyname}":
					GameEventRouter.SendEvent(Localisation.LocalisationEvent.SetSubstitution, base.gameObject, text, (InProgressFurbyBaby != null) ? InProgressFurbyBaby.Name : string.Empty);
					break;
				case "{selectedbabyname}":
					GameEventRouter.SendEvent(Localisation.LocalisationEvent.SetSubstitution, base.gameObject, text, (SelectedFurbyBaby != null) ? SelectedFurbyBaby.Name : string.Empty);
					break;
				case "{saveslot}":
					GameEventRouter.SendEvent(Localisation.LocalisationEvent.SetSubstitution, base.gameObject, text, Singleton<GameDataStoreObject>.Instance.GetCurrentSlotIndex().ToString());
					break;
				case "{adultlevel}":
					GameEventRouter.SendEvent(Localisation.LocalisationEvent.SetSubstitution, base.gameObject, text, Level.ToString());
					break;
				}
			}
		}
	}
}
