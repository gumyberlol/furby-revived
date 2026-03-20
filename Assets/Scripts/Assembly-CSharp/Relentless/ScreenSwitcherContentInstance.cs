using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ScreenSwitcherContentInstance
	{
		public string m_Description = string.Empty;

		public string m_NamedTextKey;

		public GameObject m_HintObject;

		public bool HaveValidKey()
		{
			return m_NamedTextKey != null;
		}

		public string GetStringFromKey()
		{
			if (string.IsNullOrEmpty(m_NamedTextKey))
			{
				Logging.LogError("ScreenSwitcherContentInstance: Empty named text key : " + m_NamedTextKey);
				return null;
			}
			if (Singleton<Localisation>.Exists)
			{
				return Singleton<Localisation>.Instance.GetText(m_NamedTextKey);
			}
			Logging.LogError("ScreenSwitcherContentInstance: have named text key : " + m_NamedTextKey + " BUT NO LOCALIZATION SYSTEM!");
			return null;
		}

		public void ApplyTranslationToComponentLabels(UILabel targetLabel)
		{
			string stringFromKey = GetStringFromKey();
			if (!string.IsNullOrEmpty(stringFromKey))
			{
				targetLabel.text = stringFromKey;
			}
			else
			{
				targetLabel.text = "*" + stringFromKey + "*";
			}
		}
	}
}
