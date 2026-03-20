using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurBallCurrentGame : GameEventReceiver
	{
		public enum FurBallModeType
		{
			PlayAsFurby = 0,
			PlayAsFurbling = 1
		}

		private class FurbyModeCurrentInfo
		{
			private int m_score;

			private int m_turnsRemaining;

			private HintState m_Waiting = new HintState();

			private GameEventSubscription m_CancelHint;

			public void ResetGame(FurBallRoundData gameData)
			{
				m_score = 0;
				m_turnsRemaining = gameData.NumberOfFurbyModeTurns;
				m_Waiting.m_Interval = 6f;
				m_Waiting.m_PayloadEvent = HintEvents.Furball_YouNeedToDispatchBall_Attacking;
				if (m_CancelHint == null)
				{
					m_CancelHint = new GameEventSubscription(typeof(HintEvents), OnEvent);
				}
				StartTurn();
			}

			public void Score(int amount)
			{
				m_score += amount;
			}

			public void NextTurn()
			{
				m_turnsRemaining--;
				StartTurn();
			}

			private void StartTurn()
			{
				if (m_turnsRemaining > 0)
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballReadyToShootBall);
					m_Waiting.Enable();
					return;
				}
				GameEventRouter.SendEvent(BabyEndMinigameEvent.SetScore, null, GetScore());
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowDialog);
				GameEventRouter.SendEvent(FurBallGameEvent.FurballGameFinished);
				GameEventRouter.SendEvent(HintEvents.DeactivateAll);
				m_Waiting.Disable();
			}

			public void Update()
			{
				m_Waiting.TestAndBroadcastState();
			}

			public int GetScore()
			{
				return m_score;
			}

			public int GetTurnsRemaining()
			{
				return m_turnsRemaining;
			}

			private void OnEvent(Enum eventType, GameObject obj, params object[] prms)
			{
				if (eventType.Equals(HintEvents.Furball_DispatchedBall))
				{
					m_Waiting.Disable();
				}
			}
		}

		private class FurblingModeCurrentInfo
		{
			private int m_score;

			private int m_roundIndex;

			public void ResetGame(FurBallRoundData gameData)
			{
				m_score = 0;
				m_roundIndex = -1;
			}

			public void Score(int amount)
			{
				m_score += amount;
			}

			public void NextRound(FurBallRoundData gameData)
			{
				m_roundIndex++;
				StartRound(gameData);
			}

			private void StartRound(FurBallRoundData gameData)
			{
				if (m_roundIndex == gameData.GetTotalNumberOfRounds)
				{
					GameEventRouter.SendEvent(BabyEndMinigameEvent.SetScore, null, GetScore());
					GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowDialog);
					GameEventRouter.SendEvent(FurBallGameEvent.FurballGameFinished);
				}
				else
				{
					gameData.CurrentRound = m_roundIndex;
					GameEventRouter.SendEvent(FurBallGameEvent.FurballReadyToShootBall);
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeStartRoundDelay);
				}
			}

			public void Update()
			{
			}

			public int GetScore()
			{
				return m_score;
			}

			public int GetRoundIndex()
			{
				return m_roundIndex;
			}
		}

		private FurBallModeType m_currentModeType;

		private FurbyModeCurrentInfo m_furbyModeCurrentInfo = new FurbyModeCurrentInfo();

		private FurblingModeCurrentInfo m_furblingModeCurrentInfo = new FurblingModeCurrentInfo();

		private FurBallRoundData m_gameData;

		private float m_MaintenanceTimer;

		private float m_SensorTimer;

		private FurbyCommand m_MaintenanceTone = FurbyCommand.Furball;

		public override Type EventType
		{
			get
			{
				return typeof(FurBallGameEvent);
			}
		}

		public void Update()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				m_furblingModeCurrentInfo.Update();
				break;
			case FurBallModeType.PlayAsFurby:
				m_furbyModeCurrentInfo.Update();
				break;
			}
		}

		private void Start()
		{
			m_gameData = (FurBallRoundData)UnityEngine.Object.FindObjectOfType(typeof(FurBallRoundData));
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			Singleton<FurbyDataChannel>.Instance.ToneEvent += ComAirEvent;
			GameEventRouter.SendEvent(FurBallGameEvent.FurballEnter);
		}

		private void OnDestroy()
		{
			if (Singleton<FurbyDataChannel>.Instance != null)
			{
				Singleton<FurbyDataChannel>.Instance.ToneEvent -= ComAirEvent;
				Singleton<FurbyDataChannel>.Instance.AutoConnect = true;
				Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
				Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			}
		}

		private IEnumerator MaintenanceCoroutine()
		{
			while (true)
			{
				if (Time.fixedTime > m_SensorTimer)
				{
					m_MaintenanceTone = FurbyCommand.Furball;
				}
				yield return this.CommandAndWaitOnSend(m_MaintenanceTone, CommandComplete);
				m_MaintenanceTimer = Time.fixedTime + 14f;
				while (Time.fixedTime < m_MaintenanceTimer)
				{
					yield return null;
				}
			}
		}

		public void CommandComplete(bool acknowledged)
		{
			if (acknowledged)
			{
				m_MaintenanceTone = FurbyCommand.Maintenance;
				m_SensorTimer += 42f;
			}
		}

		public void ComAirEvent(FurbyMessageType type, long tone)
		{
			if (!IsFurballTone(type, tone))
			{
				m_MaintenanceTimer = Time.fixedTime;
				m_MaintenanceTone = FurbyCommand.Furball;
			}
		}

		private static bool IsFurballTone(FurbyMessageType type, long tone)
		{
			switch (type)
			{
			case FurbyMessageType.Personality:
				return true;
			case FurbyMessageType.Sensor:
				switch ((FurbySensor)tone)
				{
				case FurbySensor.AngledSpit:
					return true;
				case FurbySensor.StraightSpit:
					return true;
				case FurbySensor.SuperSpit:
					return true;
				default:
					return false;
				}
			default:
				return false;
			}
		}

		private IEnumerator ActivateStartRound()
		{
			yield return new WaitForSeconds(5f);
			GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeActivateStartRound);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((FurBallGameEvent)(object)enumValue)
			{
			case FurBallGameEvent.FurballSelectFurblingGameMode:
				Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
				Singleton<FurbyDataChannel>.Instance.AutoConnect = true;
				StopCoroutine("MaintenanceCoroutine");
				m_currentModeType = FurBallModeType.PlayAsFurbling;
				Invoke(ResetGame, 0.1f);
				break;
			case FurBallGameEvent.FurballSelectFurbyGameMode:
				Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(false);
				Singleton<FurbyDataChannel>.Instance.AutoConnect = false;
				m_MaintenanceTone = FurbyCommand.Furball;
				StartCoroutine("MaintenanceCoroutine");
				m_currentModeType = FurBallModeType.PlayAsFurby;
				Invoke(ResetGame, 0.1f);
				break;
			case FurBallGameEvent.FurballStartPlayingFurblingMode:
				NextRound();
				break;
			case FurBallGameEvent.FurballBallShotsEnded:
				NextRound();
				break;
			case FurBallGameEvent.FurballBallDeflected:
				BallDeflected();
				break;
			case FurBallGameEvent.FurballGoalScored:
				GoalScored();
				break;
			case FurBallGameEvent.FurballStartBallRound:
				if (m_currentModeType == FurBallModeType.PlayAsFurby)
				{
					BallShot();
				}
				break;
			case FurBallGameEvent.FurballFurblingModeActivateStartRound:
				if (m_currentModeType == FurBallModeType.PlayAsFurbling)
				{
					BallShot();
				}
				break;
			case FurBallGameEvent.FurballFurblingModeStartRoundDelay:
				StartCoroutine(ActivateStartRound());
				break;
			case FurBallGameEvent.FurballGameFinished:
				Invoke(EvaluateScore, 4f);
				break;
			}
		}

		private void BallShot()
		{
			StopCoroutine("MaintenanceCoroutine");
			m_SensorTimer = Time.fixedTime + 42f;
			m_MaintenanceTone = FurbyCommand.Maintenance;
			StartCoroutine("MaintenanceCoroutine");
		}

		private void EvaluateScore()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				if (m_gameData.IsGoodScore(true, m_furblingModeCurrentInfo.GetScore()))
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeGoodResult);
				}
				else if (m_gameData.IsOkScore(true, m_furblingModeCurrentInfo.GetScore()))
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeOKResult);
				}
				else
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeBadResult);
				}
				break;
			case FurBallModeType.PlayAsFurby:
				if (m_gameData.IsGoodScore(false, m_furbyModeCurrentInfo.GetScore()))
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeGoodResult);
				}
				else if (m_gameData.IsOkScore(false, m_furbyModeCurrentInfo.GetScore()))
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeOKResult);
				}
				else
				{
					GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeBadResult);
				}
				break;
			}
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		private void NextRound()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				m_furblingModeCurrentInfo.NextRound(m_gameData);
				break;
			case FurBallModeType.PlayAsFurby:
				m_furbyModeCurrentInfo.NextTurn();
				break;
			}
		}

		private void BallDeflected()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				GameEventRouter.SendEvent(FurBallGameEvent.FurballOpponentMissesGoal);
				m_furblingModeCurrentInfo.Score(m_gameData.GetScore(true));
				break;
			case FurBallModeType.PlayAsFurby:
				GameEventRouter.SendEvent(FurBallGameEvent.FurballPlayerMissesGoal);
				break;
			}
		}

		private void GoalScored()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				GameEventRouter.SendEvent(FurBallGameEvent.FurballOpponentScoresGoal);
				break;
			case FurBallModeType.PlayAsFurby:
				GameEventRouter.SendEvent(FurBallGameEvent.FurballPlayerScoresGoal);
				m_furbyModeCurrentInfo.Score(m_gameData.GetScore(false));
				break;
			}
		}

		private void ResetGame()
		{
			CancelInvoke(EvaluateScore);
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				m_furblingModeCurrentInfo.ResetGame(m_gameData);
				break;
			case FurBallModeType.PlayAsFurby:
				m_furbyModeCurrentInfo.ResetGame(m_gameData);
				break;
			}
		}

		public int GetScore()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				return m_furblingModeCurrentInfo.GetScore();
			case FurBallModeType.PlayAsFurby:
				return m_furbyModeCurrentInfo.GetScore();
			default:
				return 0;
			}
		}

		public int GetTurnsRemaining()
		{
			switch (m_currentModeType)
			{
			case FurBallModeType.PlayAsFurbling:
				return Mathf.Min(m_gameData.GetTotalNumberOfRounds - m_furblingModeCurrentInfo.GetRoundIndex(), m_gameData.GetTotalNumberOfRounds);
			case FurBallModeType.PlayAsFurby:
				return m_furbyModeCurrentInfo.GetTurnsRemaining();
			default:
				return 0;
			}
		}
	}
}
