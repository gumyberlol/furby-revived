using Furby;
using Relentless;
using UnityEngine;

[RequireComponent(typeof(DisableInputOnLayer))]
public class ChangeLookAndFeelManager : Singleton<ChangeLookAndFeelManager>
{
	public GameObject m_popupLayer;

	public GameObject m_helpLayer;

	public GameObject m_vfxChangeTheme;

	private void Start()
	{
		GameData data = Singleton<GameDataStoreObject>.Instance.Data;
		AppLookAndFeel appLookAndFeel = data.AppLookAndFeel;
		bool canPlayAppChangeAnimation = data.CanPlayAppChangeAnimation;
		if (appLookAndFeel != AppLookAndFeel.Normal && canPlayAppChangeAnimation)
		{
			data.CanPlayAppChangeAnimation = false;
			Change(appLookAndFeel);
		}
		else if (appLookAndFeel == AppLookAndFeel.Crystal)
		{
			base.gameObject.SendGameEvent(CrystalSparkleVFXTriggers.VFXCrystalSparkleVFXPresent);
		}
	}

	public void Change(AppLookAndFeel changeTheme)
	{
		GameData data = Singleton<GameDataStoreObject>.Instance.Data;
		data.CanPlayAppChangeAnimation = changeTheme == AppLookAndFeel.Normal;
		if (changeTheme == AppLookAndFeel.Normal)
		{
			FurbyGlobals.ScreenSwitcher.SwitchScreen("Dashboard", false);
		}
		else
		{
			ChangeTheme();
		}
	}

	private void ChangeTheme()
	{
		DisableInputOnLayer component = GetComponent<DisableInputOnLayer>();
		component.enabled = true;
		m_popupLayer.SetActive(false);
		m_helpLayer.SetActive(false);
		base.gameObject.SendGameEvent(DashboardGameEvent.StartedCrystalTransition);
		m_vfxChangeTheme.SetActive(true);
	}
}
