using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	[RequireComponent(typeof(Rigidbody))]
	public class BabyGoalyController : GameEventReceiver
	{
		private enum MovementControlType
		{
			NoMovementAllowed = 0,
			ComputerControlled = 1,
			PlayerControlled = 2
		}

		private enum ComputerControlledMovement
		{
			Diving = 0,
			RunningMove = 1,
			RunningWait = 2
		}

		[Serializable]
		public class GoalyComputerPath
		{
			public Transform[] m_nodes;
		}

		private MovementControlType m_currentMovementSetting;

		private ComputerControlledMovement m_currentComputerMovement = ComputerControlledMovement.RunningMove;

		private float m_computerHoldStillDelay;

		private float m_targetT = 0.5f;

		private bool m_hasDived;

		public float MoveSpeed = 5f;

		public float m_diveSpeed = 10f;

		public GameObject KickEffectPrefab;

		public Camera ActiveCamera;

		public BoxCollider GoalieBounds;

		private Vector3 m_moveDirection;

		[SerializeField]
		private Transform m_leftGoalNode;

		[SerializeField]
		private Transform m_rightGoalNode;

		[SerializeField]
		private Transform m_diveForwardNode;

		private FurballController m_activeBall;

		public float[] m_computerControlledSpeed;

		private int m_currentPathIndex;

		[SerializeField]
		private AnimationClip m_moveLeftAnimation;

		[SerializeField]
		private AnimationClip m_moveRightAnimation;

		[SerializeField]
		private AnimationClip m_diveLeftAnimation;

		[SerializeField]
		private AnimationClip m_diveRightAnimation;

		[SerializeField]
		private AnimationClip m_idleAnimation;

		[SerializeField]
		private bool m_requireInitialTouchForMove;

		[SerializeField]
		private float m_dragRadius = 0.25f;

		[SerializeField]
		private AnimationCurve m_diveCurve = new AnimationCurve();

		[SerializeField]
		private float[] m_chanceOfDiving;

		private Animation m_animation;

		private bool m_isBeingDragged;

		private int m_diveIndex;

		public int m_NumberOfTimesGoalieSavedNoRebound;

		public bool m_CheckToSeeIfThePlayerIsDoingAnything;

		public bool m_HavingCheckedThePlayerHasntSuppliedInput = true;

		public int m_NumberOfTimesOpponentHasStarted;

		public override Type EventType
		{
			get
			{
				return typeof(FurBallGameEvent);
			}
		}

		public void GameFinished()
		{
			m_currentMovementSetting = MovementControlType.NoMovementAllowed;
			base.transform.localPosition = new Vector3(0f, -0.65f, 0f);
		}

		private void Start()
		{
			m_animation = GetComponentInChildren<Animation>();
			m_CheckToSeeIfThePlayerIsDoingAnything = false;
			m_HavingCheckedThePlayerHasntSuppliedInput = true;
			m_NumberOfTimesOpponentHasStarted = 0;
			m_NumberOfTimesGoalieSavedNoRebound = 0;
		}

		private void DoPlayerControlledMovement()
		{
			if (Input.GetMouseButton(0))
			{
				if (!(Input.mousePosition.y < (float)Screen.height * 0.8f))
				{
					return;
				}
				Ray ray = ActiveCamera.ScreenPointToRay(Input.mousePosition);
				float num = 1f / Vector3.Dot(ray.direction, Vector3.up) * (0f - ray.origin.y);
				Vector3 vector = ray.origin + ray.direction * num;
				vector.x = Mathf.Clamp(vector.x, GoalieBounds.bounds.min.x, GoalieBounds.bounds.max.x);
				vector.y = 0f;
				vector.z = Mathf.Clamp(vector.z, GoalieBounds.bounds.min.z, GoalieBounds.bounds.max.z);
				bool flag = m_isBeingDragged;
				if (m_requireInitialTouchForMove)
				{
					if ((vector - base.transform.position).magnitude < m_dragRadius)
					{
						m_isBeingDragged = true;
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					m_moveDirection = Vector3.ClampMagnitude(vector - base.transform.position, 1f) * Time.deltaTime * MoveSpeed;
				}
				m_HavingCheckedThePlayerHasntSuppliedInput = false;
			}
			else
			{
				m_isBeingDragged = false;
			}
		}

		private void FindActiveBall()
		{
			if (m_activeBall == null || !m_activeBall.gameObject.activeInHierarchy)
			{
				m_activeBall = (FurballController)UnityEngine.Object.FindObjectOfType(typeof(FurballController));
			}
		}

		private bool TrySwitchToAnimation(string animationName, string blockIfPlaying)
		{
			if ((!m_animation.isPlaying || m_animation.IsPlaying(m_moveLeftAnimation.name) || m_animation.IsPlaying(m_moveRightAnimation.name) || m_animation.IsPlaying(m_diveLeftAnimation.name) || m_animation.IsPlaying(m_diveRightAnimation.name) || m_animation.IsPlaying(m_idleAnimation.name)) && !m_animation.IsPlaying(animationName) && (blockIfPlaying == null || !m_animation.IsPlaying(blockIfPlaying)))
			{
				m_animation.Play(animationName);
				return true;
			}
			return false;
		}

		private float GetCurrentDiveSpeed()
		{
			if (m_animation.IsPlaying(m_diveLeftAnimation.name))
			{
				return m_diveCurve.Evaluate(m_animation[m_diveLeftAnimation.name].normalizedTime);
			}
			if (m_animation.IsPlaying(m_diveRightAnimation.name))
			{
				return m_diveCurve.Evaluate(m_animation[m_diveRightAnimation.name].normalizedTime);
			}
			return 0.1f;
		}

		private void DoComputerControlledMovement()
		{
			Vector3 vector = base.transform.position;
			int num = m_currentPathIndex % m_computerControlledSpeed.Length;
			float num2 = m_computerControlledSpeed[num];
			bool flag = false;
			switch (m_currentComputerMovement)
			{
			case ComputerControlledMovement.Diving:
				FindActiveBall();
				if (!(m_activeBall != null))
				{
					break;
				}
				vector = new Vector3(m_activeBall.transform.position.x, 0f, m_diveForwardNode.position.z);
				if (Mathf.Abs(m_activeBall.transform.position.x - base.transform.position.x) > 0.1f)
				{
					if (Mathf.Abs(m_activeBall.transform.position.z - base.transform.position.z) < 1.5f)
					{
						flag = true;
						m_hasDived = true;
					}
					else
					{
						num2 = 0f;
					}
				}
				if (m_hasDived)
				{
					num2 = GetCurrentDiveSpeed() * m_computerControlledSpeed[num];
				}
				break;
			case ComputerControlledMovement.RunningMove:
				vector = m_leftGoalNode.position * m_targetT + m_rightGoalNode.position * (1f - m_targetT);
				break;
			case ComputerControlledMovement.RunningWait:
				vector = base.transform.position;
				break;
			}
			Vector3 vector2 = vector - base.transform.position;
			if (vector2.magnitude < num2 * Time.deltaTime && m_currentComputerMovement == ComputerControlledMovement.RunningMove)
			{
				m_targetT = UnityEngine.Random.Range(0f, 1f);
				m_computerHoldStillDelay = UnityEngine.Random.Range(0.3f, 0.6f);
				if (m_currentComputerMovement == ComputerControlledMovement.RunningMove)
				{
					m_currentComputerMovement = ComputerControlledMovement.RunningWait;
				}
			}
			m_computerHoldStillDelay -= Time.deltaTime;
			if (m_currentComputerMovement == ComputerControlledMovement.RunningWait && m_computerHoldStillDelay < 0f)
			{
				m_currentComputerMovement = ComputerControlledMovement.RunningMove;
			}
			m_moveDirection = Vector3.ClampMagnitude(vector2, num2 * Time.deltaTime);
			if (m_moveDirection.x < 0f)
			{
				if (flag)
				{
					TrySwitchToAnimation(m_diveLeftAnimation.name, null);
				}
				else
				{
					TrySwitchToAnimation(m_moveLeftAnimation.name, m_diveLeftAnimation.name);
				}
				if (!m_animation.IsPlaying(m_moveLeftAnimation.name) && !m_animation.IsPlaying(m_diveLeftAnimation.name))
				{
					m_moveDirection = Vector3.zero;
				}
			}
			else if (m_moveDirection.x > 0f)
			{
				if (flag)
				{
					TrySwitchToAnimation(m_diveRightAnimation.name, null);
				}
				else
				{
					TrySwitchToAnimation(m_moveRightAnimation.name, m_diveRightAnimation.name);
				}
				if (!m_animation.IsPlaying(m_moveRightAnimation.name) && !m_animation.IsPlaying(m_diveRightAnimation.name))
				{
					m_moveDirection = Vector3.zero;
				}
			}
			else if (m_currentComputerMovement != ComputerControlledMovement.Diving || !m_hasDived)
			{
				TrySwitchToAnimation(m_idleAnimation.name, null);
			}
		}

		private void Update()
		{
			switch (m_currentMovementSetting)
			{
			case MovementControlType.PlayerControlled:
				DoPlayerControlledMovement();
				break;
			case MovementControlType.ComputerControlled:
				DoComputerControlledMovement();
				break;
			}
			if (m_CheckToSeeIfThePlayerIsDoingAnything && m_NumberOfTimesOpponentHasStarted > 1 && m_HavingCheckedThePlayerHasntSuppliedInput)
			{
				GameEventRouter.SendEvent(HintEvents.Furball_YouNeedToDefendTheGoal);
				m_NumberOfTimesOpponentHasStarted = 0;
			}
			if (m_animation != null && !m_animation.isPlaying)
			{
				m_animation.Play(m_idleAnimation.name);
			}
		}

		private void FixedUpdate()
		{
			base.rigidbody.MovePosition(base.rigidbody.position + m_moveDirection);
			m_moveDirection = Vector3.zero;
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (string.Compare(collisionInfo.collider.tag, "Furball_Football") == 0)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(KickEffectPrefab, collisionInfo.contacts[0].point, Quaternion.identity);
				gameObject.layer = base.gameObject.layer;
				GameEventRouter.SendEvent(FurBallGameEvent.FurballFurblingKicksBall);
				if (m_currentMovementSetting == MovementControlType.ComputerControlled)
				{
					Vector3 force = new Vector3(0f, 0f, 1f) * Mathf.Clamp(collisionInfo.relativeVelocity.magnitude, 0.2f, 0.3f);
					GameEventRouter.SendEvent(FurBallGameEvent.FurballOpponentFurblingKicksBall);
					collisionInfo.rigidbody.velocity = Vector3.zero;
					collisionInfo.rigidbody.AddForce(force, ForceMode.Impulse);
					collisionInfo.gameObject.GetComponent<FurballController>().OnKicked();
				}
				else
				{
					Vector3 vector = -collisionInfo.relativeVelocity;
					vector = Vector3.ClampMagnitude(vector, 0.6f);
					GameEventRouter.SendEvent(FurBallGameEvent.FurballPlayerFurblingKicksBall);
					collisionInfo.rigidbody.velocity = Vector3.zero;
					collisionInfo.rigidbody.AddForce(vector, ForceMode.Impulse);
					collisionInfo.gameObject.GetComponent<FurballController>().OnKicked();
				}
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((FurBallGameEvent)(object)enumValue)
			{
			case FurBallGameEvent.FurballStartPlayingFurbyMode:
				m_currentMovementSetting = MovementControlType.ComputerControlled;
				m_CheckToSeeIfThePlayerIsDoingAnything = false;
				m_currentComputerMovement = ComputerControlledMovement.RunningMove;
				m_currentPathIndex = 0;
				m_diveIndex = 0;
				break;
			case FurBallGameEvent.FurballStartPlayingFurblingMode:
				m_currentMovementSetting = MovementControlType.PlayerControlled;
				m_CheckToSeeIfThePlayerIsDoingAnything = true;
				break;
			case FurBallGameEvent.FurballGameFinished:
				m_currentMovementSetting = MovementControlType.NoMovementAllowed;
				m_CheckToSeeIfThePlayerIsDoingAnything = false;
				m_NumberOfTimesOpponentHasStarted = 0;
				break;
			case FurBallGameEvent.FurballBallShotsEnded:
				m_currentPathIndex++;
				break;
			case FurBallGameEvent.FurballReadyToShootBall:
				m_currentComputerMovement = ComputerControlledMovement.RunningMove;
				GameEventRouter.SendEvent(HintEvents.Furball_HitTheRebound_End);
				break;
			case FurBallGameEvent.FurballStartBallRound:
			{
				float num = m_chanceOfDiving[m_diveIndex % m_chanceOfDiving.Length];
				if (UnityEngine.Random.Range(0f, 1f) < num)
				{
					m_currentComputerMovement = ComputerControlledMovement.Diving;
					m_hasDived = false;
				}
				else
				{
					m_currentComputerMovement = ComputerControlledMovement.RunningMove;
				}
				m_diveIndex++;
				m_NumberOfTimesOpponentHasStarted++;
				break;
			}
			case FurBallGameEvent.FurballFurblingModeActivateStartRound:
				m_NumberOfTimesOpponentHasStarted++;
				m_currentComputerMovement = ComputerControlledMovement.Diving;
				m_hasDived = false;
				break;
			case FurBallGameEvent.FurballFurblingKicksBall:
				m_currentComputerMovement = ComputerControlledMovement.RunningMove;
				break;
			case FurBallGameEvent.FurballGoalScored:
				m_currentComputerMovement = ComputerControlledMovement.RunningMove;
				break;
			case FurBallGameEvent.FurballOpponentFurblingKicksBall:
				m_NumberOfTimesGoalieSavedNoRebound++;
				if (m_NumberOfTimesGoalieSavedNoRebound > 1)
				{
					GameEventRouter.SendEvent(HintEvents.Furball_HitTheRebound_Start);
				}
				break;
			case FurBallGameEvent.FurballBouncerHitsBall:
				m_NumberOfTimesGoalieSavedNoRebound = 0;
				GameEventRouter.SendEvent(HintEvents.Furball_HitTheRebound_End);
				break;
			}
		}
	}
}
