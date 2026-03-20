using HutongGames.PlayMaker;

namespace Furby
{
	[Tooltip("Switch to a different screen")]
	[ActionCategory("SpsMenus")]
	public class SpsSwitchScreen : FsmStateAction
	{
		public string TargetScreen;

		public bool StoreInHistory = true;

		public override void OnEnter()
		{
			FurbyGlobals.ScreenSwitcher.SwitchScreen(TargetScreen, StoreInHistory);
			Finish();
		}
	}
}
