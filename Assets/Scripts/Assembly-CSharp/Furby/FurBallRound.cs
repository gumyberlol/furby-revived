using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class FurBallRound : ScriptableObject
	{
		public int RoundNumber;

		public int MinNumberOfBalls;

		public bool ShowRoundInInspector = true;

		public List<FurBallFootBall> Balls;

		private int m_nStartPositions;

		private int m_nTargetPositions;

		public int MaxNumberOfBalls
		{
			get
			{
				return Balls.Count;
			}
			set
			{
				if (Balls == null)
				{
					Balls = new List<FurBallFootBall>();
				}
				int num = value - Balls.Count;
				for (int i = 0; i < Mathf.Abs(num); i++)
				{
					if (num > 0)
					{
						Balls.Add(NewBall());
					}
					else
					{
						Balls.RemoveAt(Balls.Count - 1);
					}
				}
			}
		}

		public int NumberOfStartPositions
		{
			get
			{
				return m_nStartPositions;
			}
			set
			{
				m_nStartPositions = value;
				foreach (FurBallFootBall ball in Balls)
				{
					bool[] array = new bool[m_nStartPositions];
					for (int i = 0; i < m_nStartPositions; i++)
					{
						if (i < ball.BallStartPositions.Length)
						{
							array[i] = ball.BallStartPositions[i];
						}
					}
					ball.BallStartPositions = array;
				}
			}
		}

		public int NumberOfTargetPositions
		{
			get
			{
				return m_nTargetPositions;
			}
			set
			{
				m_nTargetPositions = value;
				foreach (FurBallFootBall ball in Balls)
				{
					bool[] array = new bool[m_nTargetPositions];
					for (int i = 0; i < m_nTargetPositions; i++)
					{
						if (i < ball.BallTargetPositions.Length)
						{
							array[i] = ball.BallTargetPositions[i];
						}
					}
					ball.BallTargetPositions = array;
				}
			}
		}

		private FurBallFootBall NewBall()
		{
			FurBallFootBall furBallFootBall = new FurBallFootBall();
			furBallFootBall.BallStartPositions = new bool[m_nStartPositions];
			furBallFootBall.BallTargetPositions = new bool[m_nTargetPositions];
			return furBallFootBall;
		}
	}
}
