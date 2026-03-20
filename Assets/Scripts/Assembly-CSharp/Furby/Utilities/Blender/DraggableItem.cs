using UnityEngine;

namespace Furby.Utilities.Blender
{
	public class DraggableItem : MonoBehaviour
	{
		private float m_MaxTimeDelta = 0.25f;

		private float m_MaxOriginDelta = 16f;

		private float m_ClickTime = float.NaN;

		public void OnPress(bool pressStart)
		{
			if (!pressStart)
			{
				UICamera.MouseOrTouch currentTouch = UICamera.currentTouch;
				float num = Time.fixedTime - m_ClickTime;
				float magnitude = currentTouch.totalDelta.magnitude;
				if (num <= m_MaxTimeDelta && magnitude <= m_MaxOriginDelta)
				{
					OnSingleClick();
				}
			}
			else
			{
				m_ClickTime = Time.fixedTime;
			}
		}

		public void Update()
		{
		}

		public virtual void OnSingleClick()
		{
			m_ClickTime = float.NaN;
		}
	}
}
