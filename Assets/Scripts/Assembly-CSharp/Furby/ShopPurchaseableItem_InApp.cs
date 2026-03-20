using UnityEngine;

namespace Furby
{
	public abstract class ShopPurchaseableItem_InApp : MonoBehaviour
	{
		public abstract string GetItemName();

		public abstract bool IsPurchased();

		public abstract string GetSpriteName();

		public abstract void Purchase();

		public abstract UIAtlas GetAtlas();

		public abstract string GetRealPrice();

		public virtual bool AttemptPurchaseIsValid()
		{
			return true;
		}
	}
}
