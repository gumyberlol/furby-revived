using Relentless;
using UnityEngine;

namespace Furby
{
	[AddComponentMenu("Furby/SpsBackToScreen")]
	public class SpsBackToScreen : RelentlessMonoBehaviour
	{
		public delegate void GoingBackHandler(LevelReference l);

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference TargetScreen;

		public event GoingBackHandler GoingBack;

		private void OnClick()
		{
			if (this.GoingBack != null)
			{
				this.GoingBack(TargetScreen);
			}
			FurbyGlobals.ScreenSwitcher.BackToScreen(TargetScreen.SceneName);
		}
	}
}
