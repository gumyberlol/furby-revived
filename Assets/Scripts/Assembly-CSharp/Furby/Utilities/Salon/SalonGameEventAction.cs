using HutongGames.PlayMaker;
using Relentless;

namespace Furby.Utilities.Salon
{
	[Tooltip("Sends a Salon Game Event Action")]
	[ActionCategory("Furby")]
	public class SalonGameEventAction : FsmStateAction
	{
		public SalonGameEvent Action;

		public override void OnEnter()
		{
			GameEventRouter.SendEvent(Action, null);
			Finish();
		}
	}
}
