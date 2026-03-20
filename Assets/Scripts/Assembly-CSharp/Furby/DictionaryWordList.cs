using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DictionaryWordList : ScriptableObject
	{
		[Serializable]
		public class DictionaryWord
		{
			[SerializeField]
			private string m_nativeReference;

			[SerializeField]
			private string m_doodleReference;

			[SerializeField]
			private string m_audioReference;

			[SerializeField]
			private bool m_isFurbish = true;

			private string FormatText(bool translated)
			{
				string format = "DICTIONARY_{0}_{1}";
				string arg = m_nativeReference.Replace("!", "_X").Replace("?", "_Q").ToUpper()
					.Replace(" ", "_")
					.Replace("-", "_");
				if (translated)
				{
					if (m_isFurbish)
					{
						return string.Format(format, "DEF_ENG", arg);
					}
					return string.Format(format, "DEF_FUR", arg);
				}
				if (m_isFurbish)
				{
					return string.Format(format, "WORD_FUR", arg);
				}
				return string.Format(format, "WORD_ENG", arg);
			}

			public string GetNativeNamedText()
			{
				return Singleton<Localisation>.Instance.GetText(FormatText(false));
			}

			public string GetNativeNamedTextWithNonAlphabetCharactersStriped()
			{
				string text = Singleton<Localisation>.Instance.GetText(FormatText(false));
				char[] array = text.ToCharArray();
				array = Array.FindAll(array, (char c) => char.IsLetter(c) || char.IsWhiteSpace(c));
				return new string(array);
			}

			public string GetTranslatedNamedText()
			{
				return Singleton<Localisation>.Instance.GetText(FormatText(true));
			}

			public string GetDoodleName()
			{
				return m_doodleReference;
			}

			public string GetAudioName()
			{
				return m_audioReference;
			}
		}

		[SerializeField]
		[EasyEditArray]
		private DictionaryWord[] m_furbishWords;

		[EasyEditArray]
		[SerializeField]
		private DictionaryWord[] m_englishWords;

		private bool m_isSorted;

		private List<DictionaryWord> m_englishWordsSorted;

		private List<DictionaryWord> m_furbishWordsSorted;

		private int DictionaryWordComparison(DictionaryWord a, DictionaryWord b)
		{
			return string.Compare(a.GetNativeNamedTextWithNonAlphabetCharactersStriped(), b.GetNativeNamedTextWithNonAlphabetCharactersStriped());
		}

		private void Sort()
		{
			if (!m_isSorted)
			{
				m_englishWordsSorted = new List<DictionaryWord>(m_englishWords);
				m_englishWordsSorted.Sort(DictionaryWordComparison);
				m_furbishWordsSorted = new List<DictionaryWord>(m_furbishWords);
				m_furbishWordsSorted.Sort(DictionaryWordComparison);
				m_isSorted = true;
			}
		}

		public DictionaryWord GetWord(int index, DictionaryLogic.DictionaryMode dictionaryMode)
		{
			Sort();
			DictionaryWord result = null;
			switch (dictionaryMode)
			{
			case DictionaryLogic.DictionaryMode.EnglishToFurbish:
				result = m_englishWordsSorted[index];
				break;
			case DictionaryLogic.DictionaryMode.FurbishToEnglish:
				result = m_furbishWordsSorted[index];
				break;
			}
			return result;
		}

		public int GetNumWords(DictionaryLogic.DictionaryMode dictionaryMode)
		{
			Sort();
			int result = 0;
			switch (dictionaryMode)
			{
			case DictionaryLogic.DictionaryMode.EnglishToFurbish:
				result = m_englishWordsSorted.Count;
				break;
			case DictionaryLogic.DictionaryMode.FurbishToEnglish:
				result = m_furbishWordsSorted.Count;
				break;
			}
			return result;
		}
	}
}
