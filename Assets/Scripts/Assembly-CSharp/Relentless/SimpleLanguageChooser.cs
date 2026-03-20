using UnityEngine;

namespace Relentless
{
	[RequireComponent(typeof(Localisation))]
	public class SimpleLanguageChooser : RelentlessMonoBehaviour
	{
		[SerializeField]
		private Locale m_defaultLocale;

		[SerializeField]
		private LocaleMap[] m_localeMaps;

		public LocaleMap[] LocaleMaps
		{
			get
			{
				return m_localeMaps;
			}
		}

		private void Start()
		{
			Localisation component = GetComponent<Localisation>();
			Locale locale = Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale;
			if (locale == Locale.PRL_Pre_Locale)
			{
				SystemLanguage systemLanguage = Application.systemLanguage;
				locale = GetAppropriateLocale(systemLanguage, locale);
				if (!component.HasLocale(locale))
				{
					locale = m_defaultLocale;
				}
			}
			SetLocale(locale);
		}

		private Locale GetAppropriateLocale(SystemLanguage currentSystemLanguage, Locale localeInGlobalSettings)
		{
			LocaleMap[] localeMaps = m_localeMaps;
			foreach (LocaleMap localeMap in localeMaps)
			{
				SystemLanguage[] languages = localeMap.languages;
				foreach (SystemLanguage systemLanguage in languages)
				{
					if (systemLanguage == currentSystemLanguage)
					{
						localeInGlobalSettings = localeMap.locale;
						return localeInGlobalSettings;
					}
				}
			}
			return localeInGlobalSettings;
		}

		public static void SetLocale(Locale locale)
		{
			Localisation instance = Singleton<Localisation>.Instance;
			if (instance.HasLocale(locale))
			{
				Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale = locale;
				Singleton<GameDataStoreObject>.Instance.Save();
				instance.SetLocale(locale);
			}
		}
	}
}
