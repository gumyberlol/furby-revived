using Relentless;

namespace Furby.FSM
{
	public class FSMeventOnClick : FSMeventTarget
	{
		public string eventClickedName = "Clicked";

		private void OnClick()
		{
			Logging.Log("OnClick");
			SendFSMEvent(eventClickedName);
		}
	}
}
