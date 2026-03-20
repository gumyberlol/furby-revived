using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum DictionaryGameEvent
	{
		SwitchTranslationMode = 0,
		NextWord = 1,
		PreviousWord = 2,
		SelectLetter = 3,
		SayWord = 4,
		SelectNewLetter = 5,
		SwitchTranslationToEnglishToFurbish = 6,
		SwitchTranslationToFurbishToEnglish = 7
	}
}
