using System;
using Relentless;
using UnityEngine;

namespace Furby.Scanner
{
	public class LegacyFurbyDialog : CommonPanel
	{
		[SerializeField]
		private string m_BuyFurbyURL;

		[SerializeField]
		private string m_LegacyAppURL;

		public override Type EventType
		{
			get
			{
				return typeof(ScannerEvents);
			}
		}

		public void OnClickNoFurby()
		{
			FurbyGlobals.ScreenSwitcher.SwitchScreen("Dashboard");
			SetDisabled(0.25f);
		}

		public void OnClickBuy()
		{
			GameEventRouter.SendEvent(LegacyFurbyEvents.ClickedBuyFurby);
			Application.OpenURL(m_BuyFurbyURL);
			FurbyGlobals.ScreenSwitcher.BackScreen();
			SetDisabled(0.25f);
		}

		public void OnClickLegacyApp()
		{
			GameEventRouter.SendEvent(LegacyFurbyEvents.ClickedDownloadLegacyApp);
			Application.OpenURL(m_LegacyAppURL);
			FurbyGlobals.ScreenSwitcher.BackScreen();
			SetDisabled(0.25f);
		}

		protected override void OnToggleWidgets(bool enabled)
		{
			base.OnToggleWidgets(enabled);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.enabled = enabled;
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if ((ScannerEvents)(object)enumValue == ScannerEvents.OldFurbyFound)
			{
				SetEnabled(true);
			}
		}
	}
}
