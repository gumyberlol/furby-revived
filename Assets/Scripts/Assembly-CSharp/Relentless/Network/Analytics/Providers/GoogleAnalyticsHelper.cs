using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless.Network.Analytics.Providers
{
	public sealed class GoogleAnalyticsHelper
	{
		public class GIFParams
		{
			public string utmcs = "-";

			public string utmsr;

			public string utmsc;

			public string utmul;

			public string utmje = "0";

			public string utmfl;
		}

		private string m_accountid;

		private string m_domain;

		private bool m_initialised;

		public void Initialise(string accountid, string domain)
		{
			if (!m_initialised)
			{
				m_accountid = accountid;
				m_domain = domain;
				m_initialised = true;
			}
		}

		public void LogPage(string page)
		{
			LogEvent(page, string.Empty, string.Empty, string.Empty, 0.0);
		}

		public void LogEvent(string page, string category, string action, string opt_label, double opt_value)
		{
			if (!m_initialised || string.IsNullOrEmpty(m_domain))
			{
				Logging.Log("GoogleAnalytics not initialised!");
				return;
			}
			long num = UnityEngine.Random.Range(10000000, 99999999);
			long num2 = UnityEngine.Random.Range(1000000000, 2000000000);
			long epochTime = GetEpochTime();
			string text = "%3D";
			string text2 = "%7C";
			string text3 = num + "." + num2 + "." + epochTime + "." + epochTime + "." + epochTime + ".2" + WWW.EscapeURL(";") + WWW.EscapeURL("+");
			string text4 = "utmcsr" + text + "(direct)" + text2 + "utmccn" + text + "(direct)" + text2 + "utmcmd" + text + "(none)" + WWW.EscapeURL(";");
			string text5 = num + "." + epochTime + "2.2.2." + text4;
			if (page.Length == 0)
			{
				page = Application.loadedLevelName;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("utmwv", "4.6.5");
			dictionary.Add("utmn", num2.ToString());
			dictionary.Add("utmhn", WWW.EscapeURL(m_domain));
			dictionary.Add("utmcs", "UTF-8");
			dictionary.Add("utmsr", Screen.currentResolution.width + "x" + Screen.currentResolution.height);
			dictionary.Add("utmsc", "24-bit");
			dictionary.Add("utmul", "en-gb");
			dictionary.Add("utmje", "0");
			dictionary.Add("utmfl", "-");
			dictionary.Add("utmdt", WWW.EscapeURL(page));
			dictionary.Add("utmhid", num2.ToString());
			dictionary.Add("utmr", "-");
			dictionary.Add("utmp", WWW.EscapeURL(page));
			dictionary.Add("utmac", m_accountid);
			dictionary.Add("utmcc", "__utma" + text + text3 + "__utmz" + text + text5);
			if (category.Length > 0 && action.Length > 0)
			{
				string text6 = "5(" + category + "*" + action;
				if (opt_label.Length > 0)
				{
					string text7 = text6;
					text6 = text7 + "*" + opt_label + ")(" + opt_value;
				}
				text6 += ")";
				dictionary.Add("utme", text6);
				dictionary.Add("utmt", "event");
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				list.Add(item.Key + "=" + item.Value);
			}
			string text8 = "http://www.google-analytics.com/__utm.gif?" + string.Join("&", list.ToArray());
			Logging.Log("[Google URL]" + text8);
			if (text8.Length > 2048)
			{
				Logging.LogError("[Google URL] is too long (>2048)");
				return;
			}
			WWW wWW = new WWW(text8);
			while (!wWW.isDone)
			{
			}
			if (!string.IsNullOrEmpty(wWW.error))
			{
				Logging.Log("Google Analytics failed : " + wWW.error);
			}
		}

		private long GetEpochTime()
		{
			DateTime now = DateTime.Now;
			DateTime value = Convert.ToDateTime("1/1/1970 0:00:00 AM");
			TimeSpan timeSpan = now.Subtract(value);
			return ((timeSpan.Days * 24 + timeSpan.Hours) * 60 + timeSpan.Minutes) * 60 + timeSpan.Seconds;
		}
	}
}
