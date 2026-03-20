using HutongGames.PlayMaker;
using Relentless;

namespace Furby.MiniGames.HideAndSeek
{
	public class HideAndSeekState : Singleton<HideAndSeekState>
	{
		public int m_TotalLevels;

		public HideAndSeekData[] m_HideAndSeekData;

		public UILabel m_TriesLabel;

		private int m_TriesLeft;

		private int m_CurrentTry;

		private int m_CurrentLevel;

		private PlayMakerFSM m_GameStateMachine;

		private FsmEventTarget m_EventTarget = new FsmEventTarget();

		public int TriesLeft
		{
			get
			{
				return m_TriesLeft;
			}
			set
			{
				m_TriesLeft = value;
			}
		}

		public int CurrentTry
		{
			get
			{
				return m_CurrentTry;
			}
		}

		public int TotalLevels
		{
			get
			{
				return m_TotalLevels;
			}
		}

		private void Start()
		{
			m_GameStateMachine = GetComponent<PlayMakerFSM>();
			m_EventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			m_CurrentLevel = Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel % m_TotalLevels;
			m_TriesLeft = m_HideAndSeekData[m_CurrentLevel].m_TotalTries;
			m_TriesLabel.text = TriesLeft.ToString();
			m_CurrentTry = 0;
		}

		public void IncrementTries(int triesToIncrement)
		{
			TriesLeft += triesToIncrement;
			if (TriesLeft <= 0)
			{
				TriesLeft = 0;
				m_TriesLabel.text = TriesLeft.ToString();
				Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = 0;
				Singleton<GameDataStoreObject>.Instance.Save();
				m_GameStateMachine.Fsm.Event(m_EventTarget, "TriesFinished");
			}
			m_TriesLabel.text = TriesLeft.ToString();
		}

		private void FurbyFound()
		{
			Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = m_CurrentLevel + 1;
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		private void HandleFinalFound()
		{
			if (m_CurrentLevel == m_TotalLevels - 1)
			{
				Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = 0;
				Singleton<GameDataStoreObject>.Instance.Save();
				m_GameStateMachine.Fsm.Event(m_EventTarget, "FinalFound");
			}
		}

		public void HandleTurns()
		{
			m_CurrentTry++;
			TriesLeft--;
			m_TriesLabel.text = TriesLeft.ToString();
			if (TriesLeft <= 0)
			{
				Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = 0;
				Singleton<GameDataStoreObject>.Instance.Save();
				m_GameStateMachine.Fsm.Event(m_EventTarget, "TriesFinished");
			}
		}
	}
}
