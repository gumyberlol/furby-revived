using System;
using UnityEngine;

namespace Furby
{
	public class FurbuckDisplayPanel : CommonPanel
	{
		[SerializeField]
		private UILabel m_label;

		public override Type EventType
		{
			get
			{
				return typeof(SharedGuiEvents);
			}
		}

		public new void Start()
		{
			base.Start();
			UpdateBalance();
			FurbucksWallet wallet = FurbyGlobals.Wallet;
			wallet.BalanceChangedCallback = (FurbucksWallet.BalanceChangedDelegate)Delegate.Combine(wallet.BalanceChangedCallback, new FurbucksWallet.BalanceChangedDelegate(BalanceChanged));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ((bool)FurbyGlobals.Wallet)
			{
				FurbucksWallet wallet = FurbyGlobals.Wallet;
				wallet.BalanceChangedCallback = (FurbucksWallet.BalanceChangedDelegate)Delegate.Remove(wallet.BalanceChangedCallback, new FurbucksWallet.BalanceChangedDelegate(BalanceChanged));
			}
		}

		public void BalanceChanged(FurbucksWallet wallet, int balanceDelta)
		{
			UpdateBalance();
		}

		public void UpdateBalance()
		{
			m_label.text = string.Format("{0}", FurbyGlobals.Wallet.Balance);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((SharedGuiEvents)(object)enumValue)
			{
			case SharedGuiEvents.Pause:
				SetEnabled(false);
				break;
			case SharedGuiEvents.Restart:
				SetEnabled(true);
				break;
			case SharedGuiEvents.Resume:
				SetEnabled(true);
				break;
			}
		}
	}
}
