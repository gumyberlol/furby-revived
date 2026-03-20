using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class ToolConfirm : MonoBehaviour
	{
		public PlayMakerFSM m_GameStateMachine;

		private void OnClick()
		{
			m_GameStateMachine.SendEvent("Confirmed");
			base.gameObject.SendGameEvent(SalonGameEvent.ToolConfirmation);
		}
	}
}
