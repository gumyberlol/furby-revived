using Relentless;
using UnityEngine;

namespace Furby
{
	public class LoopScreen : RelentlessMonoBehaviour
	{
		public float m_screenSize;

		private int m_screenNumber;

		private void Start()
		{
			float num = base.transform.localPosition.x + base.transform.parent.localPosition.x;
			m_screenNumber = (int)(num / m_screenSize);
			BroadcastMessage(FurbyGUIMessages.s_setScreenNumber, m_screenNumber, SendMessageOptions.DontRequireReceiver);
		}

		private void OnDragMotionEnd(int screenChange)
		{
			m_screenNumber += screenChange;
			if (m_screenNumber == 2)
			{
				m_screenNumber = -1;
				base.transform.localPosition -= new Vector3(m_screenSize * 3f, 0f, 0f);
			}
			else if (m_screenNumber == -2)
			{
				m_screenNumber = 1;
				base.transform.localPosition += new Vector3(m_screenSize * 3f, 0f, 0f);
			}
			BroadcastMessage(FurbyGUIMessages.s_setScreenNumber, m_screenNumber, SendMessageOptions.DontRequireReceiver);
		}
	}
}
