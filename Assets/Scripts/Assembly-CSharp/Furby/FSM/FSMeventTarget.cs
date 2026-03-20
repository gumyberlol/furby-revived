using HutongGames.PlayMaker;
using UnityEngine;

namespace Furby.FSM
{
	public class FSMeventTarget : MonoBehaviour
	{
		public enum FSMTargetType
		{
			This = 0,
			FSM = 1
		}

		public FSMTargetType targetType;

		public GameObject targetObject;

		protected void SendFSMEvent(string eventName)
		{
			if (!(null == targetObject))
			{
				PlayMakerFSM component = targetObject.GetComponent<PlayMakerFSM>();
				if ((bool)component)
				{
					FsmVariables globalVariables = FsmVariables.GlobalVariables;
					FsmGameObject fsmGameObject = globalVariables.GetFsmGameObject("EventOriginatingObject");
					fsmGameObject.Value = base.gameObject;
					component.SendEvent(eventName);
				}
			}
		}
	}
}
