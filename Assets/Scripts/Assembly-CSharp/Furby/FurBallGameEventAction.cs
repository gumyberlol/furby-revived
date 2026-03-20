using HutongGames.PlayMaker;
using Relentless;

namespace Furby
{
	[ActionCategory("Furby")]
	[Tooltip("FurBall Game Event")]
	public class FurBallGameEventAction : FsmStateAction
	{
		public FurBallGameEvent Action;

		public override void OnEnter()
		{
			GameEventRouter.SendEvent(Action);
			Finish();
		}
	}
}
