using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class Spray : MonoBehaviour
	{
		public delegate void OnHandler();

		private bool m_on;

		[SerializeField]
		private ParticleSystem m_particles;

		public event OnHandler m_switchedOn;

		public void Start()
		{
			if (m_particles == null)
			{
				throw new ApplicationException(string.Format("{0} has not had its particle system set.", base.gameObject.name));
			}
		}

		public void On()
		{
			m_particles.Play();
			m_on = true;
			base.gameObject.SendGameEvent(PoopStationEvent.SprayPressed);
			if (this.m_switchedOn != null)
			{
				this.m_switchedOn();
			}
		}

		public void Off()
		{
			m_particles.Stop();
			m_on = false;
			base.gameObject.SendGameEvent(PoopStationEvent.SprayReleased);
		}

		public bool IsOn()
		{
			return m_on;
		}
	}
}
