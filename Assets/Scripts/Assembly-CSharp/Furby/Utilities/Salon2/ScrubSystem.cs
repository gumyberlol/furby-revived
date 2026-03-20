using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ScrubSystem : MonoBehaviour
	{
		public delegate bool ColliderTester(Collider c);

		public delegate void PointGeneratedHandler(GameObject obj, ScrubPoint point);

		public delegate void ProgressionHandler(float progression);

		public RubbyPoint.EnvironmentTester m_envTester;

		[SerializeField]
		private ScrubPointCollection m_points;

		[SerializeField]
		private RubbyPoint m_background;

		private Collider m_scrubber;

		private int m_totalPointCount;

		public ScrubPointCollection Points
		{
			get
			{
				return m_points;
			}
		}

		public RubbyPoint Background
		{
			get
			{
				return m_background;
			}
		}

		public Collider Scrubber
		{
			get
			{
				return m_scrubber;
			}
			set
			{
				m_scrubber = value;
			}
		}

		public event PointGeneratedHandler PointGenerated;

		public event ProgressionHandler OnProgression;

		public void Start()
		{
			Points.m_colliderTester = (Collider c) => c == m_scrubber;
			Background.m_colliderTester = (Collider c) => c == m_scrubber;
			Points.m_envTester = () => m_envTester == null || m_envTester();
			Background.m_envTester = () => m_envTester == null || m_envTester();
			Points.AllScrubbed += delegate
			{
				Object.Destroy(base.gameObject);
			};
			Points.PointScrubbed += delegate
			{
				int numRemaining = Points.NumRemaining;
				float num = (float)numRemaining / (float)m_totalPointCount;
				float value = 1f - num;
				value = Mathf.Clamp(value, 0f, 1f);
				if (this.OnProgression != null)
				{
					this.OnProgression(value);
				}
			};
		}

		public void GenerateScrubPoint(GameObject o, PointGeneratedHandler callerFunc)
		{
			PointGeneratedHandler handler = delegate(GameObject obj, ScrubPoint point)
			{
				callerFunc(obj, point);
				if (this.PointGenerated != null)
				{
					this.PointGenerated(obj, point);
				}
			};
			Points.GenerateScrubPoint(o, handler);
			m_totalPointCount++;
		}
	}
}
