using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	public class GuiActionRouter : MonoBehaviour
	{
		public PlayMakerFSM Receiver;

		private void NamedTextEvent(string key)
		{
			if (Receiver != null)
			{
				Receiver.SendEvent(key);
			}
		}

		public void GuiButtonUp(string buttonName)
		{
			if (Receiver != null)
			{
				Receiver.SendEvent(buttonName);
			}
		}

		public void GuiDragInput(DragData data)
		{
			if (Receiver != null)
			{
				FsmVector3 fsmVector = Receiver.FsmVariables.GetFsmVector3("GuiDragInput");
				if (fsmVector != null)
				{
					fsmVector.Value = data.Position;
					Receiver.SendEvent(data.EventName);
				}
			}
		}

		public void Awake()
		{
			if (Receiver == null)
			{
				Receiver = base.gameObject.GetComponent<PlayMakerFSM>();
			}
			if (Receiver != null)
			{
				List<GUIScreenManager> list = new List<GUIScreenManager>();
				base.gameObject.GetComponentsInChildrenIncludeInactive(list);
				GuiActionFsmActionReceiver component = Receiver.GetComponent<GuiActionFsmActionReceiver>();
				if (component != null)
				{
					component.ScreenManagers = list;
				}
				else
				{
					Logging.LogError("No GUIActionFSMActionReceiver on target FSM");
				}
			}
			else
			{
				Logging.LogError("No FSM for named text events");
			}
		}
	}
}
