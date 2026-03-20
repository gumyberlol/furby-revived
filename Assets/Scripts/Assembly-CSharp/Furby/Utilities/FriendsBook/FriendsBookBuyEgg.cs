using Relentless;

namespace Furby.Utilities.FriendsBook
{
	public class FriendsBookBuyEgg : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			SpriteFurbyDisplay component = GetComponent<SpriteFurbyDisplay>();
			if (component.IsCrystalLocked)
			{
				base.gameObject.SendGameEvent(SharedGuiEvents.CrystalThemeLocked);
			}
			else if (!component.Unlocked)
			{
				base.gameObject.SendGameEvent(FriendsBookEvent.ClickOnLockedFurby, component.Index, component.Furby);
			}
			else
			{
				base.gameObject.SendGameEvent(FriendsBookEvent.ClickOnFurby, component.Index, component.Furby);
			}
		}
	}
}
