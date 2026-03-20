using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorGameFlow : GameEventConsumer<IncubatorSequenceEvent>
	{
		[SerializeField]
		private IncubatorDialogFlow m_DialogFlow;

		[SerializeField]
		private IncubatorEggInteraction m_IncubatorInteraction;

		[SerializeField]
		private IncubatorPersonalityDialog m_PersonalitySongDialog;

		[SerializeField]
		private IncubatorLogic m_GameLogic;

		[SerializeField]
		private BabyInstance m_PreHatchingModel;

		[SerializeField]
		private BabyInstance m_HatchingModel;

		[SerializeField]
		private BabyInstance m_PostHatchingModel;

		[SerializeField]
		public float m_SaveIntervalTime = 10f;

		[SerializeField]
		public float m_PersonalityValidityTime = 10f;

		[SerializeField]
		public float m_HintingInterval = 4f;

		[SerializeField]
		private float m_IdleInteractionInterval = 0.5f;

		[SerializeField]
		private float m_IdleInteractionGraceTime = 2f;

		[SerializeField]
		private float m_ImprintAttractTime = 3f;

		[SerializeField]
		private float m_ImprintRetryTime = 6f;

		[SerializeField]
		private int m_ImprintRetryCount = 3;

		[SerializeField]
		private float m_ImprintAbsorbEffectTime = 4f;

		[SerializeField]
		private float m_ManualImprintInteractTime = 1.5f;

		[SerializeField]
		private float m_AbsorbMusicTime = 20f;

		[SerializeField]
		private float m_AbsorbSongTime = 5f;

		[SerializeField]
		private float m_FinalAbsorbMusicTime = 8f;

		[SerializeField]
		private float m_FinalAbsorbSongTime = 5f;

		[SerializeField]
		private float m_ClockHatchEffectTime = 5f;

		[SerializeField]
		private float m_ClockInteractionEffectTime = 5f;

		[SerializeField]
		private float m_AttentionTime = 4f;

		[SerializeField]
		private float m_AttentionReactionInterval = 10f;

		[SerializeField]
		private float m_PostAttentionTime = 3f;

		[SerializeField]
		private float m_PreCrackHatchTime = 14f;

		[SerializeField]
		private float m_PostCrackHatchTime = 18f;

		[SerializeField]
		private float m_PostHatchTime = 1f;

		public IncubatorBackgroundTweenTwiddler m_RotatingBackground;

		public IncubatorClockHandler m_Clock;

		private static readonly IncubatorGameEvent[] m_WarmingEvents = new IncubatorGameEvent[7]
		{
			IncubatorGameEvent.Warming_Start,
			IncubatorGameEvent.Warming_Interaction_Start,
			IncubatorGameEvent.Warming_Interaction_Feedback_Start,
			IncubatorGameEvent.Warming_Progress,
			IncubatorGameEvent.Warming_Interaction_Finish,
			IncubatorGameEvent.Warming_Interaction_Feedback_Finish,
			IncubatorGameEvent.Warming_Finished
		};

		private static readonly IncubatorGameEvent[] m_ComfortingEvents = new IncubatorGameEvent[7]
		{
			IncubatorGameEvent.Comforting_Start,
			IncubatorGameEvent.Comforting_Interaction_Start,
			IncubatorGameEvent.Comforting_Interaction_Feedback_Start,
			IncubatorGameEvent.Comforting_Progress,
			IncubatorGameEvent.Comforting_Interaction_Finish,
			IncubatorGameEvent.Comforting_Interaction_Feedback_Finish,
			IncubatorGameEvent.Comforting_Finished
		};

		private static readonly IncubatorGameEvent[] m_CleaningEvents = new IncubatorGameEvent[7]
		{
			IncubatorGameEvent.Cleaning_Start,
			IncubatorGameEvent.Cleaning_Interaction_Start,
			IncubatorGameEvent.Cleaning_Interaction_Feedback_Start,
			IncubatorGameEvent.Cleaning_Progress,
			IncubatorGameEvent.Cleaning_Interaction_Finish,
			IncubatorGameEvent.Cleaning_Interaction_Feedback_Finish,
			IncubatorGameEvent.Cleaning_Finished
		};

		public event Action ManualImprintStart;

		public event Action ManualImprintEnd;

		private IEnumerator Start()
		{
			GameEventRouter.SendEvent(IncubatorGameEvent.IncubatorOpened);
			if (IncubatorLogic.FurbyBaby.IncubationFastForwarded)
			{
				GameEventRouter.SendEvent(IncubatorGameEvent.IncubatorOpened_FastForward_Active);
			}
			else
			{
				GameEventRouter.SendEvent(IncubatorGameEvent.IncubatorOpened_FastForward_Inactive);
			}
			if (FurbyGlobals.Player.FlowStage == FlowStage.Incubator_First)
			{
				GameEventRouter.SendEvent(IncubatorGameEvent.IncubatorOpened_FirstTimeFlow);
			}
			m_GameLogic.Started();
			m_GameLogic.IncubationProgress();
			IEnumerator sequence = InteractionFlow(PumpEvents());
			while (sequence.MoveNext())
			{
				sequence = (sequence.Current as IEnumerator) ?? sequence;
				yield return sequence.Current;
			}
		}

		public void OnDestroy()
		{
			StopAllCoroutines();
		}

		private IEnumerator HintingFlow(Enum hintType)
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			while (true)
			{
				InputInactivity inputInactivity = FurbyGlobals.InputInactivity;
				if (inputInactivity.HasIntervalPassed(m_HintingInterval))
				{
					inputInactivity.ResetInactivity();
					GameEventRouter.SendEvent(hintType);
				}
				yield return null;
			}
		}

		private IEnumerator InteractionFlow(IncubatorSequenceEvent? sequenceEvent)
		{
			if (sequenceEvent.HasValue)
			{
				switch (sequenceEvent.Value)
				{
				case IncubatorSequenceEvent.Interruption_Attention_Cold:
					return AttentionFlow(m_WarmingEvents);
				case IncubatorSequenceEvent.Interruption_Attention_Scared:
					return AttentionFlow(m_ComfortingEvents);
				case IncubatorSequenceEvent.Interruption_Attention_Dusty:
					return AttentionFlow(m_CleaningEvents);
				case IncubatorSequenceEvent.Interruption_Imprint:
					return ImprintingFlow(false);
				case IncubatorSequenceEvent.Interruption_Hatch:
					return HatchingFlow();
				}
			}
			return CountdownFlow();
		}

		private IEnumerator CountdownFlow()
		{
			if (IncubatorLogic.FurbyBaby.IncubationPersonalityOverriden)
			{
				return AbsorptionCountdownFlow();
			}
			return SelectCountdownFlow();
		}

		private IEnumerator SaveDataFlow()
		{
			while (true)
			{
				foreach (object item in Delay(m_SaveIntervalTime))
				{
					yield return item;
				}
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private IEnumerator DefaultInteractionFlow()
		{
			IEnumerator gameSaver = SaveDataFlow();
			float interactTime = m_IdleInteractionGraceTime;
			float idleTime = m_IdleInteractionGraceTime;
			while (true)
			{
				if (m_IncubatorInteraction.Active)
				{
					m_IncubatorInteraction.Active = false;
					if (interactTime <= 0f)
					{
						GameEventRouter.SendEvent(IncubatorGameEvent.Idle_Interaction);
						interactTime = m_IdleInteractionInterval;
						idleTime = UnityEngine.Random.Range(4, 8);
					}
				}
				else if (idleTime <= 0f)
				{
					GameEventRouter.SendEvent(IncubatorGameEvent.Idle);
					idleTime = UnityEngine.Random.Range(4, 8);
				}
				gameSaver.MoveNext();
				yield return null;
				interactTime -= Time.deltaTime;
				idleTime -= Time.deltaTime;
			}
		}

		private IEnumerator FastForwardInteractionFlow()
		{
			IEnumerator gameSaver = SaveDataFlow();
			while (true)
			{
				gameSaver.MoveNext();
				yield return null;
			}
		}

		private IEnumerator SelectCountdownFlow()
		{
			if (IncubatorLogic.FurbyBaby.IncubationFastForwarded)
			{
				return FastForwardCountdownFlow(false);
			}
			return DefaultCountdownFlow();
		}

		private IEnumerator DefaultCountdownFlow()
		{
			IEnumerator idleFlow = DefaultInteractionFlow();
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Started);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Start);
			while (!PumpEvents().HasValue)
			{
				idleFlow.MoveNext();
				yield return m_DialogFlow.Process();
				m_GameLogic.IncubationProgress();
				if (IncubatorLogic.FurbyBaby.IncubationFastForwarded)
				{
					yield return FastForwardCountdownFlow(true);
				}
			}
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Finish);
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Finished);
			yield return InteractionFlow(LastEvent.Value);
		}

		private IEnumerator FastForwardCountdownFlow(bool continuing)
		{
			IEnumerator idleFlow = FastForwardInteractionFlow();
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Clock_Enter);
			m_RotatingBackground.EnableRotation();
			m_Clock.gameObject.SetActive(true);
			m_Clock.Resume();
			if (!continuing)
			{
				GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Started);
				GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Start);
			}
			while (!PumpEvents().HasValue)
			{
				idleFlow.MoveNext();
				m_GameLogic.IncubationProgress();
				yield return null;
			}
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Finish);
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Finished);
			if (LastEvent.Value == IncubatorSequenceEvent.Interruption_Hatch)
			{
				m_Clock.InvokeEndOfSequenceAnimation();
				m_RotatingBackground.DisableRotation_WithEaseOut();
				GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Clock_Hatch);
				yield return new WaitForSeconds(m_ClockHatchEffectTime);
			}
			else
			{
				m_Clock.Suspend();
				m_RotatingBackground.DisableRotation_WithEaseOut();
			}
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Clock_Leave);
			yield return InteractionFlow(LastEvent.Value);
		}

		private IEnumerator AbsorptionCountdownFlow()
		{
			bool singingPending = true;
			float singingInterval = m_AbsorbSongTime;
			IncubatorGameEvent startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Singing;
			IncubatorGameEvent finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Singing;
			IEnumerator idleFlow = DefaultInteractionFlow();
			if (IncubatorLogic.FurbyBaby.IncubationPersonalityOverriden)
			{
				FurbyPersonality? latestPersonality = IncubatorLogic.FurbyBaby.GetLatestPersonality();
				if (latestPersonality.HasValue)
				{
					switch (latestPersonality.Value)
					{
					case FurbyPersonality.Gobbler:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Gobbler;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Gobbler;
						break;
					case FurbyPersonality.Kooky:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Kooky;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Kooky;
						break;
					case FurbyPersonality.Base:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_RockStar;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_RockStar;
						break;
					case FurbyPersonality.SweetBelle:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_SweetBelle;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_SweetBelle;
						break;
					case FurbyPersonality.ToughGirl:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_ToughGirl;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_ToughGirl;
						break;
					}
				}
				singingInterval = m_AbsorbMusicTime;
			}
			GameEventRouter.SendEvent(startEvent);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Start);
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Started);
			foreach (IncubatorSequenceEvent? sequenceEvent in AwaitAny(false))
			{
				singingInterval -= Time.deltaTime;
				if (singingInterval < 0f)
				{
					if (singingPending)
					{
						GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Singing);
						singingPending = false;
					}
					idleFlow.MoveNext();
				}
				yield return sequenceEvent;
				m_GameLogic.IncubationProgress();
			}
			GameEventRouter.SendEvent(finishEvent);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Music_Finish);
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Finished);
			yield return InteractionFlow(LastEvent.Value);
		}

		private IEnumerator AttentionReactionFlow()
		{
			while (true)
			{
				Singleton<FurbyDataChannel>.Instance.PostAction(FurbyAction.Egg_Attention, null);
				yield return new WaitForSeconds(m_AttentionReactionInterval);
			}
		}

		private IEnumerator AttentionFlow(IncubatorGameEvent[] eventTable)
		{
			float attentionTime = 0f;
			bool feedbackActive = false;
			IncubatorEggInteraction interaction = m_IncubatorInteraction;
			FurbyGlobals.InputInactivity.ResetInactivity();
			GameEventRouter.SendEvent(eventTable[0]);
			GameEventRouter.SendEvent(FlowDialog.Incubator_Rub);
			StartCoroutine("HintingFlow", HintEvents.Incubation_MissedAnAttentionPoint);
			if (!FurbyGlobals.Player.NoFurbyForEitherReason())
			{
				StartCoroutine("AttentionReactionFlow");
			}
			while (attentionTime < m_AttentionTime)
			{
				yield return StartCoroutine(Idling(float.MaxValue));
				GameEventRouter.SendEvent(eventTable[1]);
				if (!feedbackActive)
				{
					StopCoroutine("HintingFlow");
					GameEventRouter.SendEvent(eventTable[2]);
				}
				float i = attentionTime;
				while (interaction.Active && i < m_AttentionTime)
				{
					float ratio = Mathf.Clamp01(i / m_AttentionTime);
					GameEventRouter.SendEvent(eventTable[3], null, ratio);
					yield return null;
					i += Time.deltaTime;
				}
				yield return StartCoroutine(Interacting(m_AttentionTime - attentionTime));
				attentionTime += interaction.Contribution;
				GameEventRouter.SendEvent(eventTable[4]);
				if (!interaction.Active)
				{
					yield return StartCoroutine(Idling(2f - interaction.Duration));
					if (!interaction.Active)
					{
						StartCoroutine("HintingFlow", HintEvents.Incubation_MissedAnAttentionPoint);
						GameEventRouter.SendEvent(eventTable[5]);
					}
					feedbackActive = interaction.Active;
				}
				else
				{
					interaction.Active = false;
					attentionTime = m_AttentionTime;
					GameEventRouter.SendEvent(eventTable[5]);
				}
			}
			StopCoroutine("AttentionReactionFlow");
			StopCoroutine("HintingFlow");
			m_GameLogic.AttentionFinished();
			GameEventRouter.SendEvent(eventTable[6]);
			GameEventRouter.SendEvent(FlowDialog.Hide_Dialog);
			yield return new WaitForSeconds(m_PostAttentionTime);
			yield return CountdownFlow();
		}

		private IEnumerator NoFurbyImprintingFlow(bool lastImprint)
		{
			if (IncubatorLogic.FurbyBaby.m_persistantData.PreAllocatedPersonality)
			{
				yield return new WaitForSeconds(0.5f);
				yield break;
			}
			yield return StartCoroutine(Interacting(float.MaxValue));
			bool fingerHeldDown = false;
			while (!fingerHeldDown)
			{
				StartCoroutine("HintingFlow", HintEvents.Incubation_SuggestManualImprint);
				yield return StartCoroutine(Idling(float.MaxValue));
				StopCoroutine("HintingFlow");
				base.gameObject.SendGameEvent(IncubatorGameEvent.Incubator_NoFurby_Imprint_Start);
				if (this.ManualImprintStart != null)
				{
					this.ManualImprintStart();
				}
				yield return StartCoroutine(Interacting(m_ManualImprintInteractTime));
				if (this.ManualImprintEnd != null)
				{
					this.ManualImprintEnd();
				}
				fingerHeldDown = m_IncubatorInteraction.Active;
				IncubatorGameEvent resultEvent = ((!fingerHeldDown) ? IncubatorGameEvent.Incubator_NoFurby_Imprint_Premature_End : IncubatorGameEvent.Incubator_NoFurby_Imprint_Success);
				base.gameObject.SendGameEvent(resultEvent);
			}
			GameEventRouter.SendEvent(HintEvents.Incubation_SuggestManualImprint_Expired);
			m_IncubatorInteraction.Active = false;
			StopCoroutine("HintingFlow");
			m_GameLogic.ImprintFinished(null, false);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Personality);
			GameEventRouter.SendEvent(FlowDialog.Hide_Dialog);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Short_Start);
			yield return new WaitForSeconds(m_ImprintAbsorbEffectTime);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_Short_Finish);
			yield return SelectCountdownFlow();
		}

		private IEnumerator FurbyImprintingFlow(bool lastImprint)
		{
			float imprintTime = Time.time;
			int attemptCount = m_ImprintRetryCount;
			WaitForGameEvent eventWaiter = new WaitForGameEvent();
			yield return new WaitForSeconds(m_ImprintAttractTime);
			do
			{
				if (imprintTime <= Time.time)
				{
					if (attemptCount > 0)
					{
						Singleton<FurbyDataChannel>.Instance.PostAction(FurbyAction.Egg_Sitting, null);
						imprintTime = Time.time + m_ImprintRetryTime;
						attemptCount--;
						continue;
					}
					GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Failed);
					yield return StartCoroutine(eventWaiter.WaitForAnyEventOfType(typeof(IncubatorGameEvent)));
					if ((IncubatorGameEvent)(object)eventWaiter.ReturnedEvent == IncubatorGameEvent.Imprint_Abort)
					{
						string destination = ((Singleton<GameDataStoreObject>.Instance.Data.FlowStage == FlowStage.Normal) ? "Dashboard" : "SaveSlotSelect");
						StopAllCoroutines();
						bool rememberBack = false;
						FurbyGlobals.ScreenSwitcher.SwitchScreen(destination, rememberBack);
						yield break;
					}
					yield return new WaitForSeconds(0.25f);
					imprintTime = Time.time + m_ImprintRetryTime;
					attemptCount = m_ImprintRetryCount;
				}
				else
				{
					yield return null;
				}
			}
			while (Singleton<FurbyDataChannel>.Instance.ResponseInterval > m_PersonalityValidityTime);
			GameEventRouter.SendEvent(FlowDialog.Hide_Dialog);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Personality_Query);
			yield return StartCoroutine(m_PersonalitySongDialog.ShowModal());
			if (lastImprint)
			{
				yield return StartCoroutine(ShortMusicAbsorptionFlow());
			}
			else
			{
				yield return AbsorptionCountdownFlow();
			}
		}

		private IEnumerator ImprintingFlow(bool lastImprint)
		{
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Point);
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				GameEventRouter.SendEvent(FlowDialog.Incubator_NoFurby);
				return NoFurbyImprintingFlow(lastImprint);
			}
			GameEventRouter.SendEvent(FlowDialog.Incubator_Furby);
			return FurbyImprintingFlow(lastImprint);
		}

		private IEnumerator ShortMusicAbsorptionFlow()
		{
			float sequenceTime = m_FinalAbsorbSongTime;
			IncubatorGameEvent startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Singing;
			IncubatorGameEvent finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Singing;
			if (IncubatorLogic.FurbyBaby.IncubationPersonalityOverriden)
			{
				FurbyPersonality? latestPersonality = IncubatorLogic.FurbyBaby.GetLatestPersonality();
				if (latestPersonality.HasValue)
				{
					switch (latestPersonality.Value)
					{
					case FurbyPersonality.Gobbler:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Gobbler;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Gobbler;
						break;
					case FurbyPersonality.Kooky:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_Kooky;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_Kooky;
						break;
					case FurbyPersonality.Base:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_RockStar;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_RockStar;
						break;
					case FurbyPersonality.SweetBelle:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_SweetBelle;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_SweetBelle;
						break;
					case FurbyPersonality.ToughGirl:
						startEvent = IncubatorGameEvent.Imprint_Absorb_Music_Start_ToughGirl;
						finishEvent = IncubatorGameEvent.Imprint_Absorb_Music_Finish_ToughGirl;
						break;
					}
				}
				sequenceTime = m_FinalAbsorbMusicTime;
			}
			GameEventRouter.SendEvent(startEvent);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_FinalMusic_Start);
			yield return new WaitForSeconds(sequenceTime);
			GameEventRouter.SendEvent(finishEvent);
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Absorb_FinalMusic_Finish);
			yield return CountdownFlow();
		}

		private IEnumerator HatchingFlow()
		{
			yield return StartCoroutine(ImprintingFlow(true));
			m_PostHatchingModel.ReInstantiateFlairs();
			GameEventRouter.SendEvent(IncubatorGameEvent.Hatching_Start);
			m_PreHatchingModel.Hide();
			m_HatchingModel.Show();
			m_PostHatchingModel.Show();
			yield return new WaitForSeconds(m_PreCrackHatchTime);
			GameEventRouter.SendEvent(IncubatorGameEvent.Hatching_CrackOpen);
			StartCoroutine("HatchingInteractionFlow");
			yield return new WaitForSeconds(m_PostCrackHatchTime);
			GameEventRouter.SendEvent(IncubatorGameEvent.Hatching_Burst);
			StopCoroutine("HatchingInteractionFlow");
			m_PostHatchingModel.UnhideFlairs();
			yield return StartCoroutine(Animating(m_PostHatchingModel.Instance));
			FurbyGlobals.ScreenSwitcher.Clear();
			GameEventRouter.SendEvent(IncubatorGameEvent.Hatching_Finished);
			yield return StartCoroutine(Animating(m_PostHatchingModel.Instance));
			yield return new WaitForSeconds(m_PostHatchTime);
			if (FurbyGlobals.Player.NoFurbyOnSaveGame() && FurbyGlobals.Player.IncubatorEggsHatched == 1)
			{
				GameEventRouter.SendEvent(TutorialVideoEvents.HatchingVideo);
				yield return new WaitForSeconds(0.25f);
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForEvent(VideoPlayerGameEvents.VideoHasFinished));
			}
			if (FurbyGlobals.Player.InProgressFurbyBaby.Tribe.TribeSet == Tribeset.Golden)
			{
				FurbyGlobals.ScreenSwitcher.Clear();
				FurbyGlobals.ScreenSwitcher.SwitchScreen("GoldenUnlocking", false);
			}
			else
			{
				FurbyGlobals.ScreenSwitcher.Clear();
				FurbyGlobals.ScreenSwitcher.SwitchScreen("BabyName", false);
			}
			StopAllCoroutines();
		}

		private IEnumerator HatchingInteractionFlow()
		{
			float allowTime = Time.time;
			while (true)
			{
				if (m_IncubatorInteraction.Active && allowTime < Time.time)
				{
					GameEventRouter.SendEvent(IncubatorGameEvent.Hatching_Interaction_Start);
					allowTime = m_IdleInteractionInterval + Time.time;
					m_IncubatorInteraction.Active = false;
				}
				yield return null;
			}
		}

		private IEnumerator Animating(GameObject gameObj)
		{
			while (gameObj.animation.isPlaying)
			{
				yield return null;
			}
		}

		private IEnumerator Idling(float delayTime)
		{
			while (!m_IncubatorInteraction.Active && delayTime > 0f)
			{
				yield return null;
				delayTime -= Time.deltaTime;
			}
		}

		private IEnumerator Interacting(float delayTime)
		{
			while (m_IncubatorInteraction.Active && delayTime > 0f)
			{
				yield return null;
				delayTime -= Time.deltaTime;
			}
		}

		private IEnumerable Delay(float delayTime)
		{
			while (delayTime > 0f)
			{
				yield return null;
				delayTime -= Time.deltaTime;
			}
		}
	}
}
