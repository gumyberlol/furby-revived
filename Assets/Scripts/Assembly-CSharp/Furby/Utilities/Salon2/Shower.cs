using System;
using Furby.Utilities.Hose;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Shower : MonoBehaviour
	{
		private class EnabledPeriod : IDisposable
		{
			public delegate void OnDisposeHandler();

			public event OnDisposeHandler OnDispose;

			public void Dispose()
			{
				if (this.OnDispose != null)
				{
					this.OnDispose();
				}
			}
		}

		public delegate void Handler();

		[SerializeField]
		private GameObject m_effectPrefab;

		[SerializeField]
		private GameObject m_spawnPoint;

		[SerializeField]
		private Dial m_dial;

		[SerializeField]
		private Collider m_dialCollider;

		private GameObject m_effect;

		public Dial Dial
		{
			get
			{
				return m_dial;
			}
		}

		public bool IsOn
		{
			get
			{
				return m_effect != null;
			}
		}

		public event Handler Switched;

		public void Start()
		{
			m_dial.ValueChanged += delegate(float f)
			{
				bool onOff = f >= 0.5f;
				SetOnOff(onOff);
			};
		}

		private void SetOnOff(bool b)
		{
			bool isOn = IsOn;
			if (b)
			{
				SwitchOn();
			}
			else
			{
				SwitchOff();
			}
			if (IsOn != isOn && this.Switched != null)
			{
				this.Switched();
			}
		}

		public void SwitchOn()
		{
			if (!IsOn)
			{
				base.gameObject.SendGameEvent(SalonGameEvent.ShowerOn);
				if (m_effect == null)
				{
					m_effect = UnityEngine.Object.Instantiate(m_effectPrefab) as GameObject;
					m_effect.transform.parent = m_spawnPoint.transform;
					m_effect.transform.localPosition = Vector3.zero;
				}
			}
		}

		public void SwitchOff()
		{
			if (!IsOn)
			{
				return;
			}
			base.gameObject.SendGameEvent(SalonGameEvent.ShowerOff);
			if (m_effect != null)
			{
				ParticleSystem[] componentsInChildren = m_effect.GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Stop();
				}
				m_effect = null;
			}
		}

		private void ForceOff()
		{
			SetOnOff(false);
			m_dial.SetValue(0f);
		}

		public IDisposable GetEnabledPeriod()
		{
			EnabledPeriod enabledPeriod = new EnabledPeriod();
			enabledPeriod.OnDispose += delegate
			{
				ForceOff();
			};
			return enabledPeriod;
		}

		public void EnableDialCollider(bool enable)
		{
			if (enable)
			{
				m_dialCollider.enabled = true;
			}
			else
			{
				m_dialCollider.enabled = false;
			}
		}
	}
}
