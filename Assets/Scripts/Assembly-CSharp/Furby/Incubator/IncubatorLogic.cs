using System;
using System.Collections.Generic;
using System.Linq;
using Fabric;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorLogic : GameEventConsumer<IncubatorGameEvent>
	{
		[SerializeField]
		private IncubatorTimer m_TimerMeter;

		private IncubatorLevel m_CurrentTimings;

		private List<IncubatorLevel.Interaction> m_InteractionPoints = new List<IncubatorLevel.Interaction>();

		private float m_GameTime;

		private DateTime m_SuspensionTime = DateTime.MaxValue;

		private TimeSpan m_SuspensionInterval = new TimeSpan(0L);

		[HideInInspector]
		private GameConfigBlob m_CachedGameConfigBlob;

		[SerializeField]
		public IncubatorLevel m_FixedTimings;

		private IncubatorLevel.Interaction PendingInteraction
		{
			get
			{
				if (FurbyBaby.IncubationFastForwarded)
				{
					return m_InteractionPoints.LastOrDefault();
				}
				if (IncubationExtended)
				{
					return m_InteractionPoints.LastOrDefault();
				}
				return m_InteractionPoints.FirstOrDefault();
			}
		}

		private IncubatorLevel.Interaction CurrentInteraction
		{
			get
			{
				return m_InteractionPoints.FirstOrDefault();
			}
		}

		private bool IncubationExtended
		{
			get
			{
				bool result = false;
				if (!FurbyBaby.IncubationFastForwarded & IncubationExtensionEnabled)
				{
					result = FurbyGlobals.Player.NoFurbyOnSaveGame();
				}
				return result;
			}
		}

		public bool FastForwardVisible
		{
			get
			{
				bool result = false;
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					result = !FurbyGlobals.Player.IsFirstTimeFlow();
				}
				if (FurbyBaby.m_persistantData.PreAllocatedPersonality)
				{
					result = false;
				}
				return result;
			}
		}

		public bool FastForwardThresholdReached
		{
			get
			{
				bool result = false;
				if (FurbyGlobals.Player.IncubatorEggsHatched >= FastForwardAvailabilityThreshold)
				{
					result = true;
				}
				return result;
			}
		}

		public bool? FastForwardsUsable
		{
			get
			{
				bool? result = null;
				if (!FurbyBaby.IncubationFastForwarded)
				{
					result = Convert.ToBoolean(FurbyGlobals.Player.IncubatorFastForwards);
				}
				return result;
			}
		}

		public IEnumerable<GameConsumable> FastForwardConsumables
		{
			get
			{
				if (!SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable() || !SingletonInstance<RsStoreMediator>.Instance.IsBillingSystemIsAvailable())
				{
					yield break;
				}
				GameConsumable[] incubatorConsumables = m_CachedGameConfigBlob.m_IncubatorConsumables;
				foreach (GameConsumable consumable in incubatorConsumables)
				{
					if (SingletonInstance<RsStoreMediator>.Instance.IsItemAvailable(consumable.StoreID))
					{
						yield return consumable;
					}
				}
			}
		}

		private bool IncubationExtensionEnabled
		{
			get
			{
				return m_CachedGameConfigBlob.m_EnableLongerIncubationTimes;
			}
		}

		private int FastForwardAvailabilityThreshold
		{
			get
			{
				return m_CachedGameConfigBlob.m_FastForwardAvailabilityThreshold;
			}
		}

		public static FurbyBaby FurbyBaby
		{
			get
			{
				return FurbyGlobals.Player.InProgressFurbyBaby;
			}
		}

		private IncubatorTimings FurbyTimings
		{
			get
			{
				return m_CachedGameConfigBlob.m_FurbyIncubatorTimings;
			}
		}

		private IncubatorTimings UpsellTimings
		{
			get
			{
				return m_CachedGameConfigBlob.m_IncubatorTimings;
			}
		}

		private IncubatorTimings TimingsDuringFastForward
		{
			get
			{
				return m_CachedGameConfigBlob.m_IncubatorTimings_FF;
			}
		}

		public void GenerateInteractionPoints()
		{
			m_InteractionPoints.Clear();
			foreach (IncubatorLevel.Interaction item in m_CurrentTimings.GenerateInteractions())
			{
				if (item.Time >= FurbyBaby.IncubationProgress || !item.IsAttentionType)
				{
					m_InteractionPoints.Add(item);
				}
			}
		}

		public void PersistState()
		{
			if (Singleton<FurbyGlobals>.Exists)
			{
				FurbyBaby.LastIncubationTime = DateTime.Now;
			}
			if (Singleton<GameDataStoreObject>.Exists)
			{
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public void Started()
		{
			m_CachedGameConfigBlob = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob();
			int num = (int)(DateTime.Now.Ticks % int.MaxValue);
			if (FurbyBaby.m_persistantData.FixedIncubationTime)
			{
				m_CurrentTimings = m_FixedTimings;
			}
			else
			{
				m_CurrentTimings = CalculateCurrentTimings();
			}
			if (FurbyBaby.IncubationProgress > 0f)
			{
				if (FurbyBaby.IncubationDuration == 0f)
				{
					FurbyBaby.IncubationDuration = m_CurrentTimings.IncubationTime;
				}
				if (FurbyBaby.IncubationDuration != (float)m_CurrentTimings.IncubationTime)
				{
					FurbyBaby.IncubationProgress /= FurbyBaby.IncubationDuration;
					FurbyBaby.IncubationDuration = m_CurrentTimings.IncubationTime;
					FurbyBaby.IncubationProgress *= FurbyBaby.IncubationDuration;
				}
				num = FurbyBaby.IncubationSeed;
			}
			FurbyBaby.IncubationDuration = m_CurrentTimings.IncubationTime;
			UnityEngine.Random.seed = num;
			GenerateInteractionPoints();
			m_TimerMeter.SetProgressMarkers(m_CurrentTimings);
			if (FurbyBaby.IncubationProgress > 0f)
			{
				FurbyBaby.IncubationProgress += (float)(DateTime.Now - FurbyBaby.LastIncubationTime).TotalSeconds;
				FurbyBaby.IncubationProgress = Math.Min(FurbyBaby.IncubationProgress, PendingInteraction.Time);
			}
			FurbyBaby.NextAttentionPointTime = PendingInteraction.GetDateTime(FurbyBaby.IncubationProgress);
			FurbyBaby.IncubationSeed = num;
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			PersistState();
		}

		public new void OnDisable()
		{
			PersistState();
			base.OnDisable();
		}

		public void OnApplicationPause(bool pausing)
		{
			if (!pausing)
			{
				if (m_SuspensionTime != DateTime.MaxValue)
				{
					m_SuspensionInterval = DateTime.Now - m_SuspensionTime;
					m_SuspensionTime = DateTime.MaxValue;
				}
			}
			else
			{
				m_SuspensionInterval = new TimeSpan(0L);
				m_SuspensionTime = DateTime.Now;
				PersistState();
			}
		}

		public void OnForceHatch()
		{
			m_InteractionPoints.RemoveRange(0, m_InteractionPoints.Count - 1);
			FurbyBaby.IncubationProgress = CurrentInteraction.Time - float.Epsilon;
			FurbyBaby.NextAttentionPointTime = DateTime.Now;
		}

		public void OnForceImprint()
		{
			while (CurrentInteraction.ImprintIndex == int.MinValue)
			{
				m_InteractionPoints.RemoveAt(0);
			}
			FurbyBaby.IncubationProgress = CurrentInteraction.Time - float.Epsilon;
			FurbyBaby.NextAttentionPointTime = DateTime.Now;
		}

		public void OnForceAttention(IncubatorLevel.InteractionType attendType)
		{
			IncubatorLevel.Interaction interaction = new IncubatorLevel.Interaction();
			interaction.Type = attendType;
			interaction.Time = FurbyBaby.IncubationProgress + float.Epsilon;
			m_InteractionPoints.Insert(0, interaction);
			FurbyBaby.NextAttentionPointTime = DateTime.Now;
		}

		public IncubatorLevel.Interaction IncubationProgress()
		{
			FurbyBaby.IncubationProgress += Time.deltaTime;
			m_GameTime += Time.deltaTime;
			FurbyBaby.IncubationProgress += (float)m_SuspensionInterval.TotalSeconds;
			m_SuspensionInterval = new TimeSpan(0L);
			if (PendingInteraction != null)
			{
				IncubatorLevel.Interaction currentInteraction = CurrentInteraction;
				if (FurbyBaby.IncubationProgress >= currentInteraction.Time)
				{
					if (CurrentInteraction == PendingInteraction)
					{
						FurbyBaby.IncubationProgress = currentInteraction.Time;
						GameEventRouter.SendEvent((IncubatorSequenceEvent)currentInteraction.Type);
						return currentInteraction;
					}
					if (currentInteraction.Type == IncubatorLevel.InteractionType.Imprinting)
					{
						ImprintFinished(null, false);
					}
				}
				if (0.125f > Math.Abs(currentInteraction.Time - FurbyBaby.IncubationProgress - 4.25f))
				{
					Singleton<FurbyDataChannel>.Instance.PostCommand(FurbyCommand.Application, null);
				}
			}
			FurbyBaby.LastIncubationTime = DateTime.Now;
			return null;
		}

		public FurbyBabyPersonality CombinePersonalities()
		{
			Type typeFromHandle = typeof(FurbyBabyPersonality);
			FurbyBabyPersonality furbyBabyPersonality = FurbyBabyPersonality.None;
			FurbyPersonality[] incubationPersonalities = FurbyBaby.IncubationPersonalities;
			foreach (FurbyPersonality furbyPersonality in incubationPersonalities)
			{
				furbyBabyPersonality |= FurbyBaby.MapPersonality(furbyPersonality);
			}
			if (Enum.IsDefined(typeFromHandle, furbyBabyPersonality))
			{
				return furbyBabyPersonality;
			}
			return FurbyBaby.MapPersonality(FurbyBaby.GetLatestPersonality().Value);
		}

		public void OnPurchaseFastForward(int purchaseUnits)
		{
			FurbyGlobals.Player.IncubatorFastForwards += purchaseUnits;
		}

		public void OnFastForwardActivate()
		{
			FurbyBaby.IncubationFastForwarded = true;
			FurbyGlobals.Player.IncubatorFastForwards--;
			m_CurrentTimings = CalculateCurrentTimings();
			FurbyBaby.IncubationProgress /= FurbyBaby.IncubationDuration;
			FurbyBaby.IncubationDuration = m_CurrentTimings.IncubationTime;
			FurbyBaby.IncubationProgress *= FurbyBaby.IncubationDuration;
			GenerateInteractionPoints();
			FurbyBaby.NextAttentionPointTime = PendingInteraction.GetDateTime(FurbyBaby.IncubationProgress);
		}

		public void ImprintFinished(FurbyPersonality? furbyType, bool personalityOverriden)
		{
			FurbyBaby.IncubationPersonalityOverriden = personalityOverriden;
			if (!furbyType.HasValue)
			{
				furbyType = (FurbyPersonality)(917 + UnityEngine.Random.Range(0, 5));
			}
			FurbyBaby.SetLatestPersonality(furbyType.Value);
			if (CurrentInteraction != null)
			{
				m_InteractionPoints.RemoveAt(0);
			}
			if (CurrentInteraction == null)
			{
				FurbyBaby.SetPersonality(CombinePersonalities(), FurbyBaby.Level);
				string[] flairs = FurbyBaby.m_persistantData.flairs;
				foreach (string text in flairs)
				{
					FlairLibrary.PrefabLoader prefabLoader = FurbyGlobals.FlairLibrary.GetPrefabLoader(text);
					if (!string.IsNullOrEmpty(prefabLoader.Flair.VocalSwitch))
					{
						Fabric.Event obj = new Fabric.Event();
						obj.EventAction = EventAction.SetSwitch;
						obj._eventName = "vocal_switch";
						obj._parameter = prefabLoader.Flair.VocalSwitch;
						EventManager.Instance.PostEvent(obj);
					}
				}
				FurbyBaby.Progress = FurbyBabyProgresss.P;
				FurbyBaby.m_persistantData.HatchingTime = DateTime.Now.Ticks;
				FurbyGlobals.Player.IncubatorEggsHatched++;
				GameEventRouter.SendEvent(BabyLifecycleEvent.BabyHatched, null, FurbyBaby);
				GameData data = Singleton<GameDataStoreObject>.Instance.Data;
				data.MarkEggHatched();
				if (FurbyGlobals.Player.FlowStage == FlowStage.Incubator_First)
				{
					FurbyGlobals.Player.FlowStage = FlowStage.Dashboard_InitialBaby;
				}
			}
			else
			{
				FurbyBaby.NextAttentionPointTime = PendingInteraction.GetDateTime(FurbyBaby.IncubationProgress);
			}
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		public void AttentionFinished()
		{
			FurbyBaby.IncubationProgress = CurrentInteraction.Time + 0.01f;
			m_InteractionPoints.RemoveAt(0);
			if (m_InteractionPoints.Count > 0)
			{
				FurbyBaby.NextAttentionPointTime = CurrentInteraction.GetDateTime(FurbyBaby.IncubationProgress);
			}
			PersistState();
		}

		private IncubatorLevel CalculateCurrentTimings()
		{
			IncubatorLevel exactLevel = FurbyTimings.GetExactLevel(FurbyBaby.GetEffectiveLevel());
			if (FurbyBaby.IncubationFastForwarded)
			{
				return TimingsDuringFastForward.GetExactLevel(FurbyBaby.GetEffectiveLevel());
			}
			if (IncubationExtended)
			{
				IncubatorLevel minimumLevel = UpsellTimings.GetMinimumLevel(FurbyGlobals.Player.IncubatorEggsHatched);
				minimumLevel.ImprintCount = exactLevel.ImprintCount;
				return minimumLevel;
			}
			return exactLevel;
		}

		public bool AreFastForwardsViable()
		{
			float num = FurbyBaby.IncubationDuration - FurbyBaby.IncubationProgress;
			return num > GetExpectedFastForwardDuration();
		}

		public float GetExpectedFastForwardDuration()
		{
			IncubatorLevel minimumLevel = TimingsDuringFastForward.GetMinimumLevel(FurbyGlobals.Player.IncubatorEggsHatched);
			return minimumLevel.IncubationTime;
		}
	}
}
