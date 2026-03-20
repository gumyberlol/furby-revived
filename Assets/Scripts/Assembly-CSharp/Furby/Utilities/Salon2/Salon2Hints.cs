using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Salon2Hints : MonoBehaviour
	{
		[SerializeField]
		private float m_selectIntervalWhenScrollingTriggered = 2f;

		[SerializeField]
		private HintState m_showerOn = new HintState();

		[SerializeField]
		private HintState m_showerOff = new HintState();

		[SerializeField]
		private HintState m_scroll = new HintState();

		[SerializeField]
		private HintState m_select = new HintState();

		[SerializeField]
		private HintState m_scrub = new HintState();

		private HintState m_showerHint;

		public void ConnectShowerListeners(Shower shower)
		{
			shower.Switched += delegate
			{
				base.gameObject.SendGameEvent(HintEvents.Salon_ActionPerformed);
				if (m_showerHint != null)
				{
					m_showerHint.Disable();
				}
			};
		}

		public void ConnectCarouselListeners(Carousel carousel)
		{
			m_scroll.Enable();
			carousel.Scrolled += delegate
			{
				bool flag = m_scroll.IsTriggered();
				m_scroll.Disable();
				base.gameObject.SendGameEvent(HintEvents.Salon_ActionPerformed);
				m_select.Enable();
				if (flag)
				{
					m_select.m_Interval = m_selectIntervalWhenScrollingTriggered;
				}
			};
			carousel.ItemSelected += delegate
			{
				m_select.Disable();
				base.gameObject.SendGameEvent(HintEvents.Salon_ActionPerformed);
				m_scroll.Disable();
				m_scrub.Enable();
			};
		}

		public void ConnectScrubSystemListeners(ScrubSystem scrubs)
		{
			scrubs.Points.AllScrubbed += delegate
			{
				m_scrub.Disable();
			};
			scrubs.Background.EnvTestFailed += delegate
			{
				if (m_showerHint != null)
				{
					m_showerHint.Enable();
				}
			};
			scrubs.Points.AllScrubbed += delegate
			{
				if (m_showerHint != null)
				{
					m_showerHint.Disable();
					m_showerHint = null;
				}
			};
		}

		public void Update()
		{
			m_showerOn.TestAndBroadcastState();
			m_showerOff.TestAndBroadcastState();
			m_scroll.TestAndBroadcastState();
			m_select.TestAndBroadcastState();
			m_scrub.TestAndBroadcastState();
		}

		public void HintShowerOn()
		{
			m_showerHint = m_showerOn;
		}

		public void HintShowerOff()
		{
			m_showerHint = m_showerOff;
		}
	}
}
