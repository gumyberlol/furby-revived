using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ScrubPointCollection : MonoBehaviour
	{
		public delegate void AllScrubbedHandler();

		public delegate void PointScrubbedHandler(ScrubPoint p);

		public RubbyPoint.ColliderTester m_colliderTester;

		public RubbyPoint.EnvironmentTester m_envTester;

		[SerializeField]
		private ScrubPoint m_scrubPointPrefab;

		private HashSet<ScrubPoint> m_points = new HashSet<ScrubPoint>();

		public int NumRemaining
		{
			get
			{
				return m_points.Count;
			}
		}

		public event AllScrubbedHandler AllScrubbed;

		public event PointScrubbedHandler PointScrubbed;

		private void GenerateScrubPoints(IEnumerable<GameObject> objects, ScrubSystem.PointGeneratedHandler callback)
		{
			foreach (GameObject @object in objects)
			{
				GenerateScrubPoint(@object, callback);
			}
		}

		public void GenerateScrubPoint(GameObject o, ScrubSystem.PointGeneratedHandler handler)
		{
			ScrubPoint scrubPoint = UnityEngine.Object.Instantiate(m_scrubPointPrefab) as ScrubPoint;
			scrubPoint.gameObject.name = string.Format("Scrub point for \"{0}\"", o.name);
			m_points.Add(scrubPoint);
			SetupPoint(scrubPoint, o);
			if (handler != null)
			{
				handler(o, scrubPoint);
			}
		}

		private void SetupPoint(ScrubPoint p, GameObject o)
		{
			p.transform.parent = o.transform;
			p.transform.localPosition = Vector3.zero;
			p.transform.localScale = Vector3.one;
			p.RubbyPoint.m_colliderTester = (Collider c) => m_colliderTester(c);
			p.RubbyPoint.m_envTester = () => m_envTester();
			ScrubPoint scrubPoint = p;
			scrubPoint.Scrubbed = (ScrubPoint.ScrubHandler)Delegate.Combine(scrubPoint.Scrubbed, (ScrubPoint.ScrubHandler)delegate
			{
				m_points.Remove(p);
				ReportWaitStatus(p);
				if (this.PointScrubbed != null)
				{
					this.PointScrubbed(p);
				}
				if (m_points.Count == 0)
				{
					if (this.AllScrubbed != null)
					{
						this.AllScrubbed();
					}
					UnityEngine.Object.Destroy(base.gameObject);
				}
			});
		}

		private void ReportWaitStatus(ScrubPoint p)
		{
			Logging.Log(string.Format("Point {0} has been scrubbed", p.gameObject.name));
			Logging.Log(string.Format("{0} is still waiting on the following points.", base.gameObject.name));
			foreach (ScrubPoint point in m_points)
			{
				Logging.Log(point.gameObject.name);
			}
		}

		public void ScrubRandom()
		{
			IEnumerator<ScrubPoint> enumerator = m_points.GetEnumerator();
			enumerator.MoveNext();
			ScrubPoint current = enumerator.Current;
			current.RubbyPoint.ForceMax();
		}
	}
}
