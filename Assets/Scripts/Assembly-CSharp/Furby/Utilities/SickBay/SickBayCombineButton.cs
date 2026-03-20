using Relentless;

namespace Furby.Utilities.SickBay
{
	public class SickBayCombineButton : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			GameEventRouter.SendEvent(SickBayEvent.CombineIngredientsButtonClicked, null, null);
		}
	}
}
