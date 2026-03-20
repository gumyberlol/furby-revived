using UnityEngine;

namespace Relentless
{
	public class LocalisationHelper
	{
		public static string GetLanguageIdentifier(SystemLanguage language, string cultureCode)
		{
			string languageCodeFor = GetLanguageCodeFor(language);
			return GetLanguageIdentifier(languageCodeFor, cultureCode);
		}

		public static string GetLanguageIdentifier(string languageCode, string cultureCode)
		{
			if (!string.IsNullOrEmpty(cultureCode))
			{
				return string.Format("{0}-{1}", languageCode, cultureCode);
			}
			return languageCode;
		}

		public static string GetLanguageCodeFor(SystemLanguage language)
		{
			switch (language)
			{
			case SystemLanguage.Italian:
				return "it";
			case SystemLanguage.German:
				return "de";
			case SystemLanguage.French:
				return "fr";
			case SystemLanguage.Swedish:
				return "sv";
			case SystemLanguage.Spanish:
				return "es";
			case SystemLanguage.Chinese:
				return "zh";
			case SystemLanguage.Danish:
				return "da";
			case SystemLanguage.Dutch:
				return "nl";
			case SystemLanguage.Finnish:
				return "fi";
			case SystemLanguage.Norwegian:
				return "no";
			case SystemLanguage.Portuguese:
				return "pt";
			case SystemLanguage.Japanese:
				return "jp";
			case SystemLanguage.Korean:
				return "ko";
			case SystemLanguage.Russian:
				return "ru";
			case SystemLanguage.Polish:
				return "pl";
			case SystemLanguage.Turkish:
				return "tr";
			case SystemLanguage.Czech:
				return "cs";
			case SystemLanguage.Catalan:
				return "ca";
			case SystemLanguage.Estonian:
				return "et";
			case SystemLanguage.Faroese:
				return "fo";
			case SystemLanguage.Greek:
				return "el";
			case SystemLanguage.Hebrew:
				return "he";
			case SystemLanguage.Hugarian:
				return "hu";
			case SystemLanguage.Icelandic:
				return "is";
			case SystemLanguage.Indonesian:
				return "id";
			case SystemLanguage.Latvian:
				return "lv";
			case SystemLanguage.Lithuanian:
				return "lt";
			case SystemLanguage.Romanian:
				return "ro";
			case SystemLanguage.SerboCroatian:
				return "sh";
			case SystemLanguage.Slovak:
				return "sk";
			case SystemLanguage.Slovenian:
				return "sl";
			case SystemLanguage.Thai:
				return "th";
			case SystemLanguage.Ukrainian:
				return "uk";
			case SystemLanguage.Vietnamese:
				return "vi";
			case SystemLanguage.Afrikaans:
				return "af";
			case SystemLanguage.Arabic:
				return "ar";
			case SystemLanguage.Basque:
				return "eu";
			case SystemLanguage.Belarusian:
				return "be";
			case SystemLanguage.Bulgarian:
				return "bg";
			default:
				return "en";
			}
		}

		public static string GetCurrencyCodeFor(SystemLanguage language)
		{
			switch (language)
			{
			case SystemLanguage.Catalan:
			case SystemLanguage.Danish:
			case SystemLanguage.Dutch:
			case SystemLanguage.Estonian:
			case SystemLanguage.Finnish:
			case SystemLanguage.French:
			case SystemLanguage.German:
			case SystemLanguage.Greek:
			case SystemLanguage.Hugarian:
			case SystemLanguage.Italian:
			case SystemLanguage.Latvian:
			case SystemLanguage.Norwegian:
			case SystemLanguage.Polish:
			case SystemLanguage.Portuguese:
			case SystemLanguage.SerboCroatian:
			case SystemLanguage.Slovak:
			case SystemLanguage.Slovenian:
			case SystemLanguage.Spanish:
			case SystemLanguage.Swedish:
				return "EUR";
			case SystemLanguage.Japanese:
				return "JPY";
			default:
				return "USD";
			}
		}

		public static string GetCurrencyCodeFor(string countryCode)
		{
			switch (countryCode.Truncate(2))
			{
			case "GB":
				return "GBP";
			case "JP":
				return "JPY";
			case "AD":
			case "AT":
			case "BE":
			case "CY":
			case "EE":
			case "FI":
			case "FR":
			case "DE":
			case "GR":
			case "IE":
			case "IT":
			case "LV":
			case "LU":
			case "MT":
			case "MC":
			case "ME":
			case "NL":
			case "PT":
			case "SM":
			case "SK":
			case "SI":
			case "ES":
			case "VA":
				return "EUR";
			default:
				return "USD";
			}
		}

		public static string GetCurrencySymbolFromCode(string currencyCode)
		{
			string empty = string.Empty;
			switch (currencyCode)
			{
			case "USD":
			case "CAD":
			case "HKD":
			case "AUD":
				return "$";
			case "EUR":
				return "€";
			case "SEK":
			case "NOK":
			case "DKK":
				return "kr";
			case "KRW":
				return "₩";
			case "JPY":
				return "¥";
			case "GBP":
				return "£";
			default:
				return string.Empty;
			}
		}
	}
}
