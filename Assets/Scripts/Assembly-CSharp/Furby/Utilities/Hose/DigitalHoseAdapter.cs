using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class DigitalHoseAdapter : MonoBehaviour
	{
		[Serializable]
		public class ThresholdAmount
		{
			public float m_upper;

			public int m_value;
		}

		public delegate void ChangeHandler();

		private Temperature m_temp = Temperature.Nice;

		private Pressure m_pressure;

		[SerializeField]
		private Hose m_hose;

		[SerializeField]
		private List<ThresholdAmount> m_pressureThresholds;

		[SerializeField]
		private List<ThresholdAmount> m_temperatureThresholds;

		public Hose Hose
		{
			get
			{
				return m_hose;
			}
		}

		public event ChangeHandler SwitchedOn;

		public event ChangeHandler SwitchedOff;

		public event ChangeHandler OnOffChange;

		public event ChangeHandler TemperatureChanged;

		public event ChangeHandler PressureChanged;

		public event ChangeHandler StatusChanged;

		public int GetThresholdValue(IEnumerable<ThresholdAmount> amounts, float f)
		{
			foreach (ThresholdAmount amount in amounts)
			{
				if (amount.m_upper >= f)
				{
					return amount.m_value;
				}
			}
			throw new ApplicationException(string.Format("No ThresholdAmount for value {1}", f));
		}

		public void Start()
		{
			if (m_hose == null)
			{
				throw new ApplicationException(string.Format("{0} needs a hose.", base.gameObject.name));
			}
			m_hose.Changed += OnChange;
		}

		private void OnChange(float p, float t)
		{
			Temperature temp = m_temp;
			Pressure pressure = m_pressure;
			bool flag = IsOn();
			m_pressure = (Pressure)GetThresholdValue(m_pressureThresholds, p);
			m_temp = (Temperature)GetThresholdValue(m_temperatureThresholds, t);
			bool flag2 = m_pressure != pressure;
			bool flag3 = m_temp != temp;
			bool flag4 = flag != IsOn();
			flag3 &= IsOn();
			if (flag2 && this.PressureChanged != null)
			{
				this.PressureChanged();
			}
			if (flag3 && this.TemperatureChanged != null)
			{
				this.TemperatureChanged();
			}
			if ((flag3 || flag2) && this.StatusChanged != null)
			{
				this.StatusChanged();
			}
			if (flag4 && IsOn() && this.SwitchedOn != null)
			{
				this.SwitchedOn();
			}
			if (flag4 && !IsOn() && this.SwitchedOff != null)
			{
				this.SwitchedOff();
			}
			if (flag4 && this.OnOffChange != null)
			{
				this.OnOffChange();
			}
		}

		public Temperature GetTemperature()
		{
			return m_temp;
		}

		public Pressure GetPressure()
		{
			return m_pressure;
		}

		public bool IsOn()
		{
			return !m_pressure.Equals(Pressure.Off);
		}
	}
}
