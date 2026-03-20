using Relentless;
using UnityEngine;

namespace Furby
{
	public class SpsOrNoFurbyEvent : RelentlessMonoBehaviour
	{
		public bool GoBack;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference TargetScreen;

		public bool AutoSwitch;

		public float DelayTime;

		public bool IncludeInHistory = true;

		[SerializeField]
		private SerialisableEnum m_noFurbyEvent;

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
			if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				SwitchScreens();
			}
			else
			{
				GameEventRouter.SendEvent(m_noFurbyEvent.Value);
			}
		}

		public void ManuallyInvoke()
		{
			OnClick();
		}

		private void SwitchScreens()
		{
			if (base.enabled)
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
}
