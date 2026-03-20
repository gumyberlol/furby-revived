using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LanguageOnDebugPanel : MonoBehaviour
	{
		private GameEventSubscription m_sub;

		private SimpleLanguageChooser m_LanguageChooser;

		public void Start()
		{
			m_sub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
			GameObject gameObject = GameObject.Find("Localisation");
			if (gameObject != null)
			{
				m_LanguageChooser = gameObject.GetComponentInChildren<SimpleLanguageChooser>();
			}
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			float num = 220f;
			if (DebugPanel.StartSection("CountryCode"))
			{
				GUILayout.Label("[Status]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				GUILayout.BeginHorizontal();
				GUILayout.Label("Country Code: ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(num));
				GUILayout.Label(Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.Width(num));
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
			if (DebugPanel.StartSection("Localisation"))
			{
				GUILayout.Label("[Status]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				GUILayout.BeginHorizontal();
				GUILayout.Label("Device Language:  ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(num));
				GUILayout.Label(Application.systemLanguage.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.Width(num));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Current Locale:  ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(num));
				GUILayout.Label(Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.Width(num));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.Label("[Controls]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				GUILayout.Label("WARNING: Reloads the scene!", RelentlessGUIStyles.Style_Warning);
				GUILayout.Space(10f);
				GUILayout.Label("Select Language: ", RelentlessGUIStyles.Style_Normal);
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Nth", RelentlessGUIStyles.Style_Column, GUILayout.Width(num / 3f));
				GUILayout.Label("System.Language", RelentlessGUIStyles.Style_Column, GUILayout.Width(num));
				GUILayout.Space(5f);
				GUILayout.Label("Relentless.Locale", RelentlessGUIStyles.Style_Column, GUILayout.Width(num));
				GUILayout.EndHorizontal();
				int num2 = 0;
				LocaleMap[] localeMaps = m_LanguageChooser.LocaleMaps;
				foreach (LocaleMap localeMap in localeMaps)
				{
					string text = localeMap.languages[0].ToString();
					Locale locale = localeMap.locale;
					GUILayout.BeginHorizontal();
					GUILayout.Label(num2++.ToString(), RelentlessGUIStyles.Style_Normal, GUILayout.Width(num / 3f));
					if (GUILayout.Button(text.ToString(), GUILayout.Width(num)))
					{
						Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale = locale;
						Singleton<GameDataStoreObject>.Instance.Save();
						Singleton<Localisation>.Instance.SetLocale(locale);
						Application.LoadLevel(Application.loadedLevelName);
					}
					GUILayout.Space(5f);
					GUILayout.Label(locale.ToString(), RelentlessGUIStyles.Style_Normal, GUILayout.Width(num));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}

		public void OnDestroy()
		{
			m_sub.Dispose();
		}
	}
}
