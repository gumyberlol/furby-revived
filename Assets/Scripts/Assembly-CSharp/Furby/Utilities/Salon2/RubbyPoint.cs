using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class RubbyPoint : MonoBehaviour
	{
		public delegate bool ColliderTester(Collider collider);

		public delegate bool EnvironmentTester();

		public delegate void LevelHandler(float f);

		public delegate void Handler();

		public ColliderTester m_colliderTester;

		public EnvironmentTester m_envTester;

		private Vector3 m_lastPos;

		private float m_level;

		private float m_prevLevel;

		[SerializeField]
		private float m_maxLevel = 1f;

		[SerializeField]
		private float m_stopThreshold = 0.1f;

		private float m_timeSinceLastMovement = -1f;

		[SerializeField]
		private float m_timeBeforeCooldown = 0.5f;

		public event Handler EnvTestFailed;

		public event LevelHandler LevelChanged;

		public event Handler OnMovement;

		public event Handler OnMovementStart;

		public event Handler OnMovementStop;

		public bool HasRecentMovement()
		{
			return m_timeSinceLastMovement >= 0f && m_timeSinceLastMovement < m_stopThreshold;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (ColliderIsRelevant(collider))
			{
				m_lastPos = collider.transform.position;
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (ColliderIsRelevant(collider))
			{
				GetRubbedBy(collider);
			}
		}

		private void GetRubbedBy(Collider collider)
		{
			bool prev = HasRecentMovement();
			Vector3 position = collider.transform.position;
			float magnitude = (position - m_lastPos).magnitude;
			m_lastPos = position;
			float num = magnitude / Time.fixedDeltaTime;
			if (num > 0f)
			{
				m_timeSinceLastMovement = 0f;
			}
			bool flag = HasRecentMovement();
			EmitMovementStartEvents(flag, prev);
			if (!flag)
			{
				return;
			}
			if (m_envTester != null && !m_envTester())
			{
				if (this.EnvTestFailed != null)
				{
					this.EnvTestFailed();
				}
			}
			else
			{
				m_level += magnitude;
				m_level = Mathf.Min(m_level, m_maxLevel);
			}
		}

		private void EmitMovementStartEvents(bool now, bool prev)
		{
			if (now && this.OnMovement != null)
			{
				this.OnMovement();
			}
			if (now && !prev && this.OnMovementStart != null)
			{
				this.OnMovementStart();
			}
		}

		public void ForceMax()
		{
			Logging.Log(string.Format("RubbyPoint ForceMax called on \"{0}\"", base.gameObject.name));
			m_level = m_maxLevel;
			m_timeSinceLastMovement = 0f;
		}

		public void Update()
		{
			bool flag = HasRecentMovement();
			if (m_timeSinceLastMovement >= 0f)
			{
				m_timeSinceLastMovement += Time.deltaTime;
			}
			bool flag2 = m_timeSinceLastMovement < m_stopThreshold;
			if (flag && !flag2 && this.OnMovementStop != null)
			{
				this.OnMovementStop();
			}
			if (m_timeSinceLastMovement >= m_timeBeforeCooldown)
			{
				m_level = 0f;
			}
			if (m_level != m_prevLevel)
			{
				float value = m_level / m_maxLevel;
				value = Mathf.Clamp(value, 0f, 1f);
				if (this.LevelChanged != null)
				{
					this.LevelChanged(value);
				}
			}
			m_prevLevel = m_level;
			m_timeSinceLastMovement = Mathf.Min(m_timeSinceLastMovement, Mathf.Max(m_timeBeforeCooldown, m_stopThreshold));
		}

		private bool ColliderIsRelevant(Collider collider)
		{
			return m_colliderTester == null || m_colliderTester(collider);
		}
	}
}
