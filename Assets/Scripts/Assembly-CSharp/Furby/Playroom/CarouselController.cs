using UnityEngine;

namespace Furby.Playroom
{
	public class CarouselController : MonoBehaviour
	{
		public GameObject m_GridRoot;

		public GameObject m_Background;

		public void EnableCarousel()
		{
			base.gameObject.SetActive(true);
			if ((bool)m_GridRoot)
			{
				m_GridRoot.SetActive(true);
			}
			if ((bool)m_Background)
			{
				m_Background.SetActive(true);
			}
		}

		public void DisableCarousel()
		{
			base.gameObject.SetActive(false);
			if ((bool)m_GridRoot)
			{
				m_GridRoot.SetActive(false);
			}
			if ((bool)m_Background)
			{
				m_Background.SetActive(false);
			}
		}
	}
}
