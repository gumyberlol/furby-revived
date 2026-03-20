using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Relentless
{
	public class Localisation : Singleton<Localisation>
	{
		[GameEventEnum]
		public enum LocalisationEvent
		{
			RequestSubstitution = 0,
			SetSubstitution = 1
		}

		[Serializable]
		public class LocaleData
		{
			public Locale locale;

			public NamedTextTable textTable;
		}

		public class UnsupportedLocaleException : RelentlessException
		{
			public UnsupportedLocaleException(string message)
				: base(message)
			{
			}
		}

		private LocaleData m_currentLocaleData;

		public LocaleData[] m_supportedLocales;

		public bool m_startInFirstLanguage;

		public NamedTextTable m_preLocalNamedText;

		private GameEventSubscription m_subsSubst;

		private Dictionary<string, string> m_substitutions;

		public Locale CurrentLocale
		{
			get
			{
				if (m_currentLocaleData == null)
				{
					return Locale.PRL_Pre_Locale;
				}
				return m_currentLocaleData.locale;
			}
		}

		public event EventHandler LocaleSet;

		public bool HasLocale(Locale locale)
		{
			return m_supportedLocales.Any((LocaleData data) => data.locale == locale);
		}

		public void SetLocale(Locale locale)
		{
			m_currentLocaleData = m_supportedLocales.FirstOrDefault((LocaleData data) => data.locale == locale);
			if (m_currentLocaleData == null)
			{
				throw new UnsupportedLocaleException(string.Format("Locale {0} is not supported.", locale));
			}
			if (this.LocaleSet != null)
			{
				this.LocaleSet(this, EventArgs.Empty);
			}
		}

		private string DoSubstitutions(string inString)
		{
			Regex regex = new Regex("({[^{}]+})");
			string[] array = regex.Split(inString);
			string text = string.Empty;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2.StartsWith("{") && text2.EndsWith("}"))
				{
					GameEventRouter.SendEvent(LocalisationEvent.RequestSubstitution, base.gameObject, text2);
					string value;
					if (m_substitutions.TryGetValue(text2, out value))
					{
						text += value;
						continue;
					}
				}
				text += text2;
			}
			return text;
		}

		public string GetText(string key)
		{
			if (m_currentLocaleData == null && m_startInFirstLanguage)
			{
				SetLocale(m_supportedLocales[0].locale);
			}
			string returnValue;
			if (m_currentLocaleData != null && m_currentLocaleData.textTable.TryGetString(key, out returnValue))
			{
				return DoSubstitutions(returnValue);
			}
			if (string.IsNullOrEmpty(key))
			{
				return "<unset>";
			}
			if (m_preLocalNamedText == null)
			{
				return key;
			}
			if (m_preLocalNamedText.TryGetString(key, out returnValue))
			{
				return DoSubstitutions(returnValue);
			}
			return key;
		}

		public bool HasText(string key)
		{
			if (m_currentLocaleData != null && m_currentLocaleData.textTable.HasString(key))
			{
				return true;
			}
			if (m_preLocalNamedText.HasString(key))
			{
				return true;
			}
			return false;
		}

		public void SetSubstitution(string key, string value)
		{
			m_substitutions[key] = value;
		}

		public void OnSetSubstitution(Enum substEvent, GameObject gObj, params object[] parameters)
		{
			if (substEvent.Equals(LocalisationEvent.SetSubstitution))
			{
				string key = parameters[0] as string;
				string value = parameters[1] as string;
				SetSubstitution(key, value);
			}
		}

		private void Awake()
		{
			m_subsSubst = new GameEventSubscription(OnSetSubstitution, LocalisationEvent.SetSubstitution);
			m_substitutions = new Dictionary<string, string>();
		}

		public override void OnDestroy()
		{
			m_subsSubst.Dispose();
			m_subsSubst = null;
			base.OnDestroy();
		}
	}
}
