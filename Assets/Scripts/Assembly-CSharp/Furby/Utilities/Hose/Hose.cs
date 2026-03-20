using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class Hose : MonoBehaviour
	{
		public delegate void Handler(float p, float t);

		private float m_p;

		private float m_t;

		public event Handler Changed;

		public void SetPressure(float f)
		{
			m_p = f;
			EmitChange();
		}

		public void SetTemperature(float f)
		{
			m_t = f;
			EmitChange();
		}

		public bool IsOn()
		{
			return m_p > 0f;
		}

		private void EmitChange()
		{
			float p = m_p;
			float t = ((!(m_p > 0f)) ? 0f : m_t);
			if (this.Changed != null)
			{
				this.Changed(p, t);
			}
		}
	}
}
