namespace Relentless
{
	public enum StoreMediatorStatus
	{
		NotStarted = 0,
		WaitingForInventory = 1,
		WaitingForBillingSystem = 2,
		AbleToMediatePurchases = 3,
		NoInventory_Halted = 4
	}
}
