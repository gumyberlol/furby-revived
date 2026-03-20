using Furby;
using UnityEngine;

public class ReloadDashboardOnLookAndFeelChange : MonoBehaviour
{
	private void OnEnable()
	{
		FurbyGlobals.ScreenSwitcher.SwitchScreen("Dashboard", false);
	}
}
