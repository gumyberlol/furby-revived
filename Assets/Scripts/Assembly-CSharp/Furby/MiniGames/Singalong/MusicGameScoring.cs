using System;
using System.Collections.Generic;
using Fabric;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class MusicGameScoring : GameEventReceiver
	{
		[SerializeField]
		private int m_scorePerNote = 1;

		[SerializeField]
		private int[] m_goodNotesRequiredForSuper;

		[SerializeField]
		private int m_superProgressSubtractedByMiss;

		[SerializeField]
		private float m_superProgressMultipliedByMiss = 1f;

		[SerializeField]
		private float m_loseSpecialModeTimeout = 8f;

		[SerializeField]
		private string m_powerUpFabricEvent = "singalong_powerup{0:00}";

		[SerializeField]
		private int m_maxSingalongPowerUp = 12;

		[SerializeField]
		private int m_scoreOnEnteringSpecialMode = 200;

		[SerializeField]
		private int m_scoreOnFinishingSpecialMode = 200;

		private int m_currentScore;

		private int m_currentSuperRank;

		private int m_currentSuperScore;

		private bool m_isInvincible;

		private bool m_canCancelSpecial;

		private float m_timeUntilSpecialCancelled;

		private MusicGameSongPlayer m_songPlayer;

		private MusicGameAudioSync[] m_audioSyncs;

		private float m_lastSeenTime;

		private int m_specialModeActivations;

		private bool m_ShouldOfferDiscoMode = true;

		private List<Fabric.Event> m_queuedEventsForNextBeat = new List<Fabric.Event>();

		public override Type EventType
		{
			get
			{
				return typeof(MusicGameEvent);
			}
		}

		private void Start()
		{
			m_songPlayer = (MusicGameSongPlayer)UnityEngine.Object.FindObjectOfType(typeof(MusicGameSongPlayer));
			m_audioSyncs = EventManager.Instance.GetComponentsInChildren<MusicGameAudioSync>();
			m_ShouldOfferDiscoMode = !FurbyGlobals.Player.NoFurbyOnSaveGame();
		}

		private float GetBPM()
		{
			MusicGameSongData currentSong = m_songPlayer.GetCurrentSong();
			if (currentSong != null)
			{
				return currentSong.GetBPM();
			}
			return 60f;
		}

		private float GetTimeInToBeat()
		{
			MusicGameFabricQuery instance = Singleton<MusicGameFabricQuery>.Instance;
			if (instance != null)
			{
				float num = (float)instance.GetCurrentSongProgressSamples() / (float)instance.GetSampleRate() + 1f / 60f;
				MusicGameSongData currentSong = m_songPlayer.GetCurrentSong();
				if (currentSong != null)
				{
					float num2 = 60f / currentSong.GetBPM();
					float num3 = num / num2 - Mathf.Floor(num / num2);
					return num3 * num2;
				}
			}
			return 0f;
		}

		private void Update()
		{
			if (m_currentSuperScore >= GetCurrentNotesRequiredForSuper() && m_canCancelSpecial)
			{
				m_timeUntilSpecialCancelled -= Time.deltaTime;
				if (m_timeUntilSpecialCancelled < 0f)
				{
					GameEventRouter.SendEvent(MusicGameEvent.RequestDiscoMode);
				}
			}
			MusicGameFabricQuery instance = Singleton<MusicGameFabricQuery>.Instance;
			if (!(instance != null))
			{
				return;
			}
			float num = (float)instance.GetCurrentSongProgressSamples() / (float)instance.GetSampleRate() + 1f / 60f;
			MusicGameSongData currentSong = m_songPlayer.GetCurrentSong();
			if (!(currentSong != null))
			{
				return;
			}
			float num2 = 60f / currentSong.GetBPM();
			int num3 = (int)Mathf.Floor(m_lastSeenTime / num2);
			int num4 = (int)Mathf.Floor(num / num2);
			if (num4 > num3)
			{
				foreach (Fabric.Event item in m_queuedEventsForNextBeat)
				{
					EventManager.Instance.PostEvent(item);
				}
				if (m_queuedEventsForNextBeat.Count > 0)
				{
					MusicGameAudioSync[] audioSyncs = m_audioSyncs;
					foreach (MusicGameAudioSync musicGameAudioSync in audioSyncs)
					{
						musicGameAudioSync.ReSync(GetBPM());
					}
				}
				m_queuedEventsForNextBeat.Clear();
			}
			m_lastSeenTime = num;
		}

		private void ChangeBarSound(int newValue, bool immediate)
		{
			int num = Mathf.Clamp((int)((float)m_currentSuperScore / (float)GetCurrentNotesRequiredForSuper() * (float)m_maxSingalongPowerUp), 0, m_maxSingalongPowerUp - 1);
			int num2 = Mathf.Clamp((int)((float)newValue / (float)GetCurrentNotesRequiredForSuper() * (float)m_maxSingalongPowerUp), 0, m_maxSingalongPowerUp - 1);
			if (newValue != 0 && num2 == 0)
			{
				num2 = 1;
			}
			EventManager.Instance.PostEvent(string.Format(m_powerUpFabricEvent, num), EventAction.SetVolume, 0f);
			EventManager.Instance.PostEvent(string.Format(m_powerUpFabricEvent, num2), EventAction.SetVolume, 1f);
		}

		private void MuteAllBarSounds()
		{
			for (int i = 0; i < m_maxSingalongPowerUp; i++)
			{
				EventManager.Instance.PostEvent(string.Format(m_powerUpFabricEvent, i), EventAction.SetVolume, 0f);
			}
		}

		private void StopAllBarSounds()
		{
			for (int i = 0; i < m_maxSingalongPowerUp; i++)
			{
				Fabric.Event obj = new Fabric.Event();
				obj.EventAction = EventAction.StopSound;
				obj._eventName = string.Format(m_powerUpFabricEvent, i);
				EventManager.Instance.PostEvent(obj);
			}
		}

		private void StartAllLoops()
		{
			for (int i = 0; i < m_maxSingalongPowerUp; i++)
			{
				Fabric.Event obj = new Fabric.Event();
				obj.EventAction = EventAction.PlaySound;
				obj._eventName = string.Format(m_powerUpFabricEvent, i);
				obj._initialiseParameters = new Fabric.InitialiseParameters();
				obj._initialiseParameters._volume = default(InitialiseParameter<float>);
				obj._initialiseParameters._volume.Value = 0f;
				EventManager.Instance.PostEvent(obj);
			}
			MuteAllBarSounds();
		}

		private void ChangeSuperScore(int newScore, bool immediateSound)
		{
			ChangeBarSound(newScore, immediateSound);
			m_currentSuperScore = newScore;
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((MusicGameEvent)(object)enumValue)
			{
			case MusicGameEvent.SongSelected:
				m_currentScore = 0;
				ChangeSuperScore(0, true);
				m_currentSuperRank = 0;
				m_isInvincible = false;
				m_specialModeActivations = 0;
				break;
			case MusicGameEvent.NoteMissed:
				if (!m_isInvincible && m_currentSuperScore < GetCurrentNotesRequiredForSuper())
				{
					int currentSuperScore = m_currentSuperScore;
					currentSuperScore = (int)((float)m_currentSuperScore * m_superProgressMultipliedByMiss);
					currentSuperScore -= m_superProgressSubtractedByMiss;
					if (currentSuperScore < 0)
					{
						currentSuperScore = 0;
					}
					ChangeSuperScore(currentSuperScore, false);
				}
				break;
			case MusicGameEvent.NoteMissedExpired:
				if (!m_isInvincible)
				{
					GameEventRouter.SendEvent(MusicGameEvent.NoteMissedExpiredNotSpecialMode);
				}
				break;
			case MusicGameEvent.NoteMissedTapped:
				if (!m_isInvincible)
				{
					GameEventRouter.SendEvent(MusicGameEvent.NoteMissedTappedNotSpecialMode);
				}
				break;
			case MusicGameEvent.NoteHit:
				if (!m_isInvincible)
				{
					float num = (float)paramList[0];
					m_currentScore += m_scorePerNote;
					ChangeSuperScore(m_currentSuperScore + 1, !(num > 0f));
					if (m_ShouldOfferDiscoMode && m_currentSuperScore == GetCurrentNotesRequiredForSuper())
					{
						GameEventRouter.SendEvent(MusicGameEvent.DiscoModeAvailable);
						GameEventRouter.SendEvent(HintEvents.Singalong_SuggestActivatingSpecialMode_START);
						m_timeUntilSpecialCancelled = m_loseSpecialModeTimeout;
						m_canCancelSpecial = true;
					}
				}
				break;
			case MusicGameEvent.DiscoModeBegin:
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestActivatingSpecialMode_END);
				ChangeBarSound(0, true);
				MuteAllBarSounds();
				m_isInvincible = true;
				m_canCancelSpecial = false;
				m_currentScore += m_scoreOnEnteringSpecialMode;
				m_specialModeActivations++;
				break;
			case MusicGameEvent.DiscoModeEnd:
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestActivatingSpecialMode_END);
				m_currentSuperRank++;
				m_currentScore += m_scoreOnFinishingSpecialMode;
				ChangeSuperScore(0, true);
				MuteAllBarSounds();
				m_isInvincible = false;
				break;
			case MusicGameEvent.DiscoModeOfferExpired:
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestActivatingSpecialMode_END);
				ChangeSuperScore(0, true);
				MuteAllBarSounds();
				break;
			case MusicGameEvent.SongStarted:
				StartAllLoops();
				break;
			case MusicGameEvent.SongFinished:
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestActivatingSpecialMode_END);
				if (m_isInvincible)
				{
					m_currentScore += m_scoreOnFinishingSpecialMode;
				}
				ChangeSuperScore(0, true);
				StopAllBarSounds();
				SendScoreReactions();
				m_isInvincible = false;
				m_canCancelSpecial = false;
				break;
			case MusicGameEvent.Enter:
				StopAllBarSounds();
				break;
			}
		}

		public int GetScore()
		{
			return m_currentScore;
		}

		private void SendScoreReactions()
		{
			MusicGameSongPlayer musicGameSongPlayer = (MusicGameSongPlayer)UnityEngine.Object.FindObjectOfType(typeof(MusicGameSongPlayer));
			if (!(musicGameSongPlayer != null))
			{
				return;
			}
			MusicGameSongData currentSong = musicGameSongPlayer.GetCurrentSong();
			if (currentSong != null)
			{
				if (m_currentScore >= currentSong.GetGoodScore())
				{
					GameEventRouter.SendEvent(MusicGameEvent.ReactionPositive);
				}
				else if (m_currentScore >= currentSong.GetOkScore())
				{
					GameEventRouter.SendEvent(MusicGameEvent.ReactionNeutral);
				}
				else
				{
					GameEventRouter.SendEvent(MusicGameEvent.ReactionNegative);
				}
			}
		}

		public int GetCurrentNotesRequiredForSuper()
		{
			return m_goodNotesRequiredForSuper[Mathf.Min(m_goodNotesRequiredForSuper.Length - 1, m_currentSuperRank)];
		}

		public float GetBarT()
		{
			int currentNotesRequiredForSuper = GetCurrentNotesRequiredForSuper();
			return Mathf.Clamp01((float)m_currentSuperScore / (float)currentNotesRequiredForSuper);
		}
	}
}
