using Relentless;

namespace Furby.Utilities.Pantry
{
	[GameEventEnum]
	public enum PantryEvent
	{
		PantryUtilityStarted = 0,
		FoodItemClicked = 1,
		FoodItemSelected = 2,
		FurbyBoredOfWaitingForFood = 3,
		FoodGiven = 4,
		FoodBounced = 5,
		FoodFlicked = 6,
		FoodTimedOut = 7,
		FurbyReactingToFood = 8,
		FoodReturned = 9,
		CommunicationsError = 10,
		NoFurby = 11,
		ResetButtonClicked = 12,
		ErrorDialogOK = 13,
		FoodWasActuallyReceived = 14
	}
}
