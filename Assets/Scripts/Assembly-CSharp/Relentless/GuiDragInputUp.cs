using UnityEngine;

namespace Relentless
{
	public class GuiDragInputUp : MonoBehaviour
	{
		public bool LimitToBoxCollider = true;

		public BoxCollider m_boxCollider;

		private void OnDrag(Vector2 delta)
		{
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
			float distance = 0f;
			Vector3 point = ray.GetPoint(distance);
			if (LimitToBoxCollider && m_boxCollider != null)
			{
				if (point.x < m_boxCollider.bounds.min.x)
				{
					point.x = m_boxCollider.bounds.min.x;
				}
				else if (point.x > m_boxCollider.bounds.max.x)
				{
					point.x = m_boxCollider.bounds.max.x;
				}
				if (point.y < m_boxCollider.bounds.min.y)
				{
					point.y = m_boxCollider.bounds.min.y;
				}
				else if (point.y > m_boxCollider.bounds.max.y)
				{
					point.y = m_boxCollider.bounds.max.y;
				}
			}
			DragData dragData = new DragData();
			dragData.EventName = base.gameObject.name;
			dragData.Position = point;
			DragData value = dragData;
			SendMessageUpwards("GuiDragInput", value);
		}
	}
}
