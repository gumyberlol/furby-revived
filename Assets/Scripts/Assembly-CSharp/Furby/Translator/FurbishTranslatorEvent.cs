using Relentless;

namespace Furby.Translator
{
	[GameEventEnum]
	public enum FurbishTranslatorEvent
	{
		TranslatorOpened = 0,
		TranslatorClosed = 1,
		Synchronized = 2,
		Translated = 3,
		ConnectionFailed = 4,
		ConnectionRetry = 5,
		ConnectionAbort = 6
	}
}
