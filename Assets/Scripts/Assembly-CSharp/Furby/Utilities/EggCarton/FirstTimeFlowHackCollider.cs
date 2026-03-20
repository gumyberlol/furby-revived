using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.EggCarton
{
	public class FirstTimeFlowHackCollider : MonoBehaviour
	{
		[SerializeField]
		private CartonEggGrid m_eggCarton;

		[SerializeField]
		private GameObject m_hatchDialogCancelButton;

		[SerializeField]
		private TutorialInstructionBox m_instructionBox;

		private GameEventSubscription m_sub;

		public void Start()
		{
			m_sub = new GameEventSubscription(OnCancelHatch, CartonGameEvent.EggDialogGenericDecline);
		}

		public void OnClick()
		{
			SelectEgg componentInChildren = m_eggCarton.GetComponentInChildren<SelectEgg>();
			if (componentInChildren != null)
			{
				componentInChildren.OnClick();
			}
		}

		private void OnCancelHatch(Enum eventType, GameObject obj, params object[] parameters)
		{
			if (eventType.Equals(CartonGameEvent.EggDialogGenericDecline) && obj == m_hatchDialogCancelButton)
			{
				m_instructionBox.DisplayPause = 0f;
				base.gameObject.SendGameEvent(FlowDialog.EggCarton_TouchEgg);
			}
		}
	}
}
