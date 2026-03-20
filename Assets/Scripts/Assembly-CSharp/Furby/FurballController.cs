using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurballController : MonoBehaviour
	{
		private float m_timeSinceKicked;

		private float m_ballKickedTimeout = 5f;

		private FurbyStrikerController m_ballManager;

		public void KickBall(Vector3 startPosition, Vector3 targetPosition, float kickForce, FurbyStrikerController ballManager)
		{
			m_timeSinceKicked = 0f;
			base.gameObject.SetActive(true);
			m_ballManager = ballManager;
			base.transform.position = startPosition;
			base.rigidbody.velocity = Vector3.zero;
			base.rigidbody.angularVelocity = Vector3.zero;
			base.rigidbody.AddForce((targetPosition - startPosition).normalized * kickForce);
			GameEventRouter.SendEvent(FurBallGameEvent.FurballBallStart);
			GameEventRouter.SendEvent(HintEvents.Furball_DispatchedBall);
		}

		private void Update()
		{
			m_timeSinceKicked += Time.deltaTime;
			if (m_timeSinceKicked > m_ballKickedTimeout)
			{
				ResetBall();
			}
		}

		public void OnKicked()
		{
			m_timeSinceKicked = 0f;
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (base.gameObject.activeInHierarchy)
			{
				switch (collisionInfo.collider.tag)
				{
				case "Furball_GoalSaveCollider":
					GameEventRouter.SendEvent(FurBallGameEvent.FurballBallDeflected);
					ResetBall();
					break;
				case "Furball_GoalScoreCollider":
					GameEventRouter.SendEvent(FurBallGameEvent.FurballGoalScored);
					ResetBall();
					break;
				case "Furball_Goalie":
					break;
				}
			}
		}

		private void ResetBall()
		{
			base.transform.position = Vector3.zero;
			base.gameObject.SetActive(false);
			m_ballManager.ReturnBallToPool(this);
		}
	}
}
