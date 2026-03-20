using HutongGames.PlayMaker;

namespace Relentless
{
	[ActionCategory("Relentless")]
	public class AutoHideGUIScreen : ActivateGUIScreen
	{
		public WhenToHide WhenToHide = WhenToHide.HideOnExit;

		public override void OnEnter()
		{
			base.OnEnter();
			if (WhenToHide == WhenToHide.HideOnEnter)
			{
				ActivateNow(false);
			}
		}

		public override void OnExit()
		{
			if (WhenToHide == WhenToHide.HideOnExit)
			{
				ActivateNow(false);
			}
			base.OnExit();
		}
	}
}
