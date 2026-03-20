using Relentless;

namespace Furby.Utilities.Pantry
{
	public class PantryResetButton : RelentlessMonoBehaviour
	{
		public delegate void PantryResetButtonDelegate();

		public event PantryResetButtonDelegate OnReset;

		private void OnClick()
		{
			GameEventRouter.SendEvent(PantryEvent.ResetButtonClicked, null, null);
			if (this.OnReset != null)
			{
				this.OnReset();
			}
		}
	}
}
