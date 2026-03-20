using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class MusicGameSongData : ScriptableObject
	{
		[Serializable]
		public class NoteInfo
		{
			[SerializeField]
			private float m_timeAfterSongStart;

			private float m_quantisedTime;

			private bool m_isQuantised;

			public NoteInfo(float time)
			{
				m_timeAfterSongStart = time;
			}

			public void Quantise(float bpm, bool quantiseEighths, int beatPush)
			{
				float num = 60f / bpm;
				float num2 = 60f / bpm;
				if (quantiseEighths)
				{
					num2 *= 0.5f;
				}
				m_quantisedTime = Mathf.Round(m_timeAfterSongStart / num2) * num2 + (float)beatPush * num;
				m_isQuantised = true;
			}

			public float GetTimeAfterStart()
			{
				if (m_isQuantised)
				{
					return m_quantisedTime;
				}
				return m_timeAfterSongStart;
			}

			public float GetRawTime()
			{
				return m_timeAfterSongStart;
			}
		}

		[Serializable]
		public class RailInfo
		{
			[SerializeField]
			private List<NoteInfo> m_noteInfo = new List<NoteInfo>();

			private int m_numNotes = -1;

			public void Quantise(float bpm, bool quantiseEighths, int beatPush)
			{
				foreach (NoteInfo item in m_noteInfo)
				{
					item.Quantise(bpm, quantiseEighths, beatPush);
				}
			}

			public void SetNotes(float[] noteList)
			{
				m_noteInfo = new List<NoteInfo>();
				foreach (float time in noteList)
				{
					m_noteInfo.Add(new NoteInfo(time));
				}
			}

			public int CountNotes()
			{
				return m_noteInfo.Count;
			}

			public NoteInfo GetNote(int index)
			{
				if (m_numNotes == -1)
				{
					m_numNotes = m_noteInfo.Count;
				}
				if (index < m_numNotes)
				{
					return m_noteInfo[index];
				}
				return null;
			}
		}

		[SerializeField]
		private RailInfo[] m_rails = new RailInfo[4];

		[SerializeField]
		private bool m_quantise;

		[SerializeField]
		private bool m_quantiseEighths;

		[SerializeField]
		private float m_quantiseBpm = 128f;

		[SerializeField]
		private int m_quantiseIntroBeats = 4;

		[SerializeField]
		private SerialisableEnum m_songStartEvent;

		[SerializeField]
		private SerialisableEnum m_specialModeNoteEvent;

		[SerializeField]
		private int m_goodScore;

		[SerializeField]
		private int m_okScore;

		[SerializeField]
		private FurbyAction m_songCommand = FurbyAction.Sing_A_Long_Song0;

		public FurbyAction GetSongCommand()
		{
			return m_songCommand;
		}

		public RailInfo[] GetRails()
		{
			if (m_quantise)
			{
				Quantise(m_quantiseBpm);
			}
			return m_rails;
		}

		private void Quantise(float bpm)
		{
			RailInfo[] rails = m_rails;
			foreach (RailInfo railInfo in rails)
			{
				railInfo.Quantise(bpm, m_quantiseEighths, m_quantiseIntroBeats);
			}
		}

		public float GetBPM()
		{
			return m_quantiseBpm;
		}

		public bool GetEighths()
		{
			return m_quantiseEighths;
		}

		public int GetGoodScore()
		{
			return m_goodScore;
		}

		public int GetOkScore()
		{
			return m_okScore;
		}

		public void SetNotes(int railIndex, float[] notes)
		{
			m_rails[railIndex].SetNotes(notes);
		}

		public Enum GetStartEvent()
		{
			return m_songStartEvent.Value;
		}

		public Enum GetSpecialModeNoteHitEvent()
		{
			return m_specialModeNoteEvent.Value;
		}
	}
}
