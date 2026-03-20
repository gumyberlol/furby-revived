using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DictionaryLogic : GameEventReceiver
	{
		public enum DictionaryMode
		{
			EnglishToFurbish = 0,
			FurbishToEnglish = 1
		}

		[SerializeField]
		private DictionaryWordList m_wordList;

		[SerializeField]
		private UILabel m_fromLabelDefinition;

		[SerializeField]
		private UILabel m_toLabelDefinition;

		[SerializeField]
		private UILabel m_modeHeader;

		[SerializeField]
		private UILabel m_fromLanguageHeader;

		[SerializeField]
		private UILabel m_toLanguageHeader;

		[SerializeField]
		private UITexture m_doodleSprite;

		[SerializeField]
		private UITexture m_letterSprite;

		[SerializeField]
		[NamedText]
		private string m_furbishToEnglishHeader;

		[SerializeField]
		[NamedText]
		private string m_englishToFurbishHeader;

		[SerializeField]
		[NamedText]
		private string m_englishHeader;

		[NamedText]
		[SerializeField]
		private string m_furbishHeader;

		[SerializeField]
		private float m_largeFontScale = 80f;

		[SerializeField]
		private float m_smallFontScale = 60f;

		[SerializeField]
		private int m_useSmallFontOverLength = 10;

		private DictionaryMode m_dictionaryMode;

		private int m_currentWordIndex;

		public override Type EventType
		{
			get
			{
				return typeof(DictionaryGameEvent);
			}
		}

		private void Start()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			RefreshView();
		}

		private void SetText(UILabel label, string text)
		{
			if (text.Length > m_useSmallFontOverLength)
			{
				label.transform.localScale = new Vector3(m_smallFontScale, m_smallFontScale, 1f);
			}
			else
			{
				label.transform.localScale = new Vector3(m_largeFontScale, m_largeFontScale, 1f);
			}
			label.text = text;
		}

		public void RefreshView()
		{
			DictionaryWordList.DictionaryWord word = m_wordList.GetWord(m_currentWordIndex, m_dictionaryMode);
			switch (m_dictionaryMode)
			{
			case DictionaryMode.EnglishToFurbish:
				m_modeHeader.text = Singleton<Localisation>.Instance.GetText(m_englishToFurbishHeader);
				m_fromLanguageHeader.text = Singleton<Localisation>.Instance.GetText(m_englishHeader);
				m_toLanguageHeader.text = Singleton<Localisation>.Instance.GetText(m_furbishHeader);
				SetText(m_fromLabelDefinition, word.GetNativeNamedText());
				SetText(m_toLabelDefinition, word.GetTranslatedNamedText());
				break;
			case DictionaryMode.FurbishToEnglish:
				m_modeHeader.text = Singleton<Localisation>.Instance.GetText(m_furbishToEnglishHeader);
				m_toLanguageHeader.text = Singleton<Localisation>.Instance.GetText(m_furbishHeader);
				m_fromLanguageHeader.text = Singleton<Localisation>.Instance.GetText(m_englishHeader);
				SetText(m_toLabelDefinition, word.GetNativeNamedText());
				SetText(m_fromLabelDefinition, word.GetTranslatedNamedText());
				break;
			}
			Texture2D texture2D = (Texture2D)Resources.Load("dictionary/doodles/" + word.GetDoodleName());
			if (texture2D != null)
			{
				m_doodleSprite.mainTexture = texture2D;
				m_doodleSprite.transform.localScale = new Vector3(texture2D.width, texture2D.height, 1f);
				m_doodleSprite.enabled = true;
			}
			else
			{
				m_doodleSprite.enabled = false;
			}
			Texture2D texture2D2 = (Texture2D)Resources.Load("dictionary/letters/" + word.GetNativeNamedTextWithNonAlphabetCharactersStriped()[0]);
			if (texture2D2 != null)
			{
				m_letterSprite.enabled = true;
				m_letterSprite.mainTexture = texture2D2;
				m_letterSprite.transform.localScale = new Vector3(texture2D2.width, texture2D2.height, 1f);
			}
			else
			{
				m_letterSprite.enabled = false;
			}
			Resources.UnloadUnusedAssets();
		}

		private void OnDestroy()
		{
			Resources.UnloadUnusedAssets();
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((DictionaryGameEvent)(object)enumValue)
			{
			case DictionaryGameEvent.NextWord:
				m_currentWordIndex++;
				if (m_currentWordIndex == m_wordList.GetNumWords(m_dictionaryMode))
				{
					m_currentWordIndex = 0;
				}
				break;
			case DictionaryGameEvent.PreviousWord:
				m_currentWordIndex--;
				if (m_currentWordIndex == -1)
				{
					m_currentWordIndex = m_wordList.GetNumWords(m_dictionaryMode) - 1;
				}
				break;
			case DictionaryGameEvent.SelectLetter:
			{
				int numWords2 = m_wordList.GetNumWords(m_dictionaryMode);
				for (int j = 0; j < numWords2; j++)
				{
					if (m_wordList.GetWord(j, m_dictionaryMode).GetNativeNamedTextWithNonAlphabetCharactersStriped()[0].ToString().ToUpper() == ((string)paramList[0]).ToUpper())
					{
						if (m_currentWordIndex != j)
						{
							m_currentWordIndex = j;
							GameEventRouter.SendEvent(DictionaryGameEvent.SelectNewLetter);
						}
						break;
					}
				}
				break;
			}
			case DictionaryGameEvent.SwitchTranslationMode:
			{
				DictionaryWordList.DictionaryWord word = m_wordList.GetWord(m_currentWordIndex, m_dictionaryMode);
				if (m_dictionaryMode == DictionaryMode.EnglishToFurbish)
				{
					m_dictionaryMode = DictionaryMode.FurbishToEnglish;
					GameEventRouter.SendEvent(DictionaryGameEvent.SwitchTranslationToFurbishToEnglish);
				}
				else
				{
					m_dictionaryMode = DictionaryMode.EnglishToFurbish;
					GameEventRouter.SendEvent(DictionaryGameEvent.SwitchTranslationToEnglishToFurbish);
				}
				m_currentWordIndex = 0;
				int numWords = m_wordList.GetNumWords(m_dictionaryMode);
				string nativeNamedText = word.GetNativeNamedText();
				string translatedNamedText = word.GetTranslatedNamedText();
				string doodleName = word.GetDoodleName();
				for (int i = 0; i < numWords; i++)
				{
					DictionaryWordList.DictionaryWord word2 = m_wordList.GetWord(i, m_dictionaryMode);
					string translatedNamedText2 = word2.GetTranslatedNamedText();
					string nativeNamedText2 = word2.GetNativeNamedText();
					if (!string.IsNullOrEmpty(doodleName) && doodleName == word2.GetDoodleName())
					{
						m_currentWordIndex = i;
						break;
					}
					if (nativeNamedText2.Equals(translatedNamedText, StringComparison.InvariantCultureIgnoreCase))
					{
						m_currentWordIndex = i;
						break;
					}
					if (translatedNamedText2.Equals(nativeNamedText, StringComparison.InvariantCultureIgnoreCase))
					{
						m_currentWordIndex = i;
						break;
					}
					if (translatedNamedText2.Contains(nativeNamedText) || nativeNamedText.Contains(translatedNamedText2))
					{
						m_currentWordIndex = i;
					}
					else if (nativeNamedText2.Contains(translatedNamedText) || translatedNamedText.Contains(nativeNamedText2))
					{
						m_currentWordIndex = i;
					}
				}
				break;
			}
			}
			RefreshView();
		}
	}
}
