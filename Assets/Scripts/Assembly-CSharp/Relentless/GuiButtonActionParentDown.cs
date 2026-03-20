namespace Relentless
{
	public class GuiButtonActionParentDown : RelentlessMonoBehaviour
	{
		public string Action;

		private void OnClick()
		{
			base.transform.parent.gameObject.BroadcastMessage(Action);
		}
	}
}
