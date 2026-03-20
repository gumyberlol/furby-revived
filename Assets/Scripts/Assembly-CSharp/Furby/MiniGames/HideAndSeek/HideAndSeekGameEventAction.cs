using HutongGames.PlayMaker;
using Relentless;

namespace Furby.MiniGames.HideAndSeek
{
	[ActionCategory("Furby")]
	[Tooltip("Hide and Seek Game Event")]
	public class HideAndSeekGameEventAction : FsmStateAction
	{
		public HideAndSeekGameEvent Action;

		public override void OnEnter()
		{
			GameEventRouter.SendEvent(Action);
			Finish();
		}
	}
}
