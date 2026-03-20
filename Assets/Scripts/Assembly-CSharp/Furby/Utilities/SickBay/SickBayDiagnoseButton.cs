using Relentless;

namespace Furby.Utilities.SickBay
{
	public class SickBayDiagnoseButton : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			GameEventRouter.SendEvent(SickBayEvent.DiagnoseButtonClicked, null, null);
		}
	}
}
