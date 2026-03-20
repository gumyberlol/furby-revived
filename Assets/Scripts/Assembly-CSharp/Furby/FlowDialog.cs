using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FlowDialog
	{
		Dash_PressHoseButton = 0,
		Hose_TurnOnHose = 16,
		Hose_TurnOffHose = 18,
		Hose_GoBackToDash = 20,
		Dash_SelectEgg = 32,
		EggCarton_TouchEgg = 48,
		Incubator_Rub = 64,
		Incubator_Furby = 66,
		Incubator_NoFurby = 68,
		Dash_BabyFeeling = 80,
		Dash_GoBackBaby = 82,
		Dash_GoToPlayroom = 84,
		Playroom_FeedBaby = 96,
		Playroom_CleanBaby = 98,
		Neighbourhood_Info = 112,
		Dash_GetAnotherEgg = 128,
		EggCarton_GetNextEgg = 144,
		EggCarton_EggFromFriend = 160,
		Friendsbook_GetEgg = 176,
		Incubator_ReturnToDash = 192,
		Dash_ExplainSlang = 208,
		Hide_Dialog = 4096
	}
}
