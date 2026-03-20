using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorDialogFlow : GameEventConsumer<IncubatorDialogEvent>
	{
		[SerializeField]
		private IncubatorUpsellDialog m_UpsellPurchaseDialog;

		[SerializeField]
		private IncubatorFastForwardDialog m_ConfirmDialog;

		[SerializeField]
		private IncubatorConfirmDialog m_UpsellDialog;

		public YieldInstruction Process()
		{
			IncubatorDialogEvent? incubatorDialogEvent = PumpEvents();
			if (incubatorDialogEvent.HasValue)
			{
				switch (incubatorDialogEvent.Value)
				{
				case IncubatorDialogEvent.Interruption_FastForward_Purchase:
					return StartCoroutine(RunDialog(m_UpsellPurchaseDialog.ShowModal()));
				case IncubatorDialogEvent.Interruption_FastForward_Activated:
					return StartCoroutine(RunDialog(m_ConfirmDialog.ShowModal()));
				case IncubatorDialogEvent.Interruption_FastForward_Upsell:
					return StartCoroutine(RunDialog(m_UpsellDialog.ShowModal()));
				}
			}
			return null;
		}

		public IEnumerator RunDialog(IEnumerator dialog)
		{
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Finished);
			while (dialog.MoveNext())
			{
				yield return dialog.Current;
			}
			GameEventRouter.SendEvent(IncubatorGameEvent.Timer_Started);
		}
	}
}
