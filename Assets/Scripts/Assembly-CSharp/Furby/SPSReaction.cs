using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class SPSReaction : GameEventReaction
	{
		public bool GoBack;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference TargetScreen;

		public bool IncludeInHistory = true;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			if (GoBack)
			{
				FurbyGlobals.ScreenSwitcher.BackScreen();
			}
			else
			{
				FurbyGlobals.ScreenSwitcher.SwitchScreen(TargetScreen, IncludeInHistory);
			}
		}
	}
}
