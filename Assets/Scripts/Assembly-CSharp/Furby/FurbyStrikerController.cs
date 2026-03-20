using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyStrikerController : GameEventReceiver
	{
		private enum StrikerMode
		{
			StrikerOpponent = 0,
			PlayingAsStriker = 1,
			NotPlaying = 2
		}

		[Serializable]
		public class AimedShot
		{
			[SerializeField]
			public Transform m_startPosition;

			[SerializeField]
			public Transform m_endPosition;

			[SerializeField]
			public float m_kickStrength;
		}

		private StrikerMode m_strikerMode = StrikerMode.NotPlaying;

		public FurBallRoundData RoundData;

		public Transform StartPositions;

		public Transform TargetPositions;

		private List<FurballController> m_furballs;

		private List<Transform> m_sortedStartTransforms;

		private List<Transform> m_sortedTargetTransforms;

		private Queue<FurballController> m_queuedBalls;

		private bool m_isRoundInProgress;

		[SerializeField]
		private AimedShot[] m_aimedShots;

		private int m_aimedShotIndex;

		private int m_numShotsDeflected;

		private int m_goalsScored;

		public override Type EventType
		{
			get
			{
				return typeof(FurBallGameEvent);
			}
		}

		private void Start()
		{
			m_furballs = new List<FurballController>(GetComponentsInChildren<FurballController>());
			m_queuedBalls = new Queue<FurballController>(m_furballs);
			foreach (FurballController furball in m_furballs)
			{
				furball.gameObject.SetActive(false);
			}
			if (StartPositions.childCount > m_furballs.Count)
			{
				Logging.LogWarning("Number of balls available (" + m_furballs.Count + ") is less than the number of StartPositions available (" + StartPositions.childCount + ")");
			}
			m_sortedStartTransforms = new List<Transform>();
			foreach (Transform startPosition in StartPositions)
			{
				m_sortedStartTransforms.Add(startPosition);
			}
			m_sortedStartTransforms.Sort((Transform t1, Transform t2) => t1.name.CompareTo(t2.name));
			m_sortedTargetTransforms = new List<Transform>();
			foreach (Transform targetPosition in TargetPositions)
			{
				m_sortedTargetTransforms.Add(targetPosition);
			}
			m_sortedTargetTransforms.Sort((Transform t1, Transform t2) => t1.name.CompareTo(t2.name));
		}

		private void KickNextBall()
		{
			int getBallPositionIndex = RoundData.GetBallPositionIndex;
			int getBallTargetIndex = RoundData.GetBallTargetIndex;
			Vector3 position = m_sortedStartTransforms[getBallPositionIndex].position;
			Vector3 position2 = m_sortedTargetTransforms[getBallTargetIndex].position;
			FurballController furballController = m_queuedBalls.Dequeue();
			furballController.KickBall(position, position2, RoundData.GetCurrentBallSpeed, this);
		}

		public int NumberOfBallsActive()
		{
			return m_furballs.Count - m_queuedBalls.Count;
		}

		public void ReturnBallToPool(FurballController ball)
		{
			if (!m_queuedBalls.Contains(ball))
			{
				m_queuedBalls.Enqueue(ball);
			}
		}

		private IEnumerator ShootPlayerBall()
		{
			m_goalsScored = 0;
			if (m_aimedShots.Length > 0)
			{
				int aimedShotIndex = Mathf.Min(m_aimedShotIndex, m_aimedShots.Length - 1);
				Vector3 startPos = m_aimedShots[aimedShotIndex].m_startPosition.position;
				Vector3 targetPos = m_aimedShots[aimedShotIndex].m_endPosition.position;
				float speed = m_aimedShots[aimedShotIndex].m_kickStrength;
				FurballController nextBall = m_queuedBalls.Dequeue();
				nextBall.KickBall(startPos, targetPos, speed, this);
				m_aimedShotIndex++;
			}
			while (NumberOfBallsActive() > 0)
			{
				yield return null;
			}
			if (m_goalsScored != 0)
			{
				GameEventRouter.SendEvent(FurBallGameEvent.FurballFurbyModeGoodRound);
			}
			else
			{
				GameEventRouter.SendEvent(FurBallGameEvent.FurballFurbyModeBadRound);
			}
			yield return new WaitForSeconds(3.5f);
			m_isRoundInProgress = false;
			GameEventRouter.SendEvent(FurBallGameEvent.FurballBallShotsEnded);
		}

		private IEnumerator ShootCurrentRoundOfBalls()
		{
			int ballsToFire = RoundData.GetNumberOfBallsThisRound();
			m_numShotsDeflected = 0;
			yield return new WaitForSeconds(2f);
			for (int ballIndex = 0; ballIndex < ballsToFire; ballIndex++)
			{
				if (ballIndex != 0)
				{
					float delay = RoundData.GetCurrentBallDelay;
					yield return new WaitForSeconds(delay);
				}
				RoundData.CurrentBall = ballIndex;
				KickNextBall();
			}
			while (NumberOfBallsActive() > 0)
			{
				yield return null;
			}
			yield return null;
			if ((float)m_numShotsDeflected >= (float)ballsToFire / 2f)
			{
				GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeGoodRound);
			}
			else
			{
				GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingModeBadRound);
			}
			m_isRoundInProgress = false;
			GameEventRouter.SendEvent(FurBallGameEvent.FurballBallShotsEnded);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((FurBallGameEvent)(object)enumValue)
			{
			case FurBallGameEvent.FurballStartPlayingFurblingMode:
				m_strikerMode = StrikerMode.StrikerOpponent;
				break;
			case FurBallGameEvent.FurballStartPlayingFurbyMode:
				m_aimedShotIndex = 0;
				m_strikerMode = StrikerMode.PlayingAsStriker;
				break;
			case FurBallGameEvent.FurballFurblingModeActivateStartRound:
				if (m_strikerMode == StrikerMode.StrikerOpponent)
				{
					Logging.Log("Shooting ball");
					ShootBall();
				}
				break;
			case FurBallGameEvent.FurballStartBallRound:
				if (m_strikerMode == StrikerMode.PlayingAsStriker)
				{
					ShootBall();
				}
				break;
			case FurBallGameEvent.FurballGameFinished:
				m_strikerMode = StrikerMode.NotPlaying;
				break;
			case FurBallGameEvent.FurballBallDeflected:
				m_numShotsDeflected++;
				break;
			case FurBallGameEvent.FurballGoalScored:
				m_goalsScored++;
				break;
			}
		}

		public bool AreAnyBallsReturning()
		{
			foreach (FurballController furball in m_furballs)
			{
				if (furball.gameObject.activeInHierarchy && furball.rigidbody.velocity.z > 0f)
				{
					return true;
				}
			}
			return false;
		}

		private void ShootBall()
		{
			switch (m_strikerMode)
			{
			case StrikerMode.StrikerOpponent:
				if (!m_isRoundInProgress)
				{
					m_isRoundInProgress = true;
					StartCoroutine(ShootCurrentRoundOfBalls());
				}
				break;
			case StrikerMode.PlayingAsStriker:
				if (!m_isRoundInProgress)
				{
					m_isRoundInProgress = true;
					StartCoroutine(ShootPlayerBall());
				}
				break;
			}
		}
	}
}
