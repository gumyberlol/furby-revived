using System;
using Furby.Scripts.FurMail;
using Relentless;
using UnityEngine;

public class FurMailMessageContents : MonoBehaviour
{
	[SerializeField]
	private UILabel m_MessageSender;

	[SerializeField]
	private UILabel m_MessageReceivedTime;

	[SerializeField]
	private UILabel m_MessageTitle;

	[SerializeField]
	private UILabel m_MessageBody;

	[SerializeField]
	private UISprite m_FurbyPortrait;

	[SerializeField]
	private UITiledSprite m_MessageBackground;

	private readonly string[] m_MonthStrings = new string[12]
	{
		"FURMAIL_MONTH_JANUARY", "FURMAIL_MONTH_FEBRUARY", "FURMAIL_MONTH_MARCH", "FURMAIL_MONTH_APRIL", "FURMAIL_MONTH_MAY", "FURMAIL_MONTH_JUNE", "FURMAIL_MONTH_JULY", "FURMAIL_MONTH_AUGUST", "FURMAIL_MONTH_SEPTEMBER", "FURMAIL_MONTH_OCTOBER",
		"FURMAIL_MONTH_NOVEMBER", "FURMAIL_MONTH_DECEMBER"
	};

	public void UpdateForNewMessage(FurMailMessage message, FurMailMediator.SenderData senderData)
	{
		m_MessageTitle.text = message.MessageSubject;
		m_MessageBody.text = message.MessageBody;
		m_MessageReceivedTime.text = GetReceivedTimeText(message.ReceivedTime);
		m_MessageSender.text = senderData.FurbyName;
		m_FurbyPortrait.spriteName = senderData.SpriteName;
		m_MessageBackground.spriteName = senderData.BackgroundSpriteName;
	}

	public string GetReceivedTimeText(DateTime dateTime)
	{
		int day = dateTime.Day;
		int month = dateTime.Month;
		int year = dateTime.Year;
		string text = Singleton<Localisation>.Instance.GetText(m_MonthStrings[month - 1]);
		string text2 = Singleton<Localisation>.Instance.GetText("FURMAIL_DATE");
		return string.Format(text2, day, text, year);
	}
}
