using UnityEngine;

namespace Furby
{
	public class EnsureEnoughFurbucks : MonoBehaviour
	{
		[SerializeField]
		private FlowStage m_flowStage;

		[SerializeField]
		private int m_bonus;

		private void Start()
		{
			if (FurbyGlobals.Player.FlowStage == m_flowStage)
			{
				FurbyGlobals.Wallet.Balance = Mathf.Max(m_bonus, FurbyGlobals.Wallet.Balance);
			}
		}
	}
}
