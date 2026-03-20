using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Scanner
{
	public class ScanScreenHelpDialog : GameEventConsumer<SharedGuiEvents>
	{
		[SerializeField]
		protected GameObject m_DialogPanel;

		public IEnumerator ShowModal()
		{
			m_DialogPanel.SetActive(true);
			foreach (SharedGuiEvents? i in Await(true, SharedGuiEvents.DialogAccept))
			{
				yield return i;
			}
			m_DialogPanel.SetActive(false);
		}
	}
}
