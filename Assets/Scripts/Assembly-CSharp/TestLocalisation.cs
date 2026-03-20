using NUnit.Framework;
using Relentless;

public class TestLocalisation : TestScript
{
	private Localisation m_localisation;

	public TestMethodState TestSingleton(TestMethodState testMethod)
	{
		m_localisation = Singleton<Localisation>.Instance;
		Assert.NotNull(m_localisation);
		return testMethod.Complete();
	}

	public TestMethodState TestPreLocaleSet(TestMethodState testMethod)
	{
		Assert.AreEqual(m_localisation.CurrentLocale, Locale.PRL_Pre_Locale);
		return testMethod.Complete();
	}

	public TestMethodState TestPreLocaleGetPrelocaleString(TestMethodState testMethod)
	{
		StringAssert.IsMatch(m_localisation.GetText("PRELOCALE_TEXT_Furby"), "Furby");
		return testMethod.Complete();
	}

	public TestMethodState TestPreLocaleHasString(TestMethodState testMethod)
	{
		Assert.IsTrue(m_localisation.HasText("PRELOCALE_TEXT_Furby"));
		Assert.IsFalse(m_localisation.HasText("MENU_OPTION_PLAYGAME"));
		return testMethod.Complete();
	}

	public TestMethodState TestHasLocale(TestMethodState testMethod)
	{
		Assert.IsFalse(m_localisation.HasLocale(Locale.PRT_Portugal));
		Assert.IsTrue(m_localisation.HasLocale(Locale.USA_USA));
		return testMethod.Complete();
	}

	public TestMethodState TestLocaleSet(TestMethodState testMethod)
	{
		m_localisation.SetLocale(Locale.USA_USA);
		Assert.AreEqual(m_localisation.CurrentLocale, Locale.USA_USA);
		return testMethod.Complete();
	}

	public TestMethodState TestPostLocaleGetPrelocaleString(TestMethodState testMethod)
	{
		StringAssert.IsMatch(m_localisation.GetText("PRELOCALE_TEXT_Furby"), "Furby");
		return testMethod.Complete();
	}

	public TestMethodState TestPostLocaleGetUSAString(TestMethodState testMethod)
	{
		m_localisation.SetLocale(Locale.USA_USA);
		StringAssert.IsMatch(m_localisation.GetText("MENU_OPTION_PLAYGAME"), "Play");
		return testMethod.Complete();
	}

	public TestMethodState TestPostLocaleGetFranceString(TestMethodState testMethod)
	{
		m_localisation.SetLocale(Locale.FRA_France);
		StringAssert.IsMatch(m_localisation.GetText("MENU_OPTION_PLAYGAME"), "Jouer");
		return testMethod.Complete();
	}
}
