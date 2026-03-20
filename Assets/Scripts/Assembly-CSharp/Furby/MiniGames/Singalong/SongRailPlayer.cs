using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class SongRailPlayer : GameEventReceiver
	{
		private int m_currentNote;

		private MusicGameSongData.RailInfo m_railInfo;

		[SerializeField]
		private float m_animationDuration;

		[SerializeField]
		private GameObject m_notePrefab;

		[SerializeField]
		private int m_railPlacementIndex;

		private List<NotesMover> m_activeNotes = new List<NotesMover>();

		private float m_lastSeenTime;

		private MusicGameSongPlayer.MusicGameSettings m_settings;

		private int m_ConsecutiveNotesMissed;

		private bool m_InDiscoMode;

		public int m_NoteMissThreshold = 5;

		public override Type EventType
		{
			get
			{
				return typeof(MusicGameEvent);
			}
		}

		public void Setup(MusicGameSongData.RailInfo railInfo, MusicGameSongPlayer.MusicGameSettings settings)
		{
			Reset();
			m_railInfo = railInfo;
			m_currentNote = 0;
			m_settings = settings;
		}

		public void Reset()
		{
			foreach (NotesMover activeNote in m_activeNotes)
			{
				if (activeNote != null)
				{
					UnityEngine.Object.Destroy(activeNote.gameObject);
				}
			}
			m_activeNotes.Clear();
			m_currentNote = 0;
			m_ConsecutiveNotesMissed = 0;
			m_InDiscoMode = false;
		}

		public void ClearActiveNotes()
		{
			foreach (NotesMover activeNote in m_activeNotes)
			{
				if (activeNote != null)
				{
					UnityEngine.Object.Destroy(activeNote.gameObject);
				}
			}
			m_activeNotes.Clear();
			m_currentNote = 0;
			m_ConsecutiveNotesMissed = 0;
		}

		public bool IsEmpty()
		{
			return m_activeNotes.Count == 0;
		}

		public void UpdateWithSongTime(float songTime, bool shouldSpawnNotes)
		{
			m_lastSeenTime = songTime;
			if (shouldSpawnNotes)
			{
				MusicGameSongData.NoteInfo note = m_railInfo.GetNote(m_currentNote);
				while (note != null && note.GetTimeAfterStart() < songTime + m_animationDuration)
				{
					if (note.GetTimeAfterStart() > songTime + m_animationDuration - 0.2f)
					{
						GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_notePrefab);
						gameObject.transform.parent = Singleton<NotePlacement>.Instance.GetRoot(m_railPlacementIndex);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localRotation = Quaternion.identity;
						gameObject.transform.localScale = m_notePrefab.transform.localScale;
						NotesMover component = gameObject.GetComponent<NotesMover>();
						if (component != null)
						{
							component.Setup(note, songTime);
							m_activeNotes.Add(component);
						}
					}
					m_currentNote++;
					note = m_railInfo.GetNote(m_currentNote);
				}
			}
			foreach (NotesMover activeNote in m_activeNotes)
			{
				if (activeNote != null)
				{
					activeNote.UpdateNote(songTime, m_settings.m_hitWindow);
				}
			}
			while (m_activeNotes.Contains(null))
			{
				m_activeNotes.Remove(null);
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((MusicGameEvent)(object)enumValue)
			{
			case MusicGameEvent.NoteMissed:
				m_ConsecutiveNotesMissed++;
				if (!m_InDiscoMode && m_ConsecutiveNotesMissed >= m_NoteMissThreshold)
				{
					GameEventRouter.SendEvent(HintEvents.Singalong_SuggestPressingANote_START);
				}
				break;
			case MusicGameEvent.NotePressed:
				m_ConsecutiveNotesMissed = 0;
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestPressingANote_END);
				if (paramList.Length > 0)
				{
					int num = (int)paramList[0];
					if (num == m_railPlacementIndex)
					{
						CheckNotesForHit();
					}
				}
				break;
			case MusicGameEvent.DiscoModeBegin:
				m_InDiscoMode = true;
				foreach (NotesMover activeNote in m_activeNotes)
				{
					if (activeNote != null)
					{
						UnityEngine.Object.Destroy(activeNote.gameObject);
					}
				}
				m_activeNotes.Clear();
				break;
			case MusicGameEvent.DiscoModeEnd:
			case MusicGameEvent.SongFinished:
				m_InDiscoMode = false;
				GameEventRouter.SendEvent(HintEvents.Singalong_SuggestPressingANote_END);
				break;
			case MusicGameEvent.NoteHit:
				break;
			}
		}

		private void CheckNotesForHit()
		{
			bool flag = false;
			float num = 0f;
			foreach (NotesMover activeNote in m_activeNotes)
			{
				if (activeNote != null)
				{
					num = activeNote.GetNote().GetTimeAfterStart() - m_lastSeenTime;
					float num2 = Mathf.Abs(num);
					if (num2 < m_settings.m_hitWindow)
					{
						flag = true;
						UnityEngine.Object.Destroy(activeNote.gameObject);
						break;
					}
				}
			}
			if (flag)
			{
				GameEventRouter.SendEvent(MusicGameEvent.NoteHit, null, num);
				return;
			}
			GameEventRouter.SendEvent(MusicGameEvent.NoteMissed, null, num);
			GameEventRouter.SendEvent(MusicGameEvent.NoteMissedTapped, null, num);
		}
	}
}
