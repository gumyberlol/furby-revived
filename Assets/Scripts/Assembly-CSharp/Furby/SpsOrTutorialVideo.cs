using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SpsOrTutorialVideo : RelentlessMonoBehaviour
	{
		public bool m_GoBack;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference m_TargetScreen;

		public bool m_AutoSwitch;

		public float m_DelayTimeSecs;

		public bool m_IncludeInHistory = true;

		public TutorialVideoEvents m_VideoEvent;

		public bool m_SuppressScreenSwitch;

		public bool m_DontSwitchScreensInNoFurbyMode;

		[SerializeField]
		private bool m_forceNoFurbyMode;

		private static bool s_ScreenSwitchIsInProgress;

		public bool m_CheckGlobalSave;

		public GameObject m_IAPDialogToNavigatePriorToSwitch;

		private void OnEnable()
		{
			s_ScreenSwitchIsInProgress = false;
			if (m_AutoSwitch)
			{
				Invoke(ManuallyInvoke, m_DelayTimeSecs);
			}
		}

		private void OnDisable()
		{
			if (m_AutoSwitch)
			{
				CancelInvoke(ManuallyInvoke);
			}
			GameEventRouter.RemoveDelegateForType(typeof(VideoPlayerGameEvents), OnVideoPlayerGameEventReceived);
		}

		private bool InNoFurbyMode()
		{
			return FurbyGlobals.Player.NoFurbyOnSaveGame() || m_forceNoFurbyMode;
		}

		private string GetVideoFilename()
		{
			return FurbyGlobals.VideoFilenameLookup.GetVideoName(m_VideoEvent);
		}

		private bool HavePlayedVideoAlready_PerSaveSlot()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(GetVideoFilename());
		}

		private bool HavePlayedVideoAlready_Global()
		{
			return Singleton<GameDataStoreObject>.Instance.GlobalData.m_videosPlayed.Contains(GetVideoFilename());
		}

		private void MarkVideoAsPlayed_PerSaveSlot()
		{
			if (!Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(GetVideoFilename()))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Add(GetVideoFilename());
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private void MarkVideoAsPlayed_Global()
		{
			if (!Singleton<GameDataStoreObject>.Instance.GlobalData.m_videosPlayed.Contains(GetVideoFilename()))
			{
				Singleton<GameDataStoreObject>.Instance.GlobalData.m_videosPlayed.Add(GetVideoFilename());
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private void PlayTheVideo()
		{
			GameEventRouter.SendEvent(m_VideoEvent);
		}

		private void OnClick()
		{
			if (!SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled && !s_ScreenSwitchIsInProgress)
			{
				if (!m_SuppressScreenSwitch && !m_DontSwitchScreensInNoFurbyMode)
				{
					SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled = true;
					s_ScreenSwitchIsInProgress = true;
				}
				if (m_CheckGlobalSave)
				{
					GameEventRouter.Instance.StartCoroutine(DriveVideoLogicForGlobalSaves());
				}
				else
				{
					GameEventRouter.Instance.StartCoroutine(DriveVideoLogicForSaveSlots());
				}
			}
		}

		public void ManuallyInvoke()
		{
			OnClick();
		}

		private IEnumerator DriveVideoLogicForGlobalSaves()
		{
			if (m_IAPDialogToNavigatePriorToSwitch != null && !Singleton<GameDataStoreObject>.Instance.GlobalData.InAppWarningDialogHasBeenShown)
			{
				GameEventRouter.SendEvent(IAPWarningEvent.IAP_Warning_Shown, null);
				m_IAPDialogToNavigatePriorToSwitch.SetActive(true);
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForAnyEventOfType(typeof(SharedGuiEvents)));
				m_IAPDialogToNavigatePriorToSwitch.SetActive(false);
				GameEventRouter.SendEvent(IAPWarningEvent.IAP_Warning_Accepted, null);
				Singleton<GameDataStoreObject>.Instance.GlobalData.InAppWarningDialogHasBeenShown = true;
				Singleton<GameDataStoreObject>.Instance.Save();
			}
			if (HavePlayedVideoAlready_Global())
			{
				SwitchScreens();
			}
			else
			{
				PlayTheVideo();
				MarkVideoAsPlayed_Global();
				SwitchScreensOnVideoPlayed();
			}
			yield return null;
		}

		private IEnumerator DriveVideoLogicForSaveSlots()
		{
			if (InNoFurbyMode())
			{
				if (FurbyGlobals.VideoSettings.m_showVideos)
				{
					PlayTheVideo();
					MarkVideoAsPlayed_PerSaveSlot();
					if (!m_SuppressScreenSwitch && !m_DontSwitchScreensInNoFurbyMode)
					{
						SwitchScreensOnVideoPlayed();
					}
				}
				else if (!m_SuppressScreenSwitch && !m_DontSwitchScreensInNoFurbyMode)
				{
					SwitchScreens();
				}
			}
			else if (HavePlayedVideoAlready_PerSaveSlot())
			{
				if (!m_SuppressScreenSwitch)
				{
					SwitchScreens();
				}
			}
			else if (FurbyGlobals.VideoSettings.m_showVideos)
			{
				PlayTheVideo();
				MarkVideoAsPlayed_PerSaveSlot();
				if (!m_SuppressScreenSwitch)
				{
					SwitchScreensOnVideoPlayed();
				}
			}
			else if (!m_SuppressScreenSwitch)
			{
				SwitchScreens();
			}
			yield return null;
		}

		private void SwitchScreensOnVideoPlayed()
		{
			GameEventRouter.RemoveDelegateForType(typeof(VideoPlayerGameEvents), OnVideoPlayerGameEventReceived);
			GameEventRouter.AddDelegateForType(typeof(VideoPlayerGameEvents), OnVideoPlayerGameEventReceived);
		}

		private void OnVideoPlayerGameEventReceived(Enum enumValue, GameObject gameObject, params object[] parameters)
		{
			switch ((VideoPlayerGameEvents)(object)enumValue)
			{
			case VideoPlayerGameEvents.VideoHasStarted:
				break;
			case VideoPlayerGameEvents.VideoHasFinished:
				SwitchScreens();
				GameEventRouter.RemoveDelegateForType(typeof(VideoPlayerGameEvents), OnVideoPlayerGameEventReceived);
				break;
			case VideoPlayerGameEvents.RequestVideoStop:
				break;
			}
		}

		private void SwitchScreens()
		{
			if (base.enabled)
			{
				if (m_GoBack)
				{
					FurbyGlobals.ScreenSwitcher.BackScreen();
				}
				else
				{
					FurbyGlobals.ScreenSwitcher.SwitchScreen(m_TargetScreen, m_IncludeInHistory);
				}
				ReleaseLockOnSuccessfulSwitch();
				SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled = false;
			}
		}

		private void ReleaseLockOnSuccessfulSwitch()
		{
			GameEventRouter.RemoveDelegateForType(typeof(SpsEvent), OnSwitchResult);
			GameEventRouter.AddDelegateForType(typeof(SpsEvent), OnSwitchResult);
		}

		private void OnSwitchResult(Enum enumValue, GameObject gameObject, params object[] parameters)
		{
			SpsEvent spsEvent = (SpsEvent)(object)enumValue;
			SpsEvent spsEvent2 = spsEvent;
			if (spsEvent2 == SpsEvent.EnterScreen)
			{
				s_ScreenSwitchIsInProgress = false;
			}
		}
	}
}
