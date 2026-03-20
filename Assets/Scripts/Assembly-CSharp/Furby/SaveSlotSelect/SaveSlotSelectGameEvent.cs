using Relentless;

namespace Furby.SaveSlotSelect
{
	[GameEventEnum]
	public enum SaveSlotSelectGameEvent
	{
		SelectSlot0 = 0,
		SelectSlot1 = 1,
		SelectSlot2 = 2,
		SelectNextFreeSlot = 3,
		DeleteSlot0 = 4,
		DeleteSlot1 = 5,
		DeleteSlot2 = 6,
		ShowDeleteDialog = 7,
		ConfirmDelete = 8
	}
}
