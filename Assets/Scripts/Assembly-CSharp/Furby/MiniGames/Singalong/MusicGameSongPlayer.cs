using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class MusicGameSongPlayer : GameEventReceiver
	{
		[Serializable]
		public class MusicGameSettings
		{
			[SerializeField]
			public float m_hitWindow = 0.2f;
		}

		public enum PlayerState
		{
			NotPlayingSong = 0,
			PlayingSongStandard = 1,
			DiscoMode = 2
		}

		private MusicGameSongData m_songData;

		[SerializeField]
		private ErrorMessageBox m_NoFurbyErrorDialog;

		[SerializeField]
		private SongRailPlayer[] m_rails;

		[SerializeField]
		private float m_filterFreq = 450f;

		[SerializeField]
		private DiscoModePlayer m_discoPlayer;

		[SerializeField]
		private MusicGameSettings m_gameSettings;

		[SerializeField]
		private GameObject m_discoMeter;

		private bool m_hasPlayed;

		private bool m_discoModeAdvertised;

		private PlayerState m_currentState;

		public PlayerState CurrentPlayerState
		{
			get
			{
				return m_currentState;
			}
		}

		public override Type EventType
		{
			get
			{
				return typeof(MusicGameEvent);
			}
		}

		private void OnDestroy()
		{
			if (Singleton<FurbyDataChannel>.Exists)
			{
				Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			}
			GameEventRouter.RemoveDelegateForEnums(OnGamePause, SharedGuiEvents.Pause);
			GameEventRouter.RemoveDelegateForEnums(OnGameUnPause, SharedGuiEvents.Resume);
		}

		private void Start()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Sing_A_Long);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			GameEventRouter.SendEvent(MusicGameEvent.Enter);
			GameEventRouter.AddDelegateForEnums(OnGamePause, SharedGuiEvents.Pause);
			GameEventRouter.AddDelegateForEnums(OnGameUnPause, SharedGuiEvents.Resume);
		}

		private void StartPlayingSong(MusicGameSongData selectedSong)
		{
			m_songData = selectedSong;
			MusicGameSongData.RailInfo[] rails = m_songData.GetRails();
			int num = 0;
			SongRailPlayer[] rails2 = m_rails;
			foreach (SongRailPlayer songRailPlayer in rails2)
			{
				songRailPlayer.Setup(rails[num], m_gameSettings);
				num++;
			}
			GameEventRouter.SendEvent(selectedSong.GetStartEvent());
			GameEventRouter.SendEvent(MusicGameEvent.SongStarted);
			m_currentState = PlayerState.PlayingSongStandard;
			m_hasPlayed = false;
		}

		private void UpdateSong(bool shouldPlayNotes)
		{
			if (Singleton<MusicGameFabricQuery>.Exists)
			{
				MusicGameFabricQuery instance = Singleton<MusicGameFabricQuery>.Instance;
				int currentSongProgressSamples = instance.GetCurrentSongProgressSamples();
				float songTime = (float)currentSongProgressSamples / (float)GetSampleRate();
				SongRailPlayer[] rails = m_rails;
				foreach (SongRailPlayer songRailPlayer in rails)
				{
					if (m_discoModeAdvertised)
					{
						songRailPlayer.UpdateWithSongTime(songTime, false);
					}
					else
					{
						songRailPlayer.UpdateWithSongTime(songTime, shouldPlayNotes);
					}
				}
				if (m_hasPlayed)
				{
					if (currentSongProgressSamples == -1)
					{
						GameEventRouter.SendEvent(MusicGameEvent.SongFinished);
						GameEventRouter.SendEvent(BabyEndMinigameEvent.SetScore, base.gameObject, m_discoPlayer.Score);
						GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowDialog);
						GameEventRouter.SendEvent(MusicGameEvent.GameEnded);
					}
				}
				else if (currentSongProgressSamples > 0)
				{
					m_hasPlayed = true;
				}
			}
			else
			{
				Logging.LogError("MusicGame: Trying to play song, but can't query progress - MusicGameFabricQuery should exist in the fabric hierarchy");
			}
		}

		private void Update()
		{
			switch (m_currentState)
			{
			case PlayerState.NotPlayingSong:
				m_discoModeAdvertised = false;
				break;
			case PlayerState.PlayingSongStandard:
				UpdateSong(true);
				break;
			case PlayerState.DiscoMode:
				UpdateSong(!(m_discoPlayer == null) && m_discoPlayer.ShouldNotesContinue());
				m_discoModeAdvertised = false;
				break;
			}
		}

		private IEnumerator StartSongCoroutine(MusicGameSongData songData)
		{
			yield return new WaitForSeconds(2f);
			if (!FurbyGlobals.Player.NoFurbyForEitherReason())
			{
				Logging.Log("Furby exists - trying to connect");
				while (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
				{
					yield return new WaitForSeconds(6f);
					Logging.Log("Trying to post singalong");
					if (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
					{
						GameObject pauseMenu = GameObject.Find("PauseButtonPanel");
						WaitForGameEvent eventWaiter = new WaitForGameEvent();
						m_NoFurbyErrorDialog.SetAcceptCancelState("SCANNER_NOTFOUND_INITIAL_TEXT", "FURBYCOMMSERROR_RETRY", SharedGuiEvents.DialogAccept, "MENU_OPTION_CONTINUENOFURBY", SharedGuiEvents.DialogCancel);
						GameEventRouter.SendEvent(MusicGameEvent.FurbyNotFound);
						pauseMenu.SetActive(false);
						m_NoFurbyErrorDialog.gameObject.SetActive(true);
						m_NoFurbyErrorDialog.Show(true);
						yield return StartCoroutine(eventWaiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
						pauseMenu.SetActive(true);
						m_NoFurbyErrorDialog.gameObject.SetActive(false);
						m_NoFurbyErrorDialog.Hide();
						if ((SharedGuiEvents)(object)eventWaiter.ReturnedEvent == SharedGuiEvents.DialogCancel)
						{
							break;
						}
						yield return new WaitForSeconds(3f);
						Logging.Log("Wait over");
					}
				}
			}
			StartPlayingSong(songData);
			yield return null;
		}

		private IEnumerator ReSendSongTone()
		{
			if (m_songData != null && !FurbyGlobals.Player.NoFurbyForEitherReason())
			{
				while (!Singleton<FurbyDataChannel>.Instance.PostAction(m_songData.GetSongCommand(), null))
				{
					yield return null;
				}
			}
		}

		protected void OnGamePause(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			StopCoroutine("ReSendSongTone");
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
		}

		protected void OnGameUnPause(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Sing_A_Long);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			if (m_currentState != PlayerState.NotPlayingSong)
			{
				StartCoroutine(ReSendSongTone());
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((MusicGameEvent)(object)enumValue)
			{
			case MusicGameEvent.SongSelected:
				Singleton<MusicGameFabricQuery>.Instance.SetFilter(false, m_filterFreq);
				if (paramList.Length != 0)
				{
					MusicGameSongData songData = (MusicGameSongData)paramList[0];
					StartCoroutine(StartSongCoroutine(songData));
				}
				else
				{
					Logging.LogError("Song data not sent along with select message");
				}
				break;
			case MusicGameEvent.NoteMissed:
				if (m_currentState == PlayerState.PlayingSongStandard && !m_discoModeAdvertised)
				{
					Singleton<MusicGameFabricQuery>.Instance.SetFilter(true, m_filterFreq);
				}
				break;
			case MusicGameEvent.DiscoModeAvailable:
				AdvertiseDiscoMode(true);
				break;
			case MusicGameEvent.DiscoModeOfferExpired:
				AdvertiseDiscoMode(false);
				break;
			case MusicGameEvent.NoteHit:
				Singleton<MusicGameFabricQuery>.Instance.SetFilter(false, m_filterFreq);
				if (m_currentState == PlayerState.DiscoMode)
				{
					if (m_songData != null)
					{
						GameEventRouter.SendEvent(m_songData.GetSpecialModeNoteHitEvent());
					}
				}
				else
				{
					m_discoMeter.animation.Play();
				}
				break;
			case MusicGameEvent.DiscoModeBegin:
				Singleton<MusicGameFabricQuery>.Instance.SetFilter(false, m_filterFreq);
				m_currentState = PlayerState.DiscoMode;
				m_discoModeAdvertised = false;
				break;
			case MusicGameEvent.DiscoModeEnd:
				if (m_currentState == PlayerState.DiscoMode)
				{
					m_currentState = PlayerState.PlayingSongStandard;
					m_discoModeAdvertised = false;
				}
				break;
			case MusicGameEvent.SongFinished:
				m_currentState = PlayerState.NotPlayingSong;
				m_discoModeAdvertised = false;
				break;
			}
		}

		private void AdvertiseDiscoMode(bool advertise)
		{
			m_discoModeAdvertised = advertise;
			if (advertise)
			{
				SongRailPlayer[] rails = m_rails;
				foreach (SongRailPlayer songRailPlayer in rails)
				{
					songRailPlayer.ClearActiveNotes();
				}
			}
		}

		public MusicGameSongData GetCurrentSong()
		{
			return m_songData;
		}

		public int GetSampleRate()
		{
			if (Singleton<MusicGameFabricQuery>.Exists)
			{
				MusicGameFabricQuery instance = Singleton<MusicGameFabricQuery>.Instance;
				return instance.GetSampleRate();
			}
			return 44100;
		}
	}
}
