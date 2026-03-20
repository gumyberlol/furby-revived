using Relentless;

namespace Furby.Utilities.SickBay
{
	public class SickBayResetButton : RelentlessMonoBehaviour
	{
		public delegate void SickBayResetButtonDelegate();

		public event SickBayResetButtonDelegate OnReset;

		private void OnClick()
		{
			GameEventRouter.SendEvent(SickBayEvent.ResetButtonClicked, null, null);
			if (this.OnReset != null)
			{
				this.OnReset();
			}
		}
	}
}
