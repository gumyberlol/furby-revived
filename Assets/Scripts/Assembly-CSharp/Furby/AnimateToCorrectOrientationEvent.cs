using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum AnimateToCorrectOrientationEvent
	{
		RotationStarted = 0,
		RotationEnded = 1,
		NoRotationRequired = 2
	}
}
