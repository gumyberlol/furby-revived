using System;
using System.Collections;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class Dial : MonoBehaviour
	{
		public delegate void ValueHandler(float f);

		public delegate void Handler();

		private float m_f;

		private bool m_settingFromDial;

		private DialHandle m_handle;

		public float Value
		{
			get
			{
				return m_f;
			}
		}

		public event ValueHandler ValueChanged;

		public event Handler Touched;

		public event Handler HitMax;

		public event Handler HitMin;

		public void Start()
		{
			m_handle = base.gameObject.GetComponentInChildren<DialHandle>();
			if (m_handle == null)
			{
				throw new ApplicationException(string.Format("{0} failed to find a DialHandle instance in any of its children", base.gameObject.name));
			}
			m_handle.ValueChanged += delegate(float f)
			{
				bool settingFromDial = m_settingFromDial;
				m_settingFromDial = true;
				SetValue(f);
				m_settingFromDial = settingFromDial;
			};
			m_handle.Touched += delegate
			{
				if (this.Touched != null)
				{
					this.Touched();
				}
			};
			SetValue(0f);
		}

		public void SetValue(float f)
		{
			float f2 = m_f;
			m_f = f;
			if (!m_settingFromDial)
			{
				m_handle.SetValue(f);
			}
			EmitEvents(f2);
		}

		public IEnumerator SetValueOverTime(float f, float speed)
		{
			while (true)
			{
				float distance = f - Value;
				float delta = speed * Time.deltaTime;
				bool lastFrame = Mathf.Abs(delta) >= Mathf.Abs(distance);
				delta = Mathf.Min(Mathf.Abs(delta), Mathf.Abs(distance));
				delta = ((!(distance < 0f)) ? delta : (delta * -1f));
				SetValue(Value + delta);
				if (lastFrame)
				{
					break;
				}
				yield return null;
			}
		}

		private void EmitEvents(float prev)
		{
			if (prev != m_f && this.ValueChanged != null)
			{
				this.ValueChanged(m_f);
			}
			if (m_f >= 1f && prev < 1f && this.HitMax != null)
			{
				this.HitMax();
			}
			if (m_f <= 0f && prev > 0f && this.HitMin != null)
			{
				this.HitMin();
			}
		}
	}
}
