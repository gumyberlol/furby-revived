namespace Furby.FSM
{
	public class FSMeventOnPress : FSMeventTarget
	{
		public string eventPressedName = "Pressed";

		public string eventReleasedName = "Released";

		private void OnPress(bool isPressed)
		{
			if (isPressed)
			{
				SendFSMEvent(eventPressedName);
			}
			else
			{
				SendFSMEvent(eventReleasedName);
			}
		}
	}
}
