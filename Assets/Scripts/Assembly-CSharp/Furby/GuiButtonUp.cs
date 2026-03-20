using UnityEngine;

namespace Furby
{
	public class GuiButtonUp : MonoBehaviour
	{
		private void OnClick()
		{
			SendMessageUpwards("GuiButtonUp", base.gameObject.name);
		}
	}
}
