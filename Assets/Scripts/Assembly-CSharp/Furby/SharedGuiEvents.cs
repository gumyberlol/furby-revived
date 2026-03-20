using Relentless;

namespace Furby
{
	[GameEventEnum]
	public enum SharedGuiEvents
	{
		Back = 0,
		Pause = 1,
		Resume = 2,
		Restart = 3,
		Quit = 4,
		Submit = 5,
		Cancel = 6,
		Playroom = 7,
		Dashboard = 8,
		ButtonDown = 9,
		ButtonUp = 10,
		MessageBoxAppear = 11,
		MessageBoxDisappear = 12,
		FuckbucksEarned = 13,
		FurbucksSpent = 14,
		FurbucksSmallEarn = 15,
		FurbucksLargeEarn = 16,
		DialogAccept = 17,
		DialogCancel = 18,
		DialogShow = 19,
		DialogHide = 20,
		ButtonRelease = 21,
		CrystalThemeLocked = 22,
		CrystalThemeBuy = 23,
		IncubateAttentionPoint = 256,
		DialogWasShown = 512,
		DialogWasHidden = 513,
		PrivacyPolicy_ViewMobilePolicy = 768,
		PrivacyPolicy_ViewUserAgreement = 769
	}
}
