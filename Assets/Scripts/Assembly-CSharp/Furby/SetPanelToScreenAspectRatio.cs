using Relentless;
using UnityEngine;

namespace Furby
{
	[ExecuteInEditMode]
	public class SetPanelToScreenAspectRatio : MonoBehaviour
	{
		public enum Mode
		{
			AdjustToScreen = 0,
			AdjustToCamera = 1
		}

		public int MinimumWidth = 576;

		public int MaximumWidth = 768;

		public Mode AdjustmentMode;

		private void Start()
		{
			AdjustPanelClipRange();
		}

		private void AdjustPanelClipRange()
		{
			UIPanel component = GetComponent<UIPanel>();
			if (!(component != null))
			{
				return;
			}
			float num = 1f;
			switch (AdjustmentMode)
			{
			case Mode.AdjustToScreen:
				num = (float)Screen.width / (float)Screen.height;
				break;
			case Mode.AdjustToCamera:
			{
				Camera camera = base.gameObject.GetComponentInParents<Camera>();
				if (camera == null)
				{
					camera = Camera.main;
				}
				num = (float)Screen.width * camera.rect.width / ((float)Screen.height * camera.rect.height);
				break;
			}
			}
			int num2 = Mathf.Clamp((int)(component.clipRange.w * num), MinimumWidth, MaximumWidth);
			if ((float)num2 != component.clipRange.z)
			{
				component.clipRange = new Vector4(component.clipRange.x, component.clipRange.y, num2, component.clipRange.w);
			}
		}
	}
}
