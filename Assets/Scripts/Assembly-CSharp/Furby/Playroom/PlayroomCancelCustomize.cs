using Relentless;

namespace Furby.Playroom
{
	public class PlayroomCancelCustomize : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			Singleton<PlayroomModeController>.Instance.SetInInteractionMode();
			GameEventRouter.SendEvent(PlayroomGameEvent.EnterPlayroom);
			GameEventRouter.SendEvent(PlayroomGameEvent.Customization_ChangesReverted);
			GameEventRouter.SendEvent(PlayroomGameEvent.Customization_SequenceEnded);
			Singleton<PlayroomIdlingController>.Instance.Enable = true;
		}
	}
}
