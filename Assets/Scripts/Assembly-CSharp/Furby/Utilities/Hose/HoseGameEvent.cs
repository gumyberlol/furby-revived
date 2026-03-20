using Relentless;

namespace Furby.Utilities.Hose
{
	[GameEventEnum]
	public enum HoseGameEvent
	{
		TemperatureAmount = 0,
		PressureAmount = 1,
		TemperatureFullOn = 2,
		TemeratureFullOff = 3,
		PressureFullOn = 4,
		PressureFullOff = 5,
		TouchTemperatureControl = 6,
		TouchPressureControl = 7,
		TemperatureIncrement = 8,
		TemperatureDecrement = 9,
		PressureIncrement = 10,
		PressureDecrement = 11,
		PuddleFillStart = 12,
		PuddleFillComplete = 13,
		PuddleEmptyStart = 14,
		PuddleEmptyComplete = 15,
		ItemDropped = 16,
		ItemArrivalComplete = 17,
		ItemDepartureStarted = 18,
		ItemDepartureComplete = 19,
		NoFurbyFirstTimeGiveXP = 20,
		FurbyIsRelieved = 21
	}
}
