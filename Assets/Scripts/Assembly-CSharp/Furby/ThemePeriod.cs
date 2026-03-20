using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public abstract class ThemePeriod : ScriptableObject
	{
		[Serializable]
		public class DateRange
		{
			public int startMonth;

			public int startDay;

			public int endMonth;

			public int endDay;

			public bool Covers(DateTime when)
			{
				DateTime dateTime = new DateTime(when.Year, startMonth, startDay);
				DateTime dateTime2 = new DateTime(when.Year, endMonth, endDay, 23, 59, 59);
				return dateTime <= when && when <= dateTime2;
			}
		}

		[SerializeField]
		private string m_debuggingNotes;

		[SerializeField]
		private int m_unlockYear = 1970;

		[SerializeField]
		private int m_unlockMonth = 1;

		[SerializeField]
		private int m_unlockDay = 1;

		[SerializeField]
		private bool m_onForDebugging;

		public string m_nameKey;

		public DateTime UnlockDay
		{
			get
			{
				return new DateTime(m_unlockYear, m_unlockMonth, m_unlockDay);
			}
		}

		public bool IsOnNow()
		{
			return IsOn(DateTime.Now);
		}

		public bool IsOn(DateTime when)
		{
			bool flag = IsOn_(when);
			return flag & IsUnlocked(when);
		}

		protected abstract bool IsOn_(DateTime when);

		public bool IsUnlockedNow()
		{
			return IsUnlocked(DateTime.Now);
		}

		public bool IsUnlocked(DateTime when)
		{
			DateTime unlockDay = UnlockDay;
			return when >= unlockDay;
		}

		public DateTime GetStartDateFor(DateTime when)
		{
			DateTime result = DateTime.MinValue;
			if (IsOn(when))
			{
				try
				{
					result = GetStartDateFor_(when);
				}
				catch (ArgumentOutOfRangeException)
				{
					Logging.Log(string.Format("Tried to get start date for {0}, but failed", when.ToString()));
				}
			}
			return result;
		}

		protected abstract DateTime GetStartDateFor_(DateTime when);

		public DateTime GetStartDateForNow()
		{
			return GetStartDateFor(DateTime.Now);
		}
	}
}
