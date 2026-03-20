using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class NGUILocalizerFromMultiple : RelentlessMonoBehaviour
	{
		public List<string> m_LocalisedStringKeys = new List<string>();

		public NGUILocalizerSelectionAlgorithm m_Algorithm;

		public void OnEnable()
		{
			UpdateUI();
		}

		public void Start()
		{
			UpdateUI();
		}

		public string SelectLocalizedStringKey()
		{
			if (m_LocalisedStringKeys.Count > 0 && m_Algorithm == NGUILocalizerSelectionAlgorithm.TrueRandom)
			{
				int index = Random.Range(0, m_LocalisedStringKeys.Count);
				return m_LocalisedStringKeys[index];
			}
			return string.Empty;
		}

		public void UpdateUI()
		{
			string text = SelectLocalizedStringKey();
			if (text == string.Empty)
			{
				Logging.LogError(string.Format("Empty named text key on object {1}", base.name));
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				Logging.LogError(string.Format("Empty named text key : \"{0}\" on object {1}", text, base.name));
			}
			if (!Singleton<Localisation>.Exists)
			{
				return;
			}
			List<UILabel> list = new List<UILabel>();
			base.gameObject.GetComponentsInChildrenIncludeInactive(list);
			string text2 = Singleton<Localisation>.Instance.GetText(text);
			foreach (UILabel item in list)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					item.text = text2;
				}
				else
				{
					item.text = "*" + text + "*";
				}
			}
		}
	}
}
