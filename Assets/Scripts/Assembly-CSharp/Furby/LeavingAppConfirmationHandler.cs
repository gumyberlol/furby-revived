using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LeavingAppConfirmationHandler : MonoBehaviour
	{
		[SerializeField]
		public OnClickLaunchURL m_PayloadTarget;

		public void Start()
		{
			base.gameObject.SetLayerInChildren(base.transform.parent.gameObject.layer);
			StartCoroutine(WaitForCancelAndDestroy());
		}

		private IEnumerator WaitForCancelAndDestroy()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogCancel));
			Object.Destroy(base.gameObject);
		}
	}
}
