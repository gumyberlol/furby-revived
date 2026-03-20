using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.EggCarton
{
	public class GetEggTutorialFlow : MonoBehaviour
	{
		[SerializeField]
		private UIDraggablePanel m_draggablePanel;

		[SerializeField]
		private TutorialInstructionBox m_secondPopup;

		[SerializeField]
		private float m_movePanelX;

		[SerializeField]
		private GameObject m_eggButton;

		private GameEventSubscription m_eventSubs;

		private void Start()
		{
			UIPanel component = m_draggablePanel.GetComponent<UIPanel>();
			component.transform.localPosition += new Vector3(m_movePanelX, 0f, 0f);
			UnityEngine.Object.Destroy(m_draggablePanel);
			m_eventSubs = new GameEventSubscription(typeof(SharedGuiEvents), EnableSecond);
		}

		private void EnableSecond(Enum eventType, GameObject obj, params object[] parameters)
		{
			if (eventType.Equals(SharedGuiEvents.ButtonUp) && obj == m_eggButton)
			{
				base.gameObject.SendGameEvent(FlowDialog.Friendsbook_GetEgg);
				m_eventSubs.Dispose();
				m_eventSubs = null;
			}
		}
	}
}
