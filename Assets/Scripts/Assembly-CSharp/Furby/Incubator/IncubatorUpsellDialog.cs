using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorUpsellDialog : IncubatorConfirmDialog
	{
		[SerializeField]
		private IncubatorPurchaseDialog m_PurchaseDialog;

		[SerializeField]
		private GameObject m_ErrorDialog;

		public override IEnumerator ShowModal()
		{
			yield return StartCoroutine(base.ShowModal());
			if (LastEvent == SharedGuiEvents.DialogAccept)
			{
				bool haveNetwork = SetupNetworking.IsInternetReady;
				bool itemsInStore = SingletonInstance<RsStoreMediator>.Instance.AreFastForwardsAvailable();
				if (haveNetwork && itemsInStore)
				{
					yield return StartCoroutine(m_PurchaseDialog.ShowModal());
				}
				else if ((bool)m_ErrorDialog)
				{
					m_ErrorDialog.SetActive(true);
					WaitForGameEvent waiter = new WaitForGameEvent();
					yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
					m_ErrorDialog.SetActive(false);
				}
			}
		}
	}
}
