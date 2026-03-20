using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorFastForwardDialog : IncubatorConfirmDialog
	{
		[SerializeField]
		private IncubatorLogic m_GameLogic;

		public override IEnumerator ShowModal()
		{
			yield return StartCoroutine(base.ShowModal());
			if (LastEvent == SharedGuiEvents.DialogAccept)
			{
				m_GameLogic.OnFastForwardActivate();
				GameEventRouter.SendEvent(IncubatorGameEvent.Incubator_FastForward_Activated);
			}
		}
	}
}
