using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class ThresholdTrigger
	{
		[SerializeField]
		public float m_ThresholdValue;

		[SerializeField]
		public bool m_Handled;

		[SerializeField]
		public Collider m_Collider;

		public ThresholdTrigger(float value, Collider collider)
		{
			m_ThresholdValue = value;
			m_Collider = collider;
			m_Handled = false;
		}
	}
}
