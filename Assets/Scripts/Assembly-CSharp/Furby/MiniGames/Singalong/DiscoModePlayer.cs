using System;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class DiscoModePlayer : GameEventReceiver
	{
		[SerializeField]
		private MusicGameScoring m_scoring;

		[SerializeField]
		private float m_discoDuration = 8f;

		[SerializeField]
		private SongRailPlayer[] m_songRails;

		[SerializeField]
		private DiscoRailAnimations[] m_discoRails;

		private bool m_discoModeIsActive;

		private float m_discoTimeRemaining;

		[SerializeField]
		private GameObject m_vfx_pointsAdded;

		public override Type EventType
		{
			get
			{
				return typeof(MusicGameEvent);
			}
		}

		public int Score
		{
			get
			{
				return m_scoring.GetScore();
			}
		}

		private void Update()
		{
			if (m_discoModeIsActive)
			{
				m_discoTimeRemaining -= Time.deltaTime;
				if (m_discoTimeRemaining < 0f)
				{
					GameEventRouter.SendEvent(MusicGameEvent.DiscoModeEnd);
				}
			}
		}

		public bool IsDiscoModeActive()
		{
			return m_discoModeIsActive;
		}

		public float GetDiscoTimeRemainingT()
		{
			return m_discoTimeRemaining / m_discoDuration;
		}

		public bool ShouldNotesContinue()
		{
			if (m_discoModeIsActive)
			{
				if (m_discoRails.Length > 0)
				{
					float discoModeOutLength = m_discoRails[0].GetDiscoModeOutLength();
					return m_discoTimeRemaining < discoModeOutLength - 0.75f;
				}
				return true;
			}
			return true;
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((MusicGameEvent)(object)enumValue)
			{
			case MusicGameEvent.RequestDiscoMode:
				RequestDisco();
				break;
			case MusicGameEvent.DiscoModeBegin:
				m_discoTimeRemaining = m_discoDuration;
				ActivateDiscoMode();
				break;
			case MusicGameEvent.DiscoModeEnd:
				DeactivateDiscoMode();
				break;
			case MusicGameEvent.SongFinished:
				DeactivateDiscoMode();
				break;
			case MusicGameEvent.NotePressed:
				if (m_discoModeIsActive)
				{
					GameEventRouter.SendEvent(MusicGameEvent.NoteHit);
				}
				break;
			}
		}

		private void ActivateDiscoMode()
		{
			m_discoModeIsActive = true;
			DiscoRailAnimations[] discoRails = m_discoRails;
			foreach (DiscoRailAnimations discoRailAnimations in discoRails)
			{
				discoRailAnimations.SetDiscoModeStarting(m_discoDuration);
			}
			m_vfx_pointsAdded.SetActive(false);
		}

		private void DeactivateDiscoMode()
		{
			m_discoModeIsActive = false;
			DiscoRailAnimations[] discoRails = m_discoRails;
			foreach (DiscoRailAnimations discoRailAnimations in discoRails)
			{
				discoRailAnimations.SetDiscoModeOff();
			}
			m_vfx_pointsAdded.SetActive(true);
		}

		private void RequestDisco()
		{
			if (!m_discoModeIsActive && m_scoring != null && m_scoring.GetBarT() == 1f)
			{
				GameEventRouter.SendEvent(MusicGameEvent.DiscoModeBegin);
			}
		}
	}
}
