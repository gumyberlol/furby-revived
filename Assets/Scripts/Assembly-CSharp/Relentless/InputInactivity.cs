using Furby;
using UnityEngine;

namespace Relentless
{
	public class InputInactivity : RelentlessMonoBehaviour
	{
		[SerializeField]
		private HintEvents m_event;

		private float m_TimeOfLastInput;

		public void Awake()
		{
			m_TimeOfLastInput = Time.time;
		}

		public void Update()
		{
			bool flag = false;
			flag |= Input.touchCount > 0;
			for (int i = 0; i < 3; i++)
			{
				if (flag)
				{
					break;
				}
				flag |= Input.GetMouseButton(i);
			}
			if (flag)
			{
				Emit();
			}
		}

		private void Emit()
		{
			ResetInactivity();
			GameEventRouter.SendEvent(m_event);
		}

		public bool HasIntervalPassed(float timeSecs)
		{
			float num = Time.time - m_TimeOfLastInput;
			return num >= timeSecs;
		}

		public float GetInterval()
		{
			return Time.time - m_TimeOfLastInput;
		}

		public void ResetInactivity()
		{
			m_TimeOfLastInput = Time.time;
		}
	}
}
