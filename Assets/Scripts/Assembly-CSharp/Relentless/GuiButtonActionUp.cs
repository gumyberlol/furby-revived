namespace Relentless
{
	public class GuiButtonActionUp : RelentlessMonoBehaviour
	{
		public string Action;

		private void OnClick()
		{
			SendMessageUpwards(Action);
		}
	}
}
