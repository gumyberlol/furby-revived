using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("NGUI/Interaction/Center Grid")]
	public class UICenterGrid : MonoBehaviour
	{
		public void Reposition()
		{
			UIGrid component = GetComponent<UIGrid>();
			Transform transform = component.transform;
			Vector3 zero = Vector3.zero;
			if (component.arrangement == UIGrid.Arrangement.Horizontal)
			{
				zero.x = component.cellWidth * (float)(transform.childCount - 1);
				zero.y = 0f;
			}
			else
			{
				zero.y = component.cellHeight * (float)(transform.childCount - 1);
				zero.x = 0f;
			}
			transform.localPosition = zero * -0.5f;
		}

		private void Update()
		{
			Reposition();
		}
	}
}
