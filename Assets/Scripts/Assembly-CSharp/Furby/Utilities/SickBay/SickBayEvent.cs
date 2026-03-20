using Relentless;

namespace Furby.Utilities.SickBay
{
	[GameEventEnum]
	public enum SickBayEvent
	{
		CureSectionStarted = 0,
		IngredientClicked = 1,
		IngredientAdded = 2,
		IngredientRemoved = 3,
		IngredientSlotsBecameFull = 4,
		IngredientSlotsBecameNotFull = 5,
		CombineIngredientsButtonClicked = 6,
		CombineIngredientsStarted = 7,
		CombineIngredientsFinished = 8,
		FurbyBoredOfWaitingForCure = 9,
		CureGiven = 10,
		CureBounced = 11,
		CureFlicked = 12,
		CureTimedOut = 13,
		FurbyReactingToItem = 14,
		FurbyBeingCured = 15,
		FurbyReactingToEffectsOfCure = 16,
		CommunicationsError = 17,
		NoFurby = 18,
		DiagnoseButtonClicked = 19,
		DiagnosisSectionStarted = 20,
		FurbyWasntSickBeforeCureGiven = 21,
		DiagnosisSick = 22,
		DiagnosisWell = 23,
		CureButtonClicked = 24,
		IngredientAddedCola = 25,
		IngredientAddedHoney = 26,
		IngredientAddedHotWater = 27,
		IngredientAddedIceCube = 28,
		IngredientAddedMints = 29,
		FurbyWasCured = 30,
		MedicalCaseMovedOnScreen = 31,
		MedicalCaseMovedOffScreen = 32,
		CrossVFXStarted = 33,
		CrossVFXStopped = 34,
		ErrorDialogOK = 35,
		ResetButtonClicked = 36
	}
}
