using Relentless;
using UnityEngine;

namespace Furby
{
	public class GiftSelect : RelentlessMonoBehaviour
	{
		private int m_GiftIndex = -1;

		private GiftStatus m_Status = GiftStatus.Locked;

		public UILabel m_NGUILabel;

		public UISprite m_NGUISprite_Locked;

		public UISprite m_NGUISprite_Awarded;

		public UISprite m_NGUISprite_Opened;

		private bool m_AmInitialized;

		public void InitalizeGift(int index, GiftStatus status)
		{
			m_GiftIndex = index;
			m_Status = status;
			m_AmInitialized = true;
			m_NGUISprite_Locked.enabled = false;
			m_NGUISprite_Awarded.enabled = false;
			m_NGUISprite_Opened.enabled = false;
			switch (m_Status)
			{
			case GiftStatus.Locked:
				m_NGUISprite_Locked.enabled = true;
				GetComponent<UIButton>().enabled = false;
				GetComponent<UIButtonScale>().enabled = false;
				GetComponent<BoxCollider>().enabled = false;
				break;
			case GiftStatus.Unopened:
				m_NGUISprite_Awarded.enabled = true;
				GetComponent<UIButton>().enabled = true;
				GetComponent<UIButtonScale>().enabled = true;
				GetComponent<BoxCollider>().enabled = true;
				break;
			case GiftStatus.Opened:
				m_NGUISprite_Opened.enabled = true;
				GetComponent<UIButton>().enabled = false;
				GetComponent<UIButtonScale>().enabled = false;
				GetComponent<BoxCollider>().enabled = false;
				break;
			}
		}

		public UISprite GetTargetSprite()
		{
			switch (m_Status)
			{
			case GiftStatus.Locked:
				return m_NGUISprite_Locked;
			case GiftStatus.Unopened:
				return m_NGUISprite_Awarded;
			case GiftStatus.Opened:
				return m_NGUISprite_Opened;
			default:
				return m_NGUISprite_Locked;
			}
		}

		private void OnClick()
		{
			if (m_AmInitialized && m_Status == GiftStatus.Unopened)
			{
				base.gameObject.SendGameEvent(GiftUnlockingEvents.Gifting_ChooseGift_Chosen, m_GiftIndex);
			}
		}
	}
}
