public class PurchaseEvent
{
	public PurchasableItem PurchasedItem { get; private set; }

	public string Receipt { get; private set; }

	internal PurchaseEvent(PurchasableItem purchasedItem, string receipt)
	{
		PurchasedItem = purchasedItem;
		Receipt = receipt;
	}
}
