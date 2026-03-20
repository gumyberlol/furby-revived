using HutongGames.PlayMaker;

namespace Furby
{
	[ActionCategory("SpsMenus")]
	[Tooltip("Go back to the previous screen")]
	public class SpsGoBack : FsmStateAction
	{
		public override void OnEnter()
		{
			FurbyGlobals.ScreenSwitcher.BackScreen();
			Finish();
		}
	}
}
