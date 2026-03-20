using Relentless;

namespace Furby.Utilities.SickBay
{
	public class SickBayCureButton : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			GameEventRouter.SendEvent(SickBayEvent.CureButtonClicked, null, null);
		}
	}
}
