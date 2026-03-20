using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Scanner
{
	public class ScanScreenFailureDialog : GameEventConsumer<SharedGuiEvents>
	{
		[SerializeField]
		protected GameObject m_DialogPanel;

		[SerializeField]
		protected ScanScreenHelpDialog m_HelpDialog;

		public IEnumerator ShowModal()
		{
			SharedGuiEvents[] dialogEvents = new SharedGuiEvents[3]
			{
				SharedGuiEvents.DialogAccept,
				SharedGuiEvents.DialogCancel,
				SharedGuiEvents.DialogShow
			};
			while (true)
			{
				foreach (SharedGuiEvents? i in Await(true, dialogEvents))
				{
					yield return i;
				}
				switch (LastEvent.Value)
				{
				case SharedGuiEvents.DialogAccept:
					GameEventRouter.SendEvent(ScannerEvents.RequiresInitialScan);
					yield break;
				case SharedGuiEvents.DialogShow:
					m_DialogPanel.SetActive(false);
					yield return StartCoroutine(m_HelpDialog.ShowModal());
					break;
				default:
					GameEventRouter.SendEvent(ScannerEvents.ScanningCancelled);
					yield break;
				}
				m_DialogPanel.SetActive(true);
			}
		}

		public new void OnEnable()
		{
			base.OnEnable();
			StartCoroutine(ShowModal());
		}
	}
}
