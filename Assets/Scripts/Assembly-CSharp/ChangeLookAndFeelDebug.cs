using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

public class ChangeLookAndFeelDebug : MonoBehaviour
{
	private GameEventSubscription m_appLookAndFeelSubscription;

	private void Awake()
	{
		m_appLookAndFeelSubscription = new GameEventSubscription(OnGameThemeGUI, DebugPanelEvent.DrawElementRequested);
	}

	private void OnDestroy()
	{
		m_appLookAndFeelSubscription.Dispose();
	}

	private void OnGameThemeGUI(Enum enumValue, GameObject gObj, params object[] parameters)
	{
		if (DebugPanel.StartSection("App Theme"))
		{
			IEnumerable<AppLookAndFeel> source = Enum.GetValues(typeof(AppLookAndFeel)).Cast<AppLookAndFeel>();
			foreach (AppLookAndFeel item in source.Where((AppLookAndFeel theme) => GUILayout.Button(theme.ToString())))
			{
				StartCoroutine(StartChangeAfterTime(2f, item));
			}
		}
		DebugPanel.EndSection();
	}

	private IEnumerator StartChangeAfterTime(float time, AppLookAndFeel theme)
	{
		yield return new WaitForSeconds(time);
		ChangeLookAndFeelManager changeThemeManager = GetComponent<ChangeLookAndFeelManager>();
		changeThemeManager.Change(theme);
		Singleton<GameDataStoreObject>.Instance.Data.AppLookAndFeel = theme;
	}
}
