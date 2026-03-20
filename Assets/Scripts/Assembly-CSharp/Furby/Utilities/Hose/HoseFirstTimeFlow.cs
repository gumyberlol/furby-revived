using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class HoseFirstTimeFlow : MonoBehaviour
	{
		[SerializeField]
		private PauseButtonPanel m_pauseButton;

		[SerializeField]
		private PauseMenuPanel m_pauseMenu;

		[SerializeField]
		private DigitalHoseAdapter m_hose;

		public void Start()
		{
			PauseButtonPanel.CancellationRequest waterOn = null;
			waterOn = delegate(HashSet<string> reasons)
			{
				reasons.Add("Showing first time help:  Turn water on.");
				HintWaterOn();
			};
			m_pauseButton.Pausing += waterOn;
			DigitalHoseAdapter.ChangeHandler onHoseOn = null;
			onHoseOn = delegate
			{
				m_pauseButton.Pausing -= waterOn;
				m_hose.SwitchedOn -= onHoseOn;
				AttachWaterOffListeners();
			};
			m_hose.SwitchedOn += onHoseOn;
		}

		private void HintWaterOn()
		{
			Logging.Log("First time flow; switch the water on.");
			base.gameObject.SendGameEvent(FlowDialog.Hose_TurnOnHose);
		}

		private void AttachWaterOffListeners()
		{
			PauseButtonPanel.CancellationRequest waterOff = null;
			waterOff = delegate(HashSet<string> reasons)
			{
				reasons.Add("Showing first time help:  Turn water off.");
				HintWaterOff();
			};
			m_pauseButton.Pausing += waterOff;
			DigitalHoseAdapter.ChangeHandler onHoseOff = null;
			onHoseOff = delegate
			{
				m_pauseButton.Pausing -= waterOff;
				m_hose.SwitchedOff -= onHoseOff;
				AttachBackToDash();
			};
			m_hose.SwitchedOff += onHoseOff;
		}

		private void HintWaterOff()
		{
			Logging.Log("First time flow; switch the water off.");
			base.gameObject.SendGameEvent(FlowDialog.Hose_TurnOffHose);
		}

		private void AttachBackToDash()
		{
			Logging.Log("Go back to the dash, noob.");
			base.gameObject.SendGameEvent(FlowDialog.Hose_GoBackToDash);
			m_pauseMenu.RestartRequested += delegate(HashSet<string> reasons)
			{
				reasons.Add("First time flow telling you to go back to dash.");
			};
			m_pauseButton.Enabled += delegate
			{
				Logging.Log("(showing panel again)");
				base.gameObject.SendGameEvent(FlowDialog.Hose_GoBackToDash);
			};
		}
	}
}
