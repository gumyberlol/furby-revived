using System;
using System.Collections;
using System.Collections.Generic;
using Furby.Utilities;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomInteractionMediator : RelentlessMonoBehaviour
	{
		private GameObject m_CachedTargetModel;

		public GameObject m_TargetModelInstance;

		public bool m_AllowAnimationsToBeInterrupted;

		public bool m_AllowGenericIdlesAnimationsToBeInterrupted;

		private AnimationClip m_LastInvokedIdleAnimationClip;

		public float m_InteractionGracePeriod;

		private bool m_UnderGracePeriod;

		private float m_TimeWhenGracePeriodExpires;

		public float m_timeUntilInteractionVideoPlayed = 10f;

		public LowStatusIncident m_NeglectIncident;

		public LowStatusIncident m_HungryIncident;

		public LowStatusIncident m_DirtyIncident;

		public GameObject m_LightObjectToActivate;

		public LowStatusThresholds m_LowStatusThresholds;

		public float m_TimeOfLastInput;

		public PlayroomInactivity m_Inactivity = new PlayroomInactivity();

		private bool m_InInactivitySequence;

		private FurbyComAirWaiter m_ComAirWaiter;

		private bool m_QuietMode;

		private float m_playInteractionVideoCounter;

		public GameObject[] m_popUps;

		public bool m_FirstTimeOnSceneStart = true;

		public bool m_HaveHandledLowStatus;

		private bool m_ReattachAttractMode;

		private bool m_AmCustomizing;

		public static int s_SessionVisitCount;

		public void SetInQuietMode()
		{
			m_QuietMode = true;
		}

		private bool ShouldPlayVideoForThisPlayroomVisit()
		{
			return s_SessionVisitCount % 5 == 0;
		}

		private void Awake()
		{
			m_ComAirWaiter = new FurbyComAirWaiter();
			m_TimeOfLastInput = Time.time;
			m_FirstTimeOnSceneStart = true;
			GameObject gameObject = GameObject.Find("PlayroomState");
			PlayroomState component = gameObject.GetComponent<PlayroomState>();
			if (FurbyGlobals.Player.FlowStage == FlowStage.Normal && component.CurrentState != PlayroomStateEnum.LevellingUp && FurbyGlobals.Player.NoFurbyOnSaveGame() && FurbyGlobals.VideoSettings.m_showVideos)
			{
				if (ShouldPlayVideoForThisPlayroomVisit())
				{
					StartCoroutine(StartVideoOnceAssetsAreLoaded());
				}
				s_SessionVisitCount++;
			}
		}

		private IEnumerator StartVideoOnceAssetsAreLoaded()
		{
			yield return StartCoroutine(SendEventOnceAssetsAreLoaded());
		}

		private IEnumerator SendEventOnceAssetsAreLoaded()
		{
			while (!AssetBundleHelpers.IsLoading())
			{
				yield return null;
			}
			while (AssetBundleHelpers.IsLoading())
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.1f);
			GameObject playroomStateObject = GameObject.Find("PlayroomState");
			PlayroomState playroomState = playroomStateObject.GetComponent<PlayroomState>();
			if (FurbyGlobals.Player.FlowStage == FlowStage.Normal && playroomState.CurrentState != PlayroomStateEnum.LevellingUp)
			{
				GameEventRouter.SendEvent(TutorialVideoEvents.PlayroomInteraction);
				string videoFilename = FurbyGlobals.VideoFilenameLookup.GetVideoName(TutorialVideoEvents.PlayroomInteraction);
				if (!Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(videoFilename))
				{
					Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Add(videoFilename);
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
		}

		public void Initialise()
		{
			CacheTargetModel();
			m_TimeOfLastInput = Time.time;
			m_QuietMode = false;
			GameEventRouter.AddDelegateForEnums(InterceptPlayroomEvents, PlayroomGameEvent.Customization_SequenceBegun, PlayroomGameEvent.Customization_SequenceEnded);
			SetupAttractMode();
		}

		private void SetupAttractMode()
		{
			StopCoroutine("AttractMode_HoldingFunction");
			StartCoroutine("AttractMode_HoldingFunction");
		}

		private void InterceptPlayroomEvents(Enum enumValue, GameObject gameObject, params object[] parameters)
		{
			PlayroomGameEvent playroomGameEvent = (PlayroomGameEvent)(object)enumValue;
			if (playroomGameEvent == PlayroomGameEvent.Customization_SequenceBegun)
			{
				InterruptSadSinging();
				m_AmCustomizing = true;
			}
			if (playroomGameEvent == PlayroomGameEvent.Customization_SequenceEnded)
			{
				m_AmCustomizing = false;
			}
		}

		private bool HaveCachedTargetModel()
		{
			return m_CachedTargetModel != null;
		}

		private void CacheTargetModel()
		{
			if (!m_TargetModelInstance)
			{
				return;
			}
			ModelInstance component = m_TargetModelInstance.GetComponent<ModelInstance>();
			if (component != null)
			{
				m_CachedTargetModel = component.Instance;
				if ((bool)m_CachedTargetModel && (bool)m_CachedTargetModel.animation)
				{
					m_CachedTargetModel.animation.Stop();
					InitialiseIdlingSystem();
					GameEventRouter.SendEvent(PlayroomGameEvent.EnterPlayroom);
				}
			}
		}

		private void InitialiseIdlingSystem()
		{
			Singleton<PlayroomPersonalityIdling>.Instance.Initialise();
			if (Singleton<PlayroomIdlingController>.Instance.Enable)
			{
				SelectAndInvokeAnIdleAnimation();
			}
		}

		private void OnDestroy()
		{
			if (m_InInactivitySequence)
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_SequenceInterrupted);
				m_ComAirWaiter.Interrupt();
			}
			StopCoroutine("AttractMode_HoldingFunction");
			GameEventRouter.SendEvent(PlayroomGameEvent.ExitPlayroom);
		}

		private void OnGUI()
		{
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
			{
				m_TimeOfLastInput = Time.time;
			}
		}

		public void HandleCollision(GameObject targetObject, ActionType actionType, float userValue)
		{
			if (!ShouldAllowNewEvents() || !targetObject)
			{
				return;
			}
			PlayroomGameData component = GetComponent<PlayroomGameData>();
			if (!(component != null))
			{
				return;
			}
			InteractionIncident interactionFromColliderAndAction = component.GetInteractionFromColliderAndAction(targetObject.collider, actionType);
			if (interactionFromColliderAndAction == null)
			{
				return;
			}
			bool flag = true;
			if (interactionFromColliderAndAction.m_ActionTypeRequired == ActionType.Tickle)
			{
				flag = userValue > interactionFromColliderAndAction.m_DistanceRequired;
				if (flag)
				{
					InteractionCollisionHandler interactionCollisionHandler = (InteractionCollisionHandler)GetComponent("InteractionCollisionHandler");
					foreach (ThresholdTrigger threshold in interactionCollisionHandler.m_Thresholds)
					{
						if (threshold.m_Handled)
						{
							flag = false;
						}
						else if (threshold.m_Collider == targetObject.collider)
						{
							threshold.m_Handled = true;
						}
					}
				}
			}
			if (flag)
			{
				GameEventRouter.SendEvent(interactionFromColliderAndAction.m_ReactionEvent, null, null);
				m_UnderGracePeriod = true;
				m_TimeWhenGracePeriodExpires = Time.time + m_InteractionGracePeriod;
			}
		}

		private bool ShouldAllowNewEvents()
		{
			bool flag = false;
			if (m_AllowAnimationsToBeInterrupted)
			{
				return true;
			}
			if (m_AllowGenericIdlesAnimationsToBeInterrupted)
			{
				return IsPlayingAnIdleAnimation();
			}
			return IsPlayingAGenericIdleAnimation();
		}

		private void Update()
		{
			if (m_ReattachAttractMode)
			{
				SetupAttractMode();
				m_ReattachAttractMode = false;
			}
			if (!(m_playInteractionVideoCounter < m_timeUntilInteractionVideoPlayed))
			{
				return;
			}
			bool flag = true;
			GameObject[] popUps = m_popUps;
			foreach (GameObject gameObject in popUps)
			{
				if (gameObject.activeInHierarchy)
				{
					flag = false;
				}
			}
			if (flag)
			{
				m_playInteractionVideoCounter += Time.deltaTime;
				if (m_playInteractionVideoCounter >= m_timeUntilInteractionVideoPlayed)
				{
					GameEventRouter.SendEvent(PlayroomGameEvent.PlayInteractionVideo);
				}
			}
		}

		public void LateUpdate()
		{
			if (!HaveCachedTargetModel())
			{
				CacheTargetModel();
			}
			if (m_QuietMode || !HaveCachedTargetModel())
			{
				return;
			}
			CheckForAndHandleLowStatus();
			if ((bool)Singleton<PlayroomIdlingController>.Instance && Singleton<PlayroomIdlingController>.Instance.Enable && !m_InInactivitySequence)
			{
				CheckAndHandleInactivity();
				if (!IsAnyAnimationPlaying())
				{
					DriveIdlingLogic();
				}
			}
			Singleton<LookAtTouch_Enabler>.Instance.Enable = IsPlayingAnIdleAnimation();
		}

		public void CheckAndHandleInactivity()
		{
			if (IsPlayingAGenericIdleAnimation() && !m_AmCustomizing)
			{
				float num = Time.time - m_TimeOfLastInput;
				if (num >= m_Inactivity.m_InactivityPeriod)
				{
					m_InInactivitySequence = true;
				}
			}
		}

		private IEnumerator AttractMode_HoldingFunction()
		{
			m_InInactivitySequence = false;
			while (!m_InInactivitySequence)
			{
				yield return null;
			}
			m_TimeOfLastInput = Time.time;
			yield return this.HeartBeatAndWaitOnSend();
			GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_LeftIdleForTooLong);
			yield return new WaitForSeconds(m_Inactivity.m_DurationToShowCrying);
			Logging.Log("HandleSingingResponseSequence:: Send the SympatheticBaby signal - No response needed");
			yield return this.WaitWhileComAirIsBusy();
			yield return StartCoroutine(SendComAirActionAndWaitForResponse(FurbyAction.Baby_Sympathetic, false, 15f));
			yield return new WaitForSeconds(m_Inactivity.m_DurationToWaitForSympathyToRegister);
			GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_SendingSympathyRequestToFurby);
			yield return StartCoroutine(SendComAirActionAndWaitForResponse(FurbyAction.OriginalSong, true, 15f));
			Logging.Log("HandleSingingResponseSequence:: Checking to see the results");
			if (m_ComAirWaiter.ReceivedResponse())
			{
				Logging.Log("HandleSingingResponseSequence:: There was a response!");
				yield return new WaitForSeconds(m_Inactivity.m_DurationToWaitAfterReceivingAResponse);
				m_CachedTargetModel.animation.wrapMode = WrapMode.Once;
				yield return this.HeartBeatAndWaitOnSend();
				GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_ReceivedResponse);
				yield return new WaitForSeconds(2f);
				yield return this.WaitWhileComAirIsBusy();
				yield return StartCoroutine(SendComAirActionAndWaitForResponse(FurbyAction.Baby_Happy, false, 0f));
				yield return new WaitForSeconds(m_Inactivity.m_DurationToShowThatBabyHappy);
				m_CachedTargetModel.animation.wrapMode = WrapMode.Once;
			}
			else
			{
				Logging.Log("HandleSingingResponseSequence:: No response in time!");
				GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_NoResponse);
				SelectAndInvokeAGenericIdleAnimation();
				yield return new WaitForSeconds(m_Inactivity.m_DurationToShowCryingAfterNoResponse);
				m_CachedTargetModel.animation.wrapMode = WrapMode.Once;
			}
			GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_SequenceComplete);
			m_ReattachAttractMode = true;
		}

		public void InterruptSadSinging()
		{
			if (m_InInactivitySequence)
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.SadSinging_SequenceInterrupted);
				StopCoroutine("AttractMode_HoldingFunction");
				m_ComAirWaiter.Interrupt();
			}
			m_ReattachAttractMode = true;
		}

		public IEnumerator SendComAirActionAndWaitForResponse(FurbyAction action, bool shouldWaitForResponse, float timeout)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse(action, (FurbyCommand)0, shouldWaitForResponse, timeout));
		}

		public void CheckForAndHandleLowStatus()
		{
			if (GameEventRouter.Instance == null || m_HaveHandledLowStatus)
			{
				return;
			}
			List<PlayroomGameEvent> list = new List<PlayroomGameEvent>();
			List<GameObject> list2 = new List<GameObject>();
			if (FurbyGlobals.Player.SelectedFurbyBaby.NewAttention <= m_LowStatusThresholds.NeglectThreshold)
			{
				if (m_NeglectIncident.m_HaveEvent)
				{
					list.Add(m_NeglectIncident.m_EventToRaise);
				}
				if (m_NeglectIncident.m_ObjectToActivate != null)
				{
					list2.Add(m_NeglectIncident.m_ObjectToActivate);
				}
			}
			if (FurbyGlobals.Player.SelectedFurbyBaby.NewSatiatedness <= m_LowStatusThresholds.HungrinessThreshold)
			{
				if (m_HungryIncident.m_HaveEvent)
				{
					list.Add(m_HungryIncident.m_EventToRaise);
				}
				if (m_HungryIncident.m_ObjectToActivate != null)
				{
					list2.Add(m_HungryIncident.m_ObjectToActivate);
				}
			}
			if (FurbyGlobals.Player.SelectedFurbyBaby.NewCleanliness <= m_LowStatusThresholds.DirtinessThreshold)
			{
				if (m_DirtyIncident.m_HaveEvent)
				{
					list.Add(m_DirtyIncident.m_EventToRaise);
				}
				if (m_DirtyIncident.m_ObjectToActivate != null)
				{
					list2.Add(m_DirtyIncident.m_ObjectToActivate);
				}
			}
			if (list.Count > 0 || list2.Count > 0)
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.LowStatus_StartupIsNeglected);
				m_LightObjectToActivate.SetActive(true);
			}
			else
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.LowStatus_StartupNotNeglected);
			}
			m_HaveHandledLowStatus = true;
			foreach (PlayroomGameEvent item in list)
			{
				GameEventRouter.SendEvent(item);
			}
			foreach (GameObject item2 in list2)
			{
				item2.SetActive(true);
			}
		}

		private void DriveIdlingLogic()
		{
			if (m_UnderGracePeriod)
			{
				if (Time.time > m_TimeWhenGracePeriodExpires)
				{
					m_UnderGracePeriod = false;
					SelectAndInvokeAnIdleAnimation();
				}
				else
				{
					SelectAndInvokeAGenericIdleAnimation();
				}
			}
			else
			{
				SelectAndInvokeAnIdleAnimation();
			}
		}

		public bool IsPlayingAnIdleAnimation()
		{
			if (HaveCachedTargetModel() && m_LastInvokedIdleAnimationClip != null)
			{
				return m_CachedTargetModel.animation[m_LastInvokedIdleAnimationClip.name].enabled;
			}
			return false;
		}

		public bool IsAnyAnimationPlaying()
		{
			if (HaveCachedTargetModel())
			{
				return m_CachedTargetModel.animation.isPlaying;
			}
			return false;
		}

		public void SelectAndInvokeAnIdleAnimation()
		{
			if (!HaveCachedTargetModel())
			{
				return;
			}
			FurbyIdleAnimation furbyIdleAnimation;
			if (m_FirstTimeOnSceneStart)
			{
				furbyIdleAnimation = Singleton<PlayroomPersonalityIdling>.Instance.GetGenericFurbyIdleAnimation();
				m_FirstTimeOnSceneStart = false;
			}
			else
			{
				furbyIdleAnimation = Singleton<PlayroomPersonalityIdling>.Instance.GetFurbyIdleAnimation();
			}
			if (furbyIdleAnimation != null && furbyIdleAnimation.m_AnimationClip != null)
			{
				furbyIdleAnimation.m_AnimationClip.wrapMode = WrapMode.Once;
				m_CachedTargetModel.animation.Play(furbyIdleAnimation.m_AnimationClip.name, PlayMode.StopSameLayer);
				m_CachedTargetModel.animation.wrapMode = WrapMode.Once;
				m_LastInvokedIdleAnimationClip = furbyIdleAnimation.m_AnimationClip;
				if (furbyIdleAnimation.m_HaveGameEvent)
				{
					GameEventRouter.SendEvent(furbyIdleAnimation.m_PlayroomGameEvent);
				}
				GameObject.Find("FingerEyeTracking").GetComponent<LookAtTouch>().enabled = furbyIdleAnimation.m_AllowEyeTracking;
			}
		}

		public void SelectAndInvokeAGenericIdleAnimation()
		{
			if (!HaveCachedTargetModel())
			{
				return;
			}
			FurbyIdleAnimation genericFurbyIdleAnimation = Singleton<PlayroomPersonalityIdling>.Instance.GetGenericFurbyIdleAnimation();
			if (genericFurbyIdleAnimation != null)
			{
				genericFurbyIdleAnimation.m_AnimationClip.wrapMode = WrapMode.Once;
				m_CachedTargetModel.animation.Play(genericFurbyIdleAnimation.m_AnimationClip.name, PlayMode.StopSameLayer);
				m_CachedTargetModel.animation.wrapMode = WrapMode.Once;
				m_LastInvokedIdleAnimationClip = genericFurbyIdleAnimation.m_AnimationClip;
				if (genericFurbyIdleAnimation.m_HaveGameEvent)
				{
					GameEventRouter.SendEvent(genericFurbyIdleAnimation.m_PlayroomGameEvent);
				}
				GameObject.Find("FingerEyeTracking").GetComponent<LookAtTouch>().enabled = genericFurbyIdleAnimation.m_AllowEyeTracking;
			}
		}

		private bool IsPlayingAGenericIdleAnimation()
		{
			bool result = false;
			if (HaveCachedTargetModel() && m_LastInvokedIdleAnimationClip != null && m_CachedTargetModel.animation[m_LastInvokedIdleAnimationClip.name].enabled)
			{
				string animName = m_LastInvokedIdleAnimationClip.name;
				result = Singleton<PlayroomPersonalityIdling>.Instance.IsAnimationAGenericIdle(animName);
			}
			return result;
		}
	}
}
