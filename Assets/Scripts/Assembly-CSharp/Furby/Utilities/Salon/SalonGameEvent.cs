using Relentless;

namespace Furby.Utilities.Salon
{
	[GameEventEnum]
	public enum SalonGameEvent
	{
		Enter = 0,
		Exit = 1,
		Pause = 2,
		ShowerRoom = 3,
		StylingRoom = 4,
		ToolSelection = 5,
		ToolConfirmation = 6,
		SalonRubOn = 7,
		SalonRubOff = 8,
		SalonLotionBegin = 9,
		SalonLotion = 10,
		SalonLotionComplete = 11,
		ShowerOn = 12,
		ShowerOff = 13,
		SalonScrubBegin = 14,
		SalonScrub = 15,
		ScrubComplete = 16,
		StyleBegin = 17,
		StyleOn = 18,
		StyleOff = 19,
		StyleAmount = 20,
		StyleComplete = 21,
		StylePreVerdict01 = 22,
		StylePreVerdict02 = 23,
		StylePreVerdict03 = 24,
		StylePreVerdict04 = 25,
		StyleReactionDisklike = 26,
		StyleReactionNeutral = 27,
		StyleReactionLike = 28,
		StyleReactionLove = 29,
		StyleReactionPerfect = 30,
		StyleReactionComplete = 31,
		TransitionToStylingRoomStart = 32,
		StoolBoing = 33
	}
}
