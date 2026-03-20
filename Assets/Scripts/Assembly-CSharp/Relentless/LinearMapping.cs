using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class LinearMapping
	{
		[SerializeField]
		public float m_min;

		[SerializeField]
		public float m_max;

		public float GetFor(float t)
		{
			return m_min + (m_max - m_min) * t;
		}

		public float GetRandomInRange()
		{
			return UnityEngine.Random.Range(m_min, m_max);
		}
	}
}
