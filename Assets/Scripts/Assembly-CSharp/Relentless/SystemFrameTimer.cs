using UnityEngine;

namespace Relentless
{
	public class SystemFrameTimer : SingletonInstance<SystemFrameTimer>
	{
		private float m_lastTime;

		private float m_timeThisFrame;

		private void Start()
		{
			m_lastTime = Time.realtimeSinceStartup;
		}

		private void Update()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			m_timeThisFrame = realtimeSinceStartup - m_lastTime;
			m_lastTime = realtimeSinceStartup;
		}

		public float GetDeltaTime()
		{
			return m_timeThisFrame;
		}
	}
}
