using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomEndCustomize : RelentlessMonoBehaviour
	{
		public PlayroomHintController m_HintController;

		private void Start()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
		}

		private void OnClick()
		{
			Singleton<PlayroomCustomizationModeController>.Instance.RefreshCustomizationMode(PlayroomCustomizationMode.None);
			Singleton<PlayroomModeController>.Instance.SetInInteractionMode();
			m_HintController.ConfirmChanges.Disable();
			bool flag = false;
			GameObject gameObject = GameObject.Find("LowStatusVisualization");
			if ((bool)gameObject)
			{
				LowStatusVisualization component = gameObject.GetComponent<LowStatusVisualization>();
				if (component != null && component.Suspended)
				{
					flag = true;
				}
			}
			GameEventRouter.SendEvent(PlayroomGameEvent.EnterPlayroom);
			if (flag)
			{
				GameEventRouter.SendEvent(PlayroomGameEvent.Customization_ChangesAppliedButStillInLowStatus);
			}
			else
			{
				Singleton<PlayroomIdlingController>.Instance.Enable = true;
				GameEventRouter.SendEvent(PlayroomGameEvent.Customization_ChangesApplied);
			}
			GameEventRouter.SendEvent(PlayroomGameEvent.Customization_SequenceEnded);
		}
	}
}
