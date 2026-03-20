using UnityEngine;

namespace Relentless
{
	public class GuiForwardOnDragUp : MonoBehaviour
	{
		public bool StopAfterFirstObjectFound;

		private void OnDrag(Vector2 delta)
		{
			UIDraggablePanel uIDraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (uIDraggablePanel != null)
			{
				uIDraggablePanel.Drag(delta);
			}
		}
	}
}
