using Relentless;

namespace Furby.Utilities.Blender
{
	[GameEventEnum]
	public enum BlenderEvent
	{
		UtilityOpened = 0,
		UtilityClosed = 1,
		IngredientAdded = 2,
		IngredientRemoved = 3,
		BlendButtonClicked = 4,
		DrinkButtonClicked = 5,
		UtensilClosed = 6,
		UtensilOpen = 7,
		UtensilRunning = 8,
		UtensilSlideOff = 9,
		UtensilSlideOn = 10,
		BabyIdle = 11,
		BabyJumpOn = 12,
		BabyJumpOff = 13,
		BabyDrink01 = 14,
		BabyDrink02 = 15,
		BabyReactionFart = 16,
		BabyReactionBurp = 17,
		BabyReactionSpicy = 18,
		BabyReactionSour = 19,
		BabyReactionVomit = 20,
		BabyReactionGeneric = 21,
		BabyVerdictBad = 22,
		BabyVerdictOkay = 23,
		BabyVerdictGood = 24,
		BabyVerdictGreat = 25,
		BabyVerdictPerfect = 26,
		FinishedBlending = 27
	}
}
