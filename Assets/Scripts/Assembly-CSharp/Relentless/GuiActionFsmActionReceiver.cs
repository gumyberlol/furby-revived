using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class GuiActionFsmActionReceiver : RelentlessMonoBehaviour
	{
		private PlayMakerFSM m_playmakerFsm;

		[HideInInspector]
		public List<GUIScreenManager> ScreenManagers { get; set; }

		private void Awake()
		{
			m_playmakerFsm = base.gameObject.GetComponent<PlayMakerFSM>();
			if (m_playmakerFsm == null)
			{
				Logging.LogError("Failed to find a FSM on this gameobject");
			}
		}

		public void ReceiveAction(string actionName)
		{
			if (m_playmakerFsm != null)
			{
				m_playmakerFsm.SendEvent(actionName);
			}
			else
			{
				Logging.LogError("Failed to find a FSM on this gameobject");
			}
		}
	}
}
