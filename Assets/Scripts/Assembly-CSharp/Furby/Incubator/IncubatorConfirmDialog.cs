using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorConfirmDialog : GameEventConsumer<SharedGuiEvents>
	{
		internal IncubatorDialogButton m_EventSender;

		[SerializeField]
		protected GameObject m_DialogPanel;

		public virtual IEnumerator AwaitInput()
		{
			m_EventSender = null;
			while (m_EventSender == null)
			{
				yield return null;
			}
		}

		public virtual IEnumerator ShowModal()
		{
			m_DialogPanel.SetActive(true);
			foreach (SharedGuiEvents? i in Await(true, SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel))
			{
				yield return i;
			}
			m_DialogPanel.SetActive(false);
		}
	}
}
