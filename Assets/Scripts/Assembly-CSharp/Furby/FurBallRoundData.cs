using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class FurBallRoundData : MonoBehaviour
	{
		public Transform StartPositions;

		public Transform TargetPositions;

		[SerializeField]
		public List<FurBallRound> Rounds;

		[SerializeField]
		private int m_numberOfFurbyModeTurns = 5;

		[SerializeField]
		private int m_scorePerDefendFurblingMode = 10;

		[SerializeField]
		private int m_scorePerGoalFurbyMode = 10;

		[SerializeField]
		private int m_goodFurbyModeScore = 4;

		[SerializeField]
		private int m_okFurbyModeScore = 2;

		[SerializeField]
		private int m_goodFurblingModeScore = 15;

		[SerializeField]
		private int m_okFurblingModeScore = 5;

		private int m_currRound;

		private int m_currBall;

		public int GetTotalNumberOfRounds
		{
			get
			{
				return Rounds.Count;
			}
		}

		public float GetCurrentBallSpeed
		{
			get
			{
				int currentBall = CurrentBall;
				int currentRound = CurrentRound;
				return Rounds[currentRound].Balls[currentBall].Speed;
			}
		}

		public float GetCurrentBallDelay
		{
			get
			{
				int currentBall = CurrentBall;
				int currentRound = CurrentRound;
				return Rounds[currentRound].Balls[currentBall].Delay;
			}
		}

		public int GetBallPositionIndex
		{
			get
			{
				int currentBall = CurrentBall;
				int currentRound = CurrentRound;
				return Rounds[currentRound].Balls[currentBall].GetRandomStartPosition;
			}
		}

		public int GetBallTargetIndex
		{
			get
			{
				return Rounds[CurrentRound].Balls[CurrentBall].GetRandomTargetPosition;
			}
		}

		public int CurrentRound
		{
			get
			{
				return m_currRound;
			}
			set
			{
				m_currRound = value;
			}
		}

		public int CurrentBall
		{
			get
			{
				return m_currBall;
			}
			set
			{
				m_currBall = value;
			}
		}

		public int NumberOfFurbyModeTurns
		{
			get
			{
				return m_numberOfFurbyModeTurns;
			}
		}

		public int GetNumberOfBallsThisRound()
		{
			int currentRound = CurrentRound;
			return UnityEngine.Random.Range(Rounds[currentRound].MinNumberOfBalls, Rounds[currentRound].MaxNumberOfBalls);
		}

		public bool IsGoodScore(bool furblingMode, int score)
		{
			if (furblingMode)
			{
				return score >= m_goodFurblingModeScore;
			}
			return score >= m_goodFurbyModeScore;
		}

		public bool IsOkScore(bool furblingMode, int score)
		{
			if (furblingMode)
			{
				return score >= m_okFurblingModeScore;
			}
			return score >= m_okFurbyModeScore;
		}

		public int GetScore(bool furblingMode)
		{
			if (furblingMode)
			{
				return m_scorePerDefendFurblingMode;
			}
			return m_scorePerGoalFurbyMode;
		}
	}
}
