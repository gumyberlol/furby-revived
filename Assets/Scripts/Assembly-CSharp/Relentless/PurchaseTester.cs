using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class PurchaseTester : MonoBehaviour
	{
		public List<string> m_Items;

		private List<string> m_Responses = new List<string>();

		private bool m_GoodToGo;

		private IEnumerator Start()
		{
			if (!Debug.isDebugBuild)
			{
				Object.Destroy(base.gameObject);
			}
			Object.DontDestroyOnLoad(base.gameObject);
			yield return null;
		}

		private void PurchaseComplete_ReceiptValidated(string itemID)
		{
			m_Responses.Add("SUCCESS, RECEIPT VALIDATED:" + itemID);
		}

		private void PurchaseComplete_ReceiptNotValidated(string itemID)
		{
			m_Responses.Add("SUCCESS, NOT VALIDATED:" + itemID);
		}

		private void PurchaseComplete_Unresolved(string itemID)
		{
			m_Responses.Add("COULDNT VALIDATE: " + itemID);
		}

		private void PurchaseFailed(string itemID)
		{
			m_Responses.Add("PURCHASE FAILED: " + itemID);
		}

		private void PurchaseCancelled(string itemID)
		{
			m_Responses.Add("CANCELLED:" + itemID);
		}
	}
}
