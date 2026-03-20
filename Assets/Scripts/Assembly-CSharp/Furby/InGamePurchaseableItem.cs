using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public abstract class InGamePurchaseableItem : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_lockedHierarchyToEnable;

		[SerializeField]
		private UILabel m_priceLabel;

		[SerializeField]
		private UISprite m_itemSprite;

		private bool m_isLocked;

		private bool m_isDisabled;

		public abstract void OnClickAlreadyPurchased();

		public abstract int GetFurbucksCost();

		public abstract string GetItemName();

		public abstract void Purchase();

		public abstract bool ShouldUseAfterPurchase();

		public void OnClick()
		{
			if (!m_isDisabled)
			{
				if (!m_isLocked)
				{
					OnClickAlreadyPurchased();
				}
				else if (FurbyGlobals.Wallet.Balance >= GetFurbucksCost())
				{
					GameEventRouter.SendEvent(InGamePurchaseableEvent.PurchaseButtonClickedEnoughFurbucks, null, GetFurbucksCost(), GetItemName());
					GameEventRouter.AddDelegateForEnums(PurchaseEventHandler, InGamePurchaseableEvent.PurchaseCancelled, InGamePurchaseableEvent.PurchaseConfirmed);
				}
				else
				{
					GameEventRouter.SendEvent(InGamePurchaseableEvent.PurchaseButtonClickedNotEnoughFurbucks, null, GetItemName(), GetItemName());
				}
			}
		}

		private void PurchaseEventHandler(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			InGamePurchaseableEvent inGamePurchaseableEvent = (InGamePurchaseableEvent)(object)eventType;
			if (inGamePurchaseableEvent == InGamePurchaseableEvent.PurchaseConfirmed)
			{
				FurbyGlobals.Wallet.Balance -= GetFurbucksCost();
				Purchase();
				SetLocked(false);
				if (ShouldUseAfterPurchase())
				{
					OnClickAlreadyPurchased();
				}
				GameEventRouter.SendEvent(VirtualItemPurchase.InGamePurchaseable, null, this);
			}
			GameEventRouter.RemoveDelegateForEnums(PurchaseEventHandler, InGamePurchaseableEvent.PurchaseCancelled, InGamePurchaseableEvent.PurchaseConfirmed);
		}

		public UISprite GetItemSprite()
		{
			return m_itemSprite;
		}

		public void SynchroniseVisualisation()
		{
			float num = 1f;
			if (m_isDisabled | m_isLocked)
			{
				num *= 0.5f;
			}
			m_priceLabel.text = GetFurbucksCost().ToString();
			m_lockedHierarchyToEnable.SetActive(m_isLocked);
			m_itemSprite.alpha = num;
		}

		public void SetLocked(bool isLocked)
		{
			m_isLocked = isLocked;
			SynchroniseVisualisation();
		}

		public void SetDisabled(bool disabled)
		{
			m_isDisabled = disabled;
			SynchroniseVisualisation();
		}
	}
}
