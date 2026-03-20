using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ScrubBoredomAvoider : MonoBehaviour
	{
		private float m_busyBackgroundTimer;

		private float m_timeSincePointScrubbed = -1f;

		[SerializeField]
		private float m_bgActivityDuration = 1f;

		[SerializeField]
		private float m_boredomThreshold = 2f;

		public ScrubSystem System
		{
			get
			{
				return base.gameObject.GetComponent<ScrubSystem>();
			}
		}

		public void Start()
		{
			System.Points.PointScrubbed += delegate(ScrubPoint point)
			{
				Logging.Log(string.Format("Scrubbed on {0} sets boredom timer to zero.", point.gameObject.name));
				m_timeSincePointScrubbed = 0f;
			};
			System.Background.LevelChanged += delegate
			{
				AvoidBoredom();
			};
		}

		public void Update()
		{
			float deltaTime = Time.deltaTime;
			RubbyPoint background = System.Background;
			if (background.HasRecentMovement())
			{
				m_busyBackgroundTimer += deltaTime;
			}
			else
			{
				m_busyBackgroundTimer = 0f;
			}
			if (m_timeSincePointScrubbed >= 0f)
			{
				m_timeSincePointScrubbed += deltaTime;
			}
		}

		private void AvoidBoredom()
		{
			if (IsBoring())
			{
				Logging.Log("Boredom detected, forcing a scrub point");
				System.Points.ScrubRandom();
			}
		}

		private bool IsBoring()
		{
			bool flag = m_busyBackgroundTimer >= m_bgActivityDuration;
			bool flag2 = m_timeSincePointScrubbed >= 0f && m_timeSincePointScrubbed < m_boredomThreshold;
			return flag && !flag2;
		}
	}
}
