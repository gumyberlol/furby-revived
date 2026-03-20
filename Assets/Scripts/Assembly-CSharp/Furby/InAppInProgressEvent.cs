using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum InAppInProgressEvent
	{
		ShowInProgressDialog_ProgressMode = 1,
		ShowInProgressDialog_ModalityMode = 2,
		ShowInProgressDialog_ConfirmMode = 3,
		HideInProgressDialog = 16,
		OKButtonPressed = 32,
		ExitButtonPressed = 33,
		UpdateContent = 48
	}
}
