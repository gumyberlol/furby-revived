using Relentless;
using UnityEngine;

namespace Furby
{
	public class ScrollOnAction : RelentlessMonoBehaviour
	{
		public float m_amount;

		public float m_max;

		public float m_min;

		public float m_speed;

		private float m_target;

		private void Update()
		{
			float num = m_target - base.transform.localPosition.x;
			if (num != 0f)
			{
				float x = Mathf.Min(m_speed * Time.deltaTime, Mathf.Abs(num)) * Mathf.Sign(num);
				base.transform.localPosition += new Vector3(x, 0f, 0f);
			}
		}

		private void AddScroll(int direction)
		{
			m_target += (float)direction * m_amount;
			m_target = Mathf.Clamp(m_target, m_min, m_max);
		}

		private void OnScrollLeft()
		{
			AddScroll(1);
		}

		private void OnScrollRight()
		{
			AddScroll(-1);
		}
	}
}
