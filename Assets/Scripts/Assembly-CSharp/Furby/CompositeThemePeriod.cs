using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class CompositeThemePeriod : ThemePeriod
	{
		[SerializeField]
		private List<ThemePeriod> m_periods;

		protected override bool IsOn_(DateTime when)
		{
			ThemePeriod themePeriod = m_periods.Find((ThemePeriod d) => d.IsOn(when));
			return themePeriod != null;
		}

		protected override DateTime GetStartDateFor_(DateTime when)
		{
			foreach (ThemePeriod period in m_periods)
			{
				try
				{
					return period.GetStartDateFor(when);
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
