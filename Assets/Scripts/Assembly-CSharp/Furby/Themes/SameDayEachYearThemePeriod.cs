using System;
using UnityEngine;

namespace Furby.Themes
{
	public class SameDayEachYearThemePeriod : ThemePeriod
	{
		[SerializeField]
		private DateRange m_range;

		protected override bool IsOn_(DateTime when)
		{
			return m_range.Covers(when);
		}

		protected override DateTime GetStartDateFor_(DateTime when)
		{
			return new DateTime(when.Year, m_range.startMonth, m_range.startDay);
		}
	}
}
