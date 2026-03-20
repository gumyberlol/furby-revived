using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Furby.Scanner;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DashboardFlow : MonoBehaviour
	{
		private enum DashEvent
		{
			LevelIncreased = 0,
			BabyNeedsAttention = 1,
			EggNeedsAttention = 2,
			EggAvailable = 3,
			AdultToiletLow = 4,
			AdultHygieneLow = 5,
			AdultHungerLow = 6,
			AdultHappinessLow = 7,
			NewMinigameUnlocked = 8,
			NewVirtualFriendUnlocked = 9
		}

		public float ToiletWarnLimit = 0.25f;

		public float HygieneWarnLimit = 0.25f;

		public float HungerWarnLimit = 0.25f;

		public float HappinessWarnLimit = 0.25f;

		[SerializeField]
		private float m_timeBetweenReactions = 5f;

		[SerializeField]
		private float m_waitForVirtualFurbiesNotice = 10f;

		private HashSet<DashEvent> m_importantEvents;

		private HashSet<DashEvent> m_alreadyWarnedEvents;

		private GameEventSubscription m_playerFubyEventSub;

		private void Awake()
		{
			if (FurbyGlobals.Player.InProgressFurbyBaby != null && FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.N)
			{
				FurbyGlobals.Player.InProgressFurbyBaby.Progress = FurbyBabyProgresss.P;
			}
		}

		private IEnumerator Start()
		{
			FurbyGlobals.HardwareSettingsScreenFlow.SetIsControllingGlobalInGameVolume(false);
			if (FurbyGlobals.Player.InProgressFurbyBaby != null && FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.P && !FurbyGlobals.Player.InProgressFurbyBaby.HasBeenNamed)
			{
				while (FurbyGlobals.ScreenSwitcher.IsSwitching())
				{
					yield return null;
				}
				FurbyGlobals.ScreenSwitcher.SwitchScreen("BabyName");
			}
			yield return null;
			m_importantEvents = new HashSet<DashEvent>();
			m_alreadyWarnedEvents = new HashSet<DashEvent>();
			m_playerFubyEventSub = new GameEventSubscription(typeof(PlayerFurbyEvent), OnPlayerFurbyEvent);
			IEnumerable<int> unlockLevels = FurbyGlobals.AdultLibrary.UnlockLevels;
			int oldVirtualFurbiesUnlocked = unlockLevels.Where((int x) => FurbyGlobals.Player.Level >= x).Count();
			GameEventRouter.SendEvent(PlayerFurbyCommand.UpdateAdultLevel);
			int newVirtualFurbiesUnlocked = unlockLevels.Where((int x) => FurbyGlobals.Player.Level >= x).Count();
			if (oldVirtualFurbiesUnlocked != newVirtualFurbiesUnlocked)
			{
				Invoke("InformOfVirtualFurbies", m_waitForVirtualFurbiesNotice);
			}
			GameEventRouter.SendEvent(DashboardGameEvent.ShouldPlayInitialUpsellVideo);
			ScannerBehaviour scanner = (ScannerBehaviour)UnityEngine.Object.FindObjectOfType(typeof(ScannerBehaviour));
			while (scanner.IsBusy())
			{
				yield return null;
			}
			while (true)
			{
				ScanForEvents();
				foreach (int evt in Enum.GetValues(typeof(DashEvent)))
				{
					if (m_importantEvents.Contains((DashEvent)evt))
					{
						m_importantEvents.Remove((DashEvent)evt);
						m_alreadyWarnedEvents.Add((DashEvent)evt);
						DoReaction((DashEvent)evt);
						break;
					}
				}
				yield return new WaitForSeconds(m_timeBetweenReactions);
			}
		}

		private void DoReaction(DashEvent evt)
		{
			switch (evt)
			{
			case DashEvent.AdultHappinessLow:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultIsUnhappy);
				break;
			case DashEvent.AdultHungerLow:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultIsHungry);
				break;
			case DashEvent.AdultHygieneLow:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultIsDirty);
				break;
			case DashEvent.AdultToiletLow:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultNeedsToilet);
				break;
			case DashEvent.BabyNeedsAttention:
				GameEventRouter.SendEvent(PlayerFurbyEvent.BabyNeedsAttention);
				break;
			case DashEvent.EggAvailable:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultEggReady);
				break;
			case DashEvent.EggNeedsAttention:
				GameEventRouter.SendEvent(PlayerFurbyEvent.EggNeedsAttention);
				break;
			case DashEvent.LevelIncreased:
				GameEventRouter.SendEvent(PlayerFurbyEvent.AdultLevelGained);
				break;
			case DashEvent.NewMinigameUnlocked:
				break;
			case DashEvent.NewVirtualFriendUnlocked:
				break;
			}
		}

		private void ScanForEvents()
		{
			if (NeedsToilet())
			{
				AddToWarnSet(DashEvent.AdultToiletLow);
			}
			if (IsHungry())
			{
				AddToWarnSet(DashEvent.AdultHungerLow);
			}
			if (IsUnhappy())
			{
				AddToWarnSet(DashEvent.AdultHappinessLow);
			}
			if (IsDirty())
			{
				AddToWarnSet(DashEvent.AdultHygieneLow);
			}
			if (HasEgg())
			{
				AddToWarnSet(DashEvent.EggAvailable);
			}
		}

		public bool IsHungry()
		{
			return FurbyGlobals.Player.Satiatedness < HungerWarnLimit;
		}

		public bool IsUnhappy()
		{
			return FurbyGlobals.Player.Happiness < HappinessWarnLimit;
		}

		public bool IsDirty()
		{
			return FurbyGlobals.Player.Cleanliness < HygieneWarnLimit;
		}

		public bool NeedsToilet()
		{
			return FurbyGlobals.Player.BowelEmptiness < ToiletWarnLimit;
		}

		public bool HasEgg()
		{
			return FurbyGlobals.Player.NumEggsAvailable > 0;
		}

		private void AddToWarnSet(DashEvent evt)
		{
			if (!m_alreadyWarnedEvents.Contains(evt) && !m_importantEvents.Contains(evt))
			{
				m_importantEvents.Add(evt);
			}
		}

		private void OnDestroy()
		{
			m_playerFubyEventSub.Dispose();
		}

		private void OnPlayerFurbyEvent(Enum enumValue, GameObject originator, params object[] parameters)
		{
			PlayerFurbyEvent playerFurbyEvent = (PlayerFurbyEvent)(object)enumValue;
			PlayerFurbyEvent playerFurbyEvent2 = playerFurbyEvent;
			if (playerFurbyEvent2 == PlayerFurbyEvent.AdultLevelGained)
			{
				AddToWarnSet(DashEvent.LevelIncreased);
			}
		}

		private void InformOfVirtualFurbies()
		{
			GameEventRouter.SendEvent(DashboardGameEvent.VirtualFurbiesUnlocked);
		}
	}
}
