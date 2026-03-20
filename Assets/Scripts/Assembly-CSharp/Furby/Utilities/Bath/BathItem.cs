using HutongGames.PlayMaker;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class BathItem : DragItem
	{
		private GameObject savedTarget;

		private bool onShelf = true;

		private float dragOffShelfThresholdYMin = 30f;

		private float dragOffShelfThresholdXMax = 15f;

		private Vector3 savedShelfLocalPosition;

		private Transform savedParent;

		private void ApplyAdjustedDragUnconstrained_(Vector3 adjustment)
		{
			dragCurrentWorldPos += adjustment;
			Vector3 vector = dragCurrentWorldPos - dragStartWorldPos;
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
			dragStartWorldPos = dragCurrentWorldPos;
			itemStartWorldPos = itemCurrentWorldPos;
			targetStartWorldPos = targetCurrentWorldPos;
		}

		private void StartDragOffShelf()
		{
			if (onShelf)
			{
				if (!HaveHit())
				{
					Logging.Log(string.Format("StopDragOffShelf(): {0} - No can do - No Hit Info", base.name));
					return;
				}
				if (!VerifyHit())
				{
					Logging.Log(string.Format("StopDragOffShelf(): {0} - Failed Verify", base.name));
					return;
				}
				onShelf = false;
				Logging.Log(string.Format("StartDragOffShelf: {0}", base.name));
				savedShelfLocalPosition = base.transform.localPosition;
				savedTarget = dragTarget;
				dragTarget = null;
				constainX = false;
				constainY = false;
				constainZ = true;
				StartDrag();
				ApplyAdjustedDragUnconstrained_(new Vector3(0f, 0f, -0.18f));
			}
		}

		private void SendDragEventToFSM(string eventString)
		{
			FsmVariables globalVariables = FsmVariables.GlobalVariables;
			FsmGameObject fsmGameObject = globalVariables.GetFsmGameObject("DraggingItem");
			fsmGameObject.Value = base.gameObject;
			Logging.Log(string.Format("SendDragEventToFSM(): {0} - {1}", base.name, eventString));
			PlayMakerFSM.BroadcastEvent(eventString);
		}

		private void StopDragOffShelf()
		{
			if (!onShelf)
			{
				onShelf = true;
				Logging.Log(string.Format("StopDragOffShelf(): {0}", base.name));
				base.transform.localPosition = savedShelfLocalPosition;
				constainX = false;
				constainY = true;
				constainZ = true;
				dragTarget = savedTarget;
			}
		}

		public override void OnDrag(Vector2 delta)
		{
			if (onShelf)
			{
				Vector2 vector = dragCurrentScreenPos - dragStartScreenPos;
				if (Mathf.Abs(vector.y) > dragOffShelfThresholdYMin && Mathf.Abs(vector.x) < dragOffShelfThresholdXMax)
				{
					StartDragOffShelf();
					SendDragEventToFSM("DraggingItem");
					isDragging = true;
					return;
				}
			}
			else if (!isDragging)
			{
				SendDragEventToFSM("DraggingItem");
			}
			base.OnDrag(delta);
		}

		public override void OnPress(bool isPressed)
		{
			Logging.Log(string.Format("BathItem.OnPress( {1} ): {0}", base.name, isPressed));
			base.OnPress(isPressed);
			if (isPressed || onShelf)
			{
				return;
			}
			GameObject gameObject = DoDrop();
			if (gameObject != null)
			{
				SendDragEventToFSM("DroppedItem");
				savedParent = base.transform.parent;
				base.transform.parent = gameObject.transform;
				return;
			}
			SendDragEventToFSM("DiscardedItem");
			if ((bool)savedParent)
			{
				base.transform.parent = savedParent;
				savedParent = null;
			}
			StopDragOffShelf();
		}
	}
}
