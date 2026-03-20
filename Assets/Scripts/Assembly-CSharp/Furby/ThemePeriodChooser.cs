using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ThemePeriodChooser : MonoBehaviour
	{
		[SerializeField]
		private List<ThemePeriod> m_periods;

		public IEnumerable<ThemePeriod> Periods
		{
			get
			{
				return GetUnlockedPeriods();
			}
		}

		private IEnumerable<ThemePeriod> GetUnlockedPeriods()
		{
			foreach (ThemePeriod p in m_periods)
			{
				if (p.IsUnlockedNow())
				{
					yield return p;
				}
			}
		}

		public ThemePeriod GetPeriod()
		{
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			string userChoiceName = data.ThemePeriod;
			userChoiceName = ((userChoiceName != null) ? userChoiceName : string.Empty);
			DateTime themePeriodDateTime = data.ThemePeriodDateTime;
			ThemePeriod themePeriod = m_periods.Find((ThemePeriod tp) => tp.IsOnNow());
			ThemePeriod themePeriod2 = m_periods.Find((ThemePeriod tp) => tp.name == userChoiceName);
			ThemePeriod result = themePeriod2;
			if (themePeriod != null)
			{
				DateTime startDateForNow = themePeriod.GetStartDateForNow();
				if (startDateForNow > themePeriodDateTime)
				{
					result = themePeriod;
				}
			}
			return result;
		}
	}
}
