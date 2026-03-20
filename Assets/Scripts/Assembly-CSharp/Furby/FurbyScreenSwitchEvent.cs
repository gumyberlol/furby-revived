using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum FurbyScreenSwitchEvent
	{
		StartFadeDown = 0,
		StartLevelLoad = 1,
		StartAssetBundlesLoad = 2,
		StartFadeup = 3,
		EndFadeup = 4
	}
}
