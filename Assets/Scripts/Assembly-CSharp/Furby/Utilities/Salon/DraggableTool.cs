using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class DraggableTool : MonoBehaviour
	{
		private float maxX = 0.4f;

		private float minX = -0.4f;

		private float maxY = 0.35f;

		private float minY = -0.65f;

		private float m_PressTime = float.NaN;

		public SalonItem m_SalonItem;

		public PlayMakerFSM m_GameState;

		public ParticleSystem ps;

		private bool held;

		public bool IsDragActive
		{
			get
			{
				return held;
			}
		}

		public void OnPress(bool dragStart)
		{
			m_GameState.SendEvent("Confirmed");
			if (!dragStart)
			{
				Collider collider = UICamera.lastHit.collider;
				if (collider != null)
				{
					OnDragEnd(collider.gameObject);
				}
				else
				{
					OnDragEnd(null);
				}
			}
			else
			{
				m_PressTime = Time.fixedTime;
			}
		}

		public void Update()
		{
			if (!float.IsNaN(m_PressTime))
			{
				OnDragBegin();
			}
		}

		public void OnDrag(Vector2 delta)
		{
			if (IsDragActive)
			{
				Camera currentCamera = UICamera.currentCamera;
				Transform transform = base.transform;
				Vector3 position = UICamera.lastTouchPosition;
				Vector3 position2 = currentCamera.ScreenToWorldPoint(position);
				position2.z = transform.position.z;
				position2.x = Mathf.Clamp(position2.x, minX, maxX);
				position2.y = Mathf.Clamp(position2.y, minY, maxY);
				transform.position = position2;
			}
			m_PressTime = float.NaN;
		}

		public virtual void OnDragBegin()
		{
			GetComponent<Animation>().Stop();
			ps.gameObject.SetActive(false);
			held = true;
			m_PressTime = float.NaN;
		}

		public virtual void OnDragEnd(GameObject gameObject)
		{
			held = false;
		}
	}
}
