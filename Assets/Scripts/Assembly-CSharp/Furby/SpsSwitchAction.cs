using Relentless;
using UnityEngine;

namespace Furby
{
	[AddComponentMenu("Furby/SpsSwitchAction")]
	public class SpsSwitchAction : RelentlessMonoBehaviour
	{
		public bool GoBack;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference TargetScreen;

		public bool AutoSwitch;

		public float DelayTime;

		public bool IncludeInHistory = true;

		private void OnEnable()
		{
			if (AutoSwitch)
			{
				Invoke(SwitchScreens, DelayTime);
			}
		}

		private void OnDisable()
		{
			if (AutoSwitch)
			{
				CancelInvoke(SwitchScreens);
			}
		}

		private void OnClick()
		{
			SwitchScreens();
		}

		public void ManuallyInvoke()
		{
			OnClick();
		}

		private void SwitchScreens()
		{
			if (base.enabled && !SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled)
			{
				SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled = true;
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
}
