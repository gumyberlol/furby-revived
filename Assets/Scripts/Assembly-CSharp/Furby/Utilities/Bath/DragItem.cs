using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class DragItem : MonoBehaviour
	{
		protected bool preDragging;

		protected bool isDragging;

		public bool constainX;

		public bool constainY = true;

		public bool constainZ = true;

		protected Vector2 dragStartScreenPos;

		protected Vector2 dragCurrentScreenPos;

		protected Vector3 dragStartWorldPos;

		protected Vector3 dragCurrentWorldPos;

		protected Vector3 itemStartWorldPos;

		protected Vector3 itemCurrentWorldPos;

		protected Vector3 targetStartWorldPos;

		protected Vector3 targetCurrentWorldPos;

		protected float lastHitDistance;

		public GameObject dragTarget;

		private void DEBUG_HitInfo(RaycastHit hitInfo)
		{
			if (hitInfo.collider == null)
			{
				Logging.Log("DEBUG_HitInfo(): No Collider");
			}
			else if (hitInfo.collider == base.collider)
			{
				Logging.Log("DEBUG_HitInfo(): Hit self");
			}
			else
			{
				Logging.Log(string.Format("DEBUG_HitInfo(): Hit something - {0}", hitInfo.collider.gameObject.name));
			}
		}

		public bool HaveHit()
		{
			RaycastHit lastHit = UICamera.lastHit;
			return lastHit.collider != null;
		}

		public bool VerifyHit()
		{
			Ray ray = UICamera.currentCamera.ScreenPointToRay(new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, 0f));
			float num = Mathf.Abs((base.transform.position - UICamera.currentCamera.transform.position).z);
			RaycastHit hitInfo;
			return base.collider.Raycast(ray, out hitInfo, num * 2f);
		}

		protected Vector3 PointOnItemRecalculate()
		{
			Ray ray = UICamera.currentCamera.ScreenPointToRay(new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, 0f));
			float num = Mathf.Abs((base.transform.position - UICamera.currentCamera.transform.position).magnitude);
			RaycastHit hitInfo;
			if (!base.collider.Raycast(ray, out hitInfo, num * 2f))
			{
				Logging.Log("PointOnItemRecalculate(): Failed to Hit Item. What's up with that?");
				return UICamera.currentCamera.ScreenToWorldPoint(new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, lastHitDistance));
			}
			lastHitDistance = Mathf.Abs((hitInfo.point - UICamera.currentCamera.transform.position).magnitude);
			return hitInfo.point;
		}

		protected Vector3 PointOnItem()
		{
			RaycastHit lastHit = UICamera.lastHit;
			if ((bool)lastHit.collider)
			{
				lastHitDistance = Mathf.Abs((lastHit.point - UICamera.currentCamera.transform.position).z);
				return lastHit.point;
			}
			Logging.Log("PointOnItem(): No hit. Taking an educated guess");
			return PointOnItemRecalculate();
		}

		private void StoreCurrentPositions()
		{
			dragCurrentScreenPos = UICamera.lastTouchPosition;
			dragCurrentWorldPos = PointOnItem();
		}

		protected void StartDrag()
		{
			GameObject gameObject = ((!dragTarget) ? base.gameObject : dragTarget);
			targetStartWorldPos = gameObject.transform.position;
			itemStartWorldPos = base.transform.position;
			StoreCurrentPositions();
			dragStartWorldPos = dragCurrentWorldPos;
			dragStartScreenPos = dragCurrentScreenPos;
			preDragging = true;
		}

		protected void StopDrag()
		{
			StoreCurrentPositions();
			isDragging = false;
		}

		public virtual void OnPress(bool isPressed)
		{
			if (isPressed)
			{
				if (!isDragging)
				{
					StartDrag();
				}
				return;
			}
			preDragging = false;
			DEBUG_HitInfo(UICamera.lastHit);
			if (isDragging)
			{
				StopDrag();
			}
		}

		protected void ApplyDrag()
		{
			Vector3 vector = dragCurrentWorldPos - dragStartWorldPos;
			if (constainX)
			{
				vector.x = 0f;
			}
			if (constainY)
			{
				vector.y = 0f;
			}
			if (constainZ)
			{
				vector.z = 0f;
			}
			itemCurrentWorldPos = itemStartWorldPos + vector;
			targetCurrentWorldPos = targetStartWorldPos + vector;
			if ((bool)dragTarget)
			{
				dragTarget.transform.position = targetCurrentWorldPos;
			}
			else
			{
				base.transform.position = itemCurrentWorldPos;
			}
		}

		public virtual void OnDrag(Vector2 delta)
		{
			if (preDragging)
			{
				isDragging = true;
				preDragging = false;
			}
			if (isDragging)
			{
				StoreCurrentPositions();
				ApplyDrag();
			}
		}

		public GameObject DoDrop()
		{
			RaycastHit lastHit = UICamera.lastHit;
			if (lastHit.collider == null)
			{
				return null;
			}
			if (lastHit.collider == base.collider)
			{
				return null;
			}
			GameObject gameObject = lastHit.collider.gameObject;
			DropTarget component = gameObject.GetComponent<DropTarget>();
			if (component == null)
			{
				return null;
			}
			if (component.MyDrop(base.gameObject))
			{
				return gameObject;
			}
			return null;
		}
	}
}
