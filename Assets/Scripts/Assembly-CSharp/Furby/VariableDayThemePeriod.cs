using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class VariableDayThemePeriod : ThemePeriod
	{
		[Serializable]
		public class DayOnYear
		{
			public int year;

			public DateRange range;
		}

		[SerializeField]
		private List<DayOnYear> m_dates;

		private DayOnYear GetDayForYear(int year)
		{
			return m_dates.Find((DayOnYear d) => d.year == year);
		}

		protected override bool IsOn_(DateTime when)
		{
			bool result = false;
			DayOnYear dayForYear = GetDayForYear(when.Year);
			if (dayForYear != null)
			{
				DateRange range = dayForYear.range;
				result = range.Covers(when);
			}
			return result;
		}

		protected override DateTime GetStartDateFor_(DateTime when)
		{
			int year = when.Year;
			DayOnYear dayForYear = GetDayForYear(year);
			if (dayForYear == null)
			{
				throw new ArgumentOutOfRangeException("Year", year, "No dates set.");
			}
			DateRange range = dayForYear.range;
			return new DateTime(year, range.startMonth, range.startDay);
		}
	}
}
