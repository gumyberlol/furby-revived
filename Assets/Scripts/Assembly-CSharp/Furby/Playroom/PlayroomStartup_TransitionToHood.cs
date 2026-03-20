using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomStartup_TransitionToHood : RelentlessMonoBehaviour
	{
		private bool m_hasFired;

		private void OnLevelWasLoaded()
		{
			if (!m_hasFired && !Application.loadedLevelName.ToLower().Contains("empty"))
			{
				m_hasFired = true;
				InvokeTransition(0f);
			}
		}

		public void InvokeTransition(float initialDelaySecs)
		{
			Logging.Log("InvokeTransition");
			SetPlayroomInNonInteractiveState();
			ScheduleSequenceOfEvents(initialDelaySecs);
		}

		private void SetPlayroomInNonInteractiveState()
		{
			Logging.Log("SetPlayroomInNonInteractiveState()");
			GameObject gameObject = GameObject.Find("PlayroomState");
			PlayroomState component = gameObject.GetComponent<PlayroomState>();
			component.CurrentState = PlayroomStateEnum.LevellingUp;
			GameObject gameObject2 = GameObject.Find("[8] UI");
			gameObject2.SetActive(false);
			GameObject gameObject3 = GameObject.Find("FurbyInteraction");
			PlayroomInteractionMediator component2 = gameObject3.GetComponent<PlayroomInteractionMediator>();
			component2.m_HaveHandledLowStatus = true;
		}

		private void ScheduleSequenceOfEvents(float initialDelaySecs)
		{
			StartCoroutine(SequenceCoroutine(initialDelaySecs));
		}

		private IEnumerator SequenceCoroutine(float initialDelaySecs)
		{
			yield return new WaitForSeconds(initialDelaySecs);
			yield return StartCoroutine(ExecuteSequenceOfEvents());
			SetNeighbourhoodToShowRevealSequence();
		}

		private IEnumerator ExecuteSequenceOfEvents()
		{
			AssignNeighbourhoodRoom();
			GameEventRouter.SendEvent(PlayroomGameEvent.Progression_Started);
			yield return new WaitForSeconds(1.26f);
			GameEventRouter.SendEvent(PlayroomGameEvent.Progression_FXandCelebrate);
			yield return new WaitForSeconds(4f);
			GameEventRouter.SendEvent(PlayroomGameEvent.Progression_Complete);
			TransitionToNeighbourhoodScene();
		}

		private void AssignNeighbourhoodRoom()
		{
			switch (FurbyGlobals.Player.InProgressFurbyBaby.Tribe.TribeSet)
			{
			case Tribeset.Promo:
				FurbyGlobals.BabyRepositoryHelpers.AssignRoomIndexPromo(FurbyGlobals.Player.InProgressFurbyBaby);
				break;
			case Tribeset.MainTribes:
			case Tribeset.Spring:
			case Tribeset.CrystalGem:
				FurbyGlobals.BabyRepositoryHelpers.AssignRoomIndex(FurbyGlobals.Player.InProgressFurbyBaby);
				break;
			case Tribeset.Golden:
				FurbyGlobals.BabyRepositoryHelpers.AssignRoomIndexPromo(FurbyGlobals.Player.InProgressFurbyBaby);
				break;
			}
			FurbyGlobals.BabyRepositoryHelpers.ProgressToNeighbourhood(FurbyGlobals.Player.InProgressFurbyBaby);
			BroadcastNeighbourhoodEvents();
		}

		private void TransitionToNeighbourhoodScene()
		{
			SpsSwitchAction spsSwitchAction = base.transform.gameObject.AddComponent<SpsSwitchAction>();
			spsSwitchAction.TargetScreen = new LevelReference();
			switch (FurbyGlobals.Player.InProgressFurbyBaby.Tribe.TribeSet)
			{
			case Tribeset.MainTribes:
			case Tribeset.Golden:
				spsSwitchAction.TargetScreen.Path = "NeighbourhoodScene";
				break;
			case Tribeset.Spring:
				spsSwitchAction.TargetScreen.Path = "NeighbourhoodScene_Spring";
				break;
			case Tribeset.Promo:
				spsSwitchAction.TargetScreen.Path = "NeighbourhoodScene_Promo";
				break;
			case Tribeset.CrystalGem:
			case Tribeset.CrystalGolden:
				spsSwitchAction.TargetScreen.Path = "NeighbourhoodScene_Crystal";
				break;
			}
			spsSwitchAction.ManuallyInvoke();
		}

		private void SetNeighbourhoodToShowRevealSequence()
		{
			GameObject gameObject = new GameObject("SENTINEL_ShowRevealSequence");
			HoodStartup_FromEoG hoodStartup_FromEoG = gameObject.AddComponent<HoodStartup_FromEoG>();
			hoodStartup_FromEoG.m_TargetBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			Object.DontDestroyOnLoad(gameObject);
		}

		private void BroadcastNeighbourhoodEvents()
		{
			switch (FurbyGlobals.Player.InProgressFurbyBaby.Tribe.TribeSet)
			{
			case Tribeset.MainTribes:
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 0)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Main_Level1);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 1)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Main_Level2);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 2)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Main_Level3);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 3)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Main_Level4);
				}
				break;
			case Tribeset.Spring:
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 0)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level1);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 1)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level2);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 2)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level3);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 3)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level4);
				}
				break;
			case Tribeset.Promo:
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 0)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Promo_Level1);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 1)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Promo_Level2);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 2)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Promo_Level3);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 3)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Promo_Level4);
				}
				break;
			case Tribeset.Golden:
				GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Golden);
				break;
			case Tribeset.CrystalGem:
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 0)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level1);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 1)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level2);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 2)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level3);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby.Level == 3)
				{
					GameEventRouter.SendEvent(BabyJoinsNeighbourhoodEvents.BabyJoinsNeighbourhood_Spring_Level4);
				}
				break;
			}
		}
	}
}
