using Relentless;
using UnityEngine;

namespace Furby
{
	public class ApplyLanguage : MonoBehaviour
	{
		public Locale m_Locale;

		public GameObject m_GUITick;

		private LanguageSelectPopulator m_ParentPopulator;

		public void Initialise(Locale locale, LanguageSelectPopulator parentPopulator)
		{
			m_Locale = locale;
			m_ParentPopulator = parentPopulator;
			Refresh();
		}

		public void Refresh()
		{
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale == m_Locale)
			{
				m_GUITick.SetActive(true);
			}
			else
			{
				m_GUITick.SetActive(false);
			}
		}

		public void OnClick()
		{
			SimpleLanguageChooser.SetLocale(m_Locale);
			m_ParentPopulator.Refresh();
			m_ParentPopulator.ProhibitInput();
			Invoke("ReloadScene", 1f);
		}

		private void ReloadScene()
		{
			Application.LoadLevel(Application.loadedLevelName);
			m_ParentPopulator.AllowInput();
		}
	}
}
