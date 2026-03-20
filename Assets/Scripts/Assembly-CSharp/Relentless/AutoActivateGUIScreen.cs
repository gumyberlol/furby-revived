using HutongGames.PlayMaker;

namespace Relentless
{
	[ActionCategory("Relentless")]
	public class AutoActivateGUIScreen : ActivateGUIScreen
	{
		public bool MakeExclusive;

		public override void OnEnter()
		{
			base.OnEnter();
			ActivateNow(true);
			Finish();
		}
	}
}
