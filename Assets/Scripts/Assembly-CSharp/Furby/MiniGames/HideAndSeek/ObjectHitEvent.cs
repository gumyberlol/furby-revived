using Relentless;

namespace Furby.MiniGames.HideAndSeek
{
	public class ObjectHitEvent : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			if (base.gameObject.tag == "special")
			{
				base.gameObject.SendGameEvent(HideAndSeekGameEvent.BallonPoppedSpecial);
			}
			else
			{
				base.gameObject.SendGameEvent(HideAndSeekGameEvent.BalloonPopped);
			}
		}
	}
}
