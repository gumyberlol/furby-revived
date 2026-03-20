using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public abstract class ShopPurchaseableItem : MonoBehaviour
	{
		public virtual int GetFurbucksCost()
		{
			return 0;
		}

		public abstract string GetItemName();

		public abstract bool IsPurchased();

		public abstract string GetSpriteName();

		public abstract void Purchase();

		public abstract UIAtlas GetAtlas();

		public virtual bool AttemptPurchaseIsValid()
		{
			return true;
		}

		private IEnumerator OnClick()
		{
			if (IsPurchased() || !AttemptPurchaseIsValid())
			{
				yield break;
			}
			if (GetFurbucksCost() <= FurbyGlobals.Wallet.Balance)
			{
				GameEventRouter.SendEvent(ShopGameEvents.ShopItemSelectedPurchase, null, GetFurbucksCost(), GetItemName());
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForAnyEventOfType(typeof(ShopGameEvents)));
				if (waiter.ReturnedEvent.Equals(ShopGameEvents.ShopItemConfirmPurchase))
				{
					FurbyGlobals.Wallet.Balance -= GetFurbucksCost();
					Purchase();
					GameEventRouter.SendEvent(VirtualItemPurchase.BabyItem, base.gameObject, this);
					GameEventRouter.SendEvent(ShopGameEvents.ShopItemsUpdated);
				}
			}
			else
			{
				GameEventRouter.SendEvent(ShopGameEvents.ShopItemSelectedTooCostly, null, GetItemName(), GetItemName());
			}
		}
	}
}
