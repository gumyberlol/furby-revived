using UnityEngine;

namespace Relentless
{
	public class GuiButtonActionEntireScreen : RelentlessMonoBehaviour
	{
		public string Action;

		private void OnClick()
		{
			GUIScreen gUIScreen = HierarchyHelper.FindParent<GUIScreen>(base.transform);
			if (gUIScreen != null)
			{
				gUIScreen.gameObject.BroadcastMessage(Action, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
