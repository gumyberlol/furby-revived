using Relentless;
using UnityEngine;

namespace Furby
{
	public class ScrollButton : RelentlessMonoBehaviour
	{
		public string m_scrollScreenName;

		public bool m_leftButton;

		private FingerGesturesSwipe m_scroller;

		private UIButton m_buttonComponent;

		private void Start()
		{
			GameObject gameObject = GameObject.Find(m_scrollScreenName);
			if (gameObject != null)
			{
				m_scroller = gameObject.GetComponent<FingerGesturesSwipe>();
			}
			m_buttonComponent = GetComponent<UIButton>();
		}

		private void Update()
		{
			if (!(m_scroller != null))
			{
				return;
			}
			bool flag = false;
			if (!m_leftButton)
			{
				if (m_scroller.CanScrollRight())
				{
					flag = true;
				}
			}
			else if (m_scroller.CanScrollLeft())
			{
				flag = true;
			}
			if (flag)
			{
				m_buttonComponent.isEnabled = true;
			}
			else
			{
				m_buttonComponent.isEnabled = false;
			}
		}
	}
}
