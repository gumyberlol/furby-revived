using Relentless;
using UnityEngine;

public class LanguageChangeButton : MonoBehaviour
{
	[SerializeField]
	private Locale m_Language;

	[SerializeField]
	private GameObject m_GUITick;

	private void Awake()
	{
		if (Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale == m_Language)
		{
			m_GUITick.SetActive(true);
		}
		else
		{
			m_GUITick.SetActive(false);
		}
	}

	private void OnClick()
	{
		SimpleLanguageChooser.SetLocale(m_Language);
		Application.LoadLevelAsync("AdvancedSettingsScene");
	}
}
