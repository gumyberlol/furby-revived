using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class ShowerSwitch : MonoBehaviour
	{
		private bool m_shower;

		private UISprite m_switchSprite;

		public ParticleManager pm;

		private float cooldown;

		private void Start()
		{
			m_shower = false;
			m_switchSprite = GetComponent<UISprite>();
		}

		private void Update()
		{
			if (cooldown > 0f)
			{
				cooldown -= Time.deltaTime;
			}
		}

		private void OnDrag(Vector2 delta)
		{
			if (delta.x < -5f && m_shower)
			{
				TurnOff();
			}
			else if (delta.x > 5f && !m_shower)
			{
				TurnOn();
			}
		}

		private void OnClick()
		{
			if (m_shower)
			{
				TurnOff();
			}
			else
			{
				TurnOn();
			}
		}

		private void TurnOn()
		{
			if (!(cooldown > 0f))
			{
				m_switchSprite.transform.Rotate(0f, 180f, 0f);
				m_shower = true;
				pm.Activator(true);
				base.gameObject.SendGameEvent(SalonGameEvent.ShowerOn);
			}
		}

		private void TurnOff()
		{
			if (!(cooldown > 0f))
			{
				m_switchSprite.transform.Rotate(0f, -180f, 0f);
				m_shower = false;
				pm.Activator(false);
				base.gameObject.SendGameEvent(SalonGameEvent.ShowerOff);
			}
		}

		public void Reset()
		{
			base.gameObject.SendGameEvent(SalonGameEvent.ShowerOff);
			m_switchSprite.transform.localRotation = Quaternion.identity;
			m_shower = false;
			pm.ChangeEmission(0f);
			cooldown = 1f;
		}
	}
}
