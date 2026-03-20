using UnityEngine;

namespace Relentless
{
	public class GuiTextureFullscreen : RelentlessMonoBehaviour
	{
		private void OnEnable()
		{
			GUITexture component = GetComponent<GUITexture>();
			if ((bool)component)
			{
				component.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
				base.transform.localScale = Vector3.zero;
			}
		}
	}
}
