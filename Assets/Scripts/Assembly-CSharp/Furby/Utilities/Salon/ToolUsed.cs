using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class ToolUsed : MonoBehaviour
	{
		public PlayMakerFSM m_GameStateMachine;

		public UISprite selected;

		private void OnClick()
		{
			m_GameStateMachine.SendEvent("ToolUsed");
			Logging.Log("Used");
		}
	}
}
